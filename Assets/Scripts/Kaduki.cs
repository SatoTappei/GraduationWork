using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class Kaduki : Adventurer
    {
        [SerializeField] Vector2Int _spawnCoords;
        [SerializeField] AudioClip _punchHitSE;
        [SerializeField] AudioClip _deathSE;

        DungeonManager _dungeonManager;
        UiManager _uiManager;
        Animator _animator;
        AudioSource _audioSource;
        AdventurerAI _adventurerAI;

        Queue<SharedInformation> _pendingInfomation;
        Vector2Int _currentCoords;
        Vector2Int _currentDirection;
        string _pathTarget;
        int _currentPathIndex;
        List<Cell> _path;
        int _statusBarID;
        int _profileWindowID;
        bool _isKnockback;

        public override Vector2Int Coords => _currentCoords;
        public override Vector2Int Direction => _currentDirection;

        void Awake()
        {
            _dungeonManager = DungeonManager.Find();
            _uiManager = UiManager.Find();
            _animator = GetComponentInChildren<Animator>();
            _audioSource = GetComponent<AudioSource>();
            _adventurerAI = GetComponent<AdventurerAI>();
            CurrentHp = MaxHp;
            CurrentEmotion = MaxEmotion;
            Item = new string[3];
            Information = new SharedInformation[4];
            ExploreRecord = new ExploreRecord(Blueprint.Height, Blueprint.Width);
            ActionLog = new Queue<string>();

            // �ŏ�����m���Ă������AI�����肷��̂Ɏg���^�ɕϊ�����B
            for (int i = 0; i < AdventurerSheet.DecisionSupportContext.Length; i++)
            {
                BilingualString text = AdventurerSheet.DecisionSupportContext[i];
                if (text.Japanese != string.Empty && text.English != string.Empty)
                {
                    Information[i] = new SharedInformation(text, "Myself");
                }
            }

            AvailableActions = new List<string>()
            {
                "Move Forward",
                "Move North",
                "Move South",
                "Move East",
                "Move West",
                "Attack Surrounding",
                "Scavenge Surrounding",
                "Talk Surrounding"
            };

            _currentCoords = _spawnCoords;
            _currentDirection = Vector2Int.up;
            _path = new List<Cell>();
            _pendingInfomation = new Queue<SharedInformation>();
        }

        void Start()
        {
            _dungeonManager.AddActorOnCell(_currentCoords, this);
            Cell cell = _dungeonManager.GetCell(_currentCoords);
            transform.position = cell.Position;

            _statusBarID = _uiManager.RegisterToStatusBar(this);
            _uiManager.ShowLine(_statusBarID, "����ɂ��́B");
            _uiManager.AddLog($"{AdventurerSheet.DisplayName}���_���W�����ɂ���Ă����B");
            _profileWindowID = _uiManager.RegisterToProfileWindow(this);

            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
            EvaluatePendingInfomationAsync().Forget();
        }

        void OnDestroy()
        {
            if (_uiManager != null) _uiManager.DeleteStatusBarStatus(_statusBarID);
        }

        async UniTask UpdateAsync(CancellationToken token)
        {
            SubGoals = await _adventurerAI.SelectSubGoalAsync();
            CurrentSubGoalIndex = 0;

            while (true)
            {
                _uiManager.UpdateProfileWindowStatus(_profileWindowID, this);

                ElapsedTurn++;

                string selected = await _adventurerAI.SelectNextActionAsync();
                if (selected == "Move Forward") await MoveAsync(_currentDirection);
                else if (selected == "Move North") await MoveAsync(Vector2Int.up);
                else if (selected == "Move South") await MoveAsync(Vector2Int.down);
                else if (selected == "Move East") await MoveAsync(Vector2Int.right);
                else if (selected == "Move West") await MoveAsync(Vector2Int.left);
                else if (selected == "Return To Entrance") await MoveAsync("Entrance");
                else if (selected == "Attack Surrounding") await AttackAsync();
                else if (selected == "Scavenge Surrounding") await ScavAsync();
                else if (selected == "Talk Surrounding") await TalkAsync();

                if (await DeathAsync() || await EscapeAsync()) break;

                // �T�u�S�[����B�������ꍇ�A���̃T�u�S�[����ݒ�B
                if (SubGoals[CurrentSubGoalIndex].IsCompleted())
                {
                    CurrentSubGoalIndex++;

                    // ���p�\�ȍs���̑I����������ꍇ�͒ǉ��B
                    IEnumerable<string> choices = SubGoals[CurrentSubGoalIndex].GetAdditionalChoices();
                    AvailableActions.AddRange(choices);
                }

                await UniTask.Yield();
            }

            Destroy(gameObject);
        }

        async UniTask EvaluatePendingInfomationAsync()
        {
            while (true)
            {
                if (_pendingInfomation.TryDequeue(out SharedInformation info))
                {
                    info.Score = await _adventurerAI.EvaluateInformationAsync(info);
                    List<SharedInformation> temp = Information.ToList();
                    temp.Add(info);
                    Sort(temp, 0, temp.Count);

                    if (temp.Count > 4)
                    {
                        temp.RemoveAt(temp.Count - 1);
                    }

                    Information = temp.ToArray();

                    TalkContent = await _adventurerAI.SelectTalkContentAsync(Information);
                }

                await UniTask.Yield();
            }
        }

        // �ׂ̃Z���Ɉړ��B
        async UniTask MoveAsync(Vector2Int direction)
        {
            Cell targetCell = _dungeonManager.GetCell(_currentCoords + direction);
            if (targetCell.IsPassable())
            {
                _dungeonManager.Pathfinding(_currentCoords, _currentCoords + direction, _path);
                _pathTarget = direction.ToString();
            }
            else
            {
                // �o�H�T�����o���Ȃ��̂Œ��ڍX�V�B�ړ��������������ς��邽�߂Ɉړ��������s���B
                _path.Clear();
                _path.Add(targetCell);
                _pathTarget = $"{direction}(IsImpassable)";
            }

            _currentPathIndex = 0;

            await MoveNextCellAsync();
        }

        // �o�H�ɉ����Ĉړ��B
        async UniTask MoveAsync(string target)
        {
            // ���݂̌o�H�ƈႤ�ڕW��I�������ꍇ�͍ēx�o�H�T���B
            if (_pathTarget != target)
            {
                if (target == "Treasure") PathfindingToTreasure();
                else if (target == "Enemy") PathfindingToEnemy();
                else if (target == "Entrance") PathfindingToEntrance();
                else Debug.LogWarning($"�Ή�����ڕW�����݂��Ȃ����ߌo�H�T�����o���Ȃ��B: {target}");

                _pathTarget = target;
                _currentPathIndex = 0;
            }

            await MoveNextCellAsync();
        }

        // �Ƃ肠�����A�o�H�T�����邲�ƂɃ����_���ȕ󔠂�I�Ԃ悤�ɂ��Ă����B
        // ��X�A�s�����f��A�ēx�T������ۂɓ����󔠂�I�Ԃ悤�ȏ����ɂ������B
        void PathfindingToTreasure()
        {
            List<Cell> targetCells = _dungeonManager.GetCells("Treasure").ToList();
            int i = Random.Range(0,targetCells.Count);
            Actor treasure = targetCells[i].GetActors().Where(a => a.ID == "Treasure").First();

            // �󔠂̃}�X�ւ͌o�H�T�����o���Ȃ��̂ŁA���ʂ̈ʒu�܂ł̌o�H�T���B
            Vector2Int goalCoords = treasure.Coords;
            if (treasure.Direction == Vector2Int.up) goalCoords += Vector2Int.up;
            else if (treasure.Direction == Vector2Int.down) goalCoords += Vector2Int.down;
            else if (treasure.Direction == Vector2Int.left) goalCoords += Vector2Int.left;
            else if (treasure.Direction == Vector2Int.right) goalCoords += Vector2Int.right;

            _dungeonManager.Pathfinding(_currentCoords, goalCoords, _path);
        }

        // �Ƃ肠�����A�o�H�T�����邲�ƂɃ����_���ȓG��I�Ԃ悤�ɂ��Ă����B
        // ��X�A�s�����f��A�ēx�T������ۂɓ����G��I�Ԃ悤�ȏ����ɂ������B
        void PathfindingToEnemy()
        {
            List<Cell> targetCells = _dungeonManager.GetCells("BlackKaduki").ToList();
            int i = Random.Range(0, targetCells.Count);
            Actor enemy = targetCells[i].GetActors().Where(a => a.ID == "BlackKaduki").First();

            // �G�̃}�X�ւ͌o�H�T�����o���Ȃ��̂ŁA���͂̈ʒu�܂ł̌o�H�T���B
            foreach (Vector2Int dir in GetDirection())
            {
                Vector2Int goalCoords = enemy.Coords + dir;
                if (_dungeonManager.GetCell(goalCoords).IsPassable())
                {
                    _dungeonManager.Pathfinding(_currentCoords, goalCoords, _path);
                    break;
                }
            }

            IEnumerable<Vector2Int> GetDirection()
            {
                yield return Vector2Int.up;
                yield return Vector2Int.down;
                yield return Vector2Int.left;
                yield return Vector2Int.right;
            }
        }

        // �����ւ̌o�H�T���B
        void PathfindingToEntrance()
        {
            List<Cell> targetCells = _dungeonManager.GetCells("Entrance").ToList();
            int i = Random.Range(0, targetCells.Count);
            Actor entrance = targetCells[i].GetActors().Where(a => a.ID == "Entrance").First();

            _dungeonManager.Pathfinding(_currentCoords, entrance.Coords, _path);
        }

        // ���̃Z���Ɉړ��B
        async UniTask MoveNextCellAsync()
        {
            if (_path.Count == 0) return;

            // ���g�̌����Ă�������X�V�B
            _currentDirection = _path[_currentPathIndex].Coords - Coords;

            // �ڂ̑O�̃Z���̕������m�F���ăh�A���`�F�b�N���ĊJ����B
            Vector2Int frontCoords = _currentCoords + _currentDirection;
            if ("2468".Contains(Blueprint.Doors[frontCoords.y][frontCoords.x]))
            {
                Actor actor = _dungeonManager.GetActorsOnCell(frontCoords).Where(c => c.ID == "Door").FirstOrDefault();
                if (actor != null && actor is DungeonEntity door)
                {
                    door.Interact(actor);
                }
            }

            string directionName = default;
            Vector2Int coordsDirection = frontCoords - _currentCoords;
            if (coordsDirection == Vector2Int.up) directionName = "north";
            if (coordsDirection == Vector2Int.down) directionName = "south";
            if (coordsDirection == Vector2Int.left) directionName = "west";
            if (coordsDirection == Vector2Int.right) directionName = "east";

            if (_path[_currentPathIndex].IsPassable())
            {
                // �ڂ̑O�̃Z���Ɏ��g��o�^�B���̃L�����N�^�[�Ƃ̌����͋��e����̂ŉ���Z���ɂ͓o�^���Ȃ��B
                _dungeonManager.RemoveActorOnCell(_currentCoords, this);
                _currentCoords = _path[_currentPathIndex].Coords;
                _dungeonManager.AddActorOnCell(_currentCoords, this);

                _animator.Play("Walk");

                Vector3 startPosition = transform.position;
                Vector3 goalPosition = _path[_currentPathIndex].Position;
                Transform axis = transform.Find("ForwardAxis");
                Vector3 goalDirection = (goalPosition - startPosition).normalized;
                Quaternion startRotation = axis.rotation;
                Quaternion goalRotation;
                if (goalDirection == Vector3.zero) goalRotation = Quaternion.identity;
                else goalRotation = Quaternion.LookRotation(goalDirection);
                for (float t = 0; t <= 1; t += Time.deltaTime * 1.0f)
                {
                    axis.rotation = Quaternion.Lerp(startRotation, goalRotation, t * 4);
                    transform.position = Vector3.Lerp(startPosition, goalPosition, t);
                    await UniTask.Yield();
                }

                _animator.Play("Idle");

                _currentPathIndex++;
                _currentPathIndex = Mathf.Min(_currentPathIndex, _path.Count - 1);

                AddActionLog($"turn{ElapsedTurn}: Successfully moved to the {directionName}.");
                UpdateExploreRecord(_currentCoords);
            }
            else
            {
                Vector3 startPosition = transform.position;
                Vector3 goalPosition = _path[_currentPathIndex].Position;
                Transform axis = transform.Find("ForwardAxis");
                Vector3 goalDirection = (goalPosition - startPosition).normalized;
                Quaternion startRotation = axis.rotation;
                Quaternion goalRotation;
                if (goalDirection == Vector3.zero) goalRotation = Quaternion.identity;
                else goalRotation = Quaternion.LookRotation(goalDirection);
                for (float t = 0; t <= 1; t += Time.deltaTime * 1.0f)
                {
                    axis.rotation = Quaternion.Lerp(startRotation, goalRotation, t * 4);
                    await UniTask.Yield();
                }

                _currentPathIndex++;
                _currentPathIndex = Mathf.Min(_currentPathIndex, _path.Count - 1);

                AddActionLog($"turn{ElapsedTurn}: Failed to move to the {directionName}. Cannot move in this direction.");
            }
        }

        // ���͂�Actor�ɍU������B
        async UniTask AttackAsync()
        {
            // ���͂̃Z������ڕW��I�ԁB
            IDamageable targetDamageable = null;
            Actor targetActor = null;
            for (int i = -1; i <= 1; i++)
            {
                for (int k = -1; k <= 1; k++)
                {
                    // �㉺���E��4�����̂݁B
                    if ((i == 0 && k == 0) || Mathf.Abs(i * k) > 0) continue;

                    Vector2Int neighbourCoords = new Vector2Int(_currentCoords.x + k, _currentCoords.y + i);
                    Cell cell = _dungeonManager.GetCell(neighbourCoords);

                    if (cell.GetActors().Count == 0) continue;

                    foreach (Actor actor in cell.GetActors())
                    {
                        if (actor is IDamageable damageable)
                        {
                            targetDamageable = damageable;
                            targetActor = actor;
                            break;
                        }
                    }

                    if (targetDamageable != null) break;
                }

                if (targetDamageable != null) break;
            }

            if (targetDamageable == null)
            {
                AddActionLog($"turn{ElapsedTurn}: attack neighbour target failure.");
                return;
            }

            // �ڕW�������B
            Vector3 position = _dungeonManager.GetCell(_currentCoords).Position;
            Vector3 targetPosition = _dungeonManager.GetCell(targetActor.Coords).Position;
            Transform axis = transform.Find("ForwardAxis");
            Vector3 goalDirection = (targetPosition - position).normalized;
            Quaternion startRotation = axis.rotation;
            Quaternion goalRotation;
            if (goalDirection == Vector3.zero) goalRotation = Quaternion.identity;
            else goalRotation = Quaternion.LookRotation(goalDirection);
            for (float t = 0; t <= 1; t += Time.deltaTime * 1.0f)
            {
                axis.rotation = Quaternion.Lerp(startRotation, goalRotation, t * 4);

                await UniTask.Yield();
            }

            _animator.Play("Attack");
            _uiManager.ShowLine(_statusBarID, "�U������B");

            string result = targetDamageable.Damage(ID, "�p���`", 34, _currentCoords);
            if (result == "Defeated")
            {
                _uiManager.ShowLine(_statusBarID, "�E�����B");
                _uiManager.AddLog("Kaduki��BlackKaduki���E�����B");
                DefeatCount++;
                AddActionLog($"turn{ElapsedTurn}: we attacked the enemy. And defeated them.");
            }

            AddActionLog($"turn{ElapsedTurn}: we attacked the enemy. The enemy is still alive.");
        }

        // ���͂�Actor������B
        async UniTask ScavAsync()
        {
            // ���͂̃Z������ڕW��I�ԁB
            IScavengeable targetScavengeable = null;
            Actor targetActor = null;
            for (int i = -1; i <= 1; i++)
            {
                for (int k = -1; k <= 1; k++)
                {
                    // �㉺���E��4�����̂݁B
                    if ((i == 0 && k == 0) || Mathf.Abs(i * k) > 0) continue;

                    Vector2Int neighbourCoords = new Vector2Int(_currentCoords.x + k, _currentCoords.y + i);
                    Cell cell = _dungeonManager.GetCell(neighbourCoords);

                    foreach (Actor actor in cell.GetActors())
                    {
                        // �󔠂�D�悵�ċ���悤�ɂ������̂ŁA���������_�Ń��[�v���甲����B
                        if (actor.ID == "Treasure")
                        {
                            targetScavengeable = actor as IScavengeable;
                            targetActor = actor;
                            break;
                        }
                        else if(actor is IScavengeable scav)
                        {
                            targetScavengeable = scav;
                            targetActor = actor;
                        }
                    }

                    if (targetActor != null && targetActor.ID == "Treasure") break;
                }

                if (targetActor != null && targetActor.ID == "Treasure") break;
            }

            if (targetScavengeable == null)
            {
                AddActionLog($"turn{ElapsedTurn}: scavenge neighbour target failure.");
                return;
            }
            
            // �ڕW�������B
            Vector3 position = _dungeonManager.GetCell(_currentCoords).Position;
            Vector3 targetPosition = _dungeonManager.GetCell(targetActor.Coords).Position;
            Transform axis = transform.Find("ForwardAxis");
            Vector3 goalDirection = (targetPosition - position).normalized;
            Quaternion startRotation = axis.rotation;
            Quaternion goalRotation;
            if (goalDirection == Vector3.zero) goalRotation = Quaternion.identity;
            else goalRotation = Quaternion.LookRotation(goalDirection);
            for (float t = 0; t <= 1; t += Time.deltaTime * 1.0f)
            {
                axis.rotation = Quaternion.Lerp(startRotation, goalRotation, t * 4);

                await UniTask.Yield();
            }

            _animator.Play("Scav");

            targetScavengeable.Scavenge();
            if (targetActor.ID == "Treasure")
            {
                TreasureCount++;
                _uiManager.ShowLine(_statusBarID, "������B");
            }

            AddActionLog($"turn{ElapsedTurn}: scavenge neighbour target success.");
        }

        // ���͂�Adventure�Ɖ�b����B
        async UniTask TalkAsync()
        {
            const float ParticlePlayTime = 5.0f;
            const float AnimationPlayTime = 7.0f;

            // ���͂̃Z������ڕW��I�ԁB
            ITalkable targetTalkable = null;
            Actor targetActor = null;
            for (int i = -1; i <= 1; i++)
            {
                for (int k = -1; k <= 1; k++)
                {
                    // �㉺���E��4�����̂݁B
                    if ((i == 0 && k == 0) || Mathf.Abs(i * k) > 0) continue;

                    Vector2Int neighbourCoords = new Vector2Int(_currentCoords.x + k, _currentCoords.y + i);
                    Cell cell = _dungeonManager.GetCell(neighbourCoords);

                    if (cell.GetActors().Count == 0) continue;

                    foreach (Actor actor in cell.GetActors())
                    {
                        if (actor is ITalkable talkable)
                        {
                            targetTalkable = talkable;
                            targetActor = actor;
                            break;
                        }
                    }

                    if (targetTalkable != null) break;
                }

                if (targetTalkable != null) break;
            }

            if (targetTalkable == null)
            {
                AddActionLog($"turn{ElapsedTurn}: talk neighbour target failure.");
                return;
            }

            // �ڕW�������B
            Vector3 position = _dungeonManager.GetCell(_currentCoords).Position;
            Vector3 targetPosition = _dungeonManager.GetCell(targetActor.Coords).Position;
            Transform axis = transform.Find("ForwardAxis");
            Vector3 goalDirection = (targetPosition - position).normalized;
            Quaternion startRotation = axis.rotation;
            Quaternion goalRotation;
            if (goalDirection == Vector3.zero) goalRotation = Quaternion.identity;
            else goalRotation = Quaternion.LookRotation(goalDirection);
            for (float t = 0; t <= 1; t += Time.deltaTime * 1.0f)
            {
                axis.rotation = Quaternion.Lerp(startRotation, goalRotation, t * 4);

                await UniTask.Yield();
            }

            _animator.Play("Talk");
            _uiManager.ShowLine(_statusBarID, "��b����B");

            ParticleSystem particle = transform.Find("ForwardAxis")
                .Find("Particle_Talk").GetComponent<ParticleSystem>();
            particle.Play();

            targetTalkable.Talk(ID, TalkContent.Text, _currentCoords);

            AddActionLog($"turn{ElapsedTurn}: talk neighbour target success.");

            await UniTask.WaitForSeconds(Mathf.Max(ParticlePlayTime, AnimationPlayTime));
        }

        // ���S���Ă���ꍇ�͉��o���Đ���true��Ԃ��B�����Ă���ꍇ�͉�������false��Ԃ��B
        async UniTask<bool> DeathAsync()
        {
            const float AnimationLength = 2.5f;

            if (CurrentHp > 0) return false;

            _animator.Play("Death");
            _audioSource.clip = _deathSE;
            _audioSource.Play();
            _uiManager.ShowLine(_statusBarID, "���S�����B");

            await UniTask.WaitForSeconds(AnimationLength);
            
            _dungeonManager.RemoveActorOnCell(_currentCoords, this);

            return true;
        }

        // �_���[�W���󂯂�B
        public override string Damage(string id, string weapon, int value, Vector2Int coords)
        {
            if (CurrentHp <= 0) return "Corpse";

            CurrentHp -= value;
            CurrentHp = Mathf.Max(0, CurrentHp);
            _uiManager.UpdateStatusBarStatus(_statusBarID, this);
            _uiManager.ShowLine(_statusBarID, "�_���[�W���󂯂��B");

            if (!_isKnockback) StartCoroutine(HitEffectAsync(coords));

            if (CurrentHp <= 0) return "Defeated";
            else return "Hit";
        }

        // �U�������g�Ƀq�b�g�������o�B
        IEnumerator HitEffectAsync(Vector2Int coords)
        {
            _isKnockback = true;

            ParticleSystem particle = transform.Find("ForwardAxis")
                .Find("Particle_Damage").GetComponent<ParticleSystem>();
            particle.Play();

            _audioSource.clip = _punchHitSE;
            _audioSource.Play();

            Vector2Int diff = coords - _currentCoords;
            Vector3 forward = new Vector3(diff.x, 0, diff.y);
            yield return KnockbackAsync(-forward);
            yield return KnockbackAsync(forward);

            Transform fbx = transform.Find("ForwardAxis").Find("FBX");
            fbx.localPosition = Vector3.zero;

            _isKnockback = false;
        }

        // �m�b�N�o�b�N�B
        IEnumerator KnockbackAsync(Vector3 direction)
        {
            const float Speed = 10.0f;
            const float Distance = 0.2f;

            Transform fbx = transform.GetChild(0).GetChild(0);
            Vector3 start = fbx.position;
            Vector3 goal = start + direction * Distance;
            for (float t = 0; t <= 1; t += Time.deltaTime * Speed)
            {
                fbx.position = Vector3.Lerp(start, goal, t);
                yield return null;
            }
        }

        // �E�o�����𖞂����Ă���ꍇ�͉��o���Đ���true��Ԃ��B����ȊO�̏ꍇ�͉�������false��Ԃ��B
        async UniTask<bool> EscapeAsync()
        {
            const float AnimationLength = 1.0f * 2;

            if (TreasureCount == 0 && DefeatCount == 0) return false;
            else if (Blueprint.Interaction[_currentCoords.y][_currentCoords.x] != '<') return false;

            _animator.Play("Jump");
            _uiManager.ShowLine(_statusBarID, "�ړI��B�����ĒE�o�B");
            _uiManager.AddLog("Kaduki���_���W��������E�o�����B");

            await UniTask.WaitForSeconds(AnimationLength);

            _dungeonManager.RemoveActorOnCell(_currentCoords, this);

            return true;
        }
        
        // ���͂̉�b�\�ȃL�����N�^�[�Ɖ�b����B
        public override void Talk(string id, BilingualString text, Vector2Int coords)
        {
            SharedInformation info = new SharedInformation(text, "Adventurer");
            _pendingInfomation.Enqueue(info);
        }

        // �s�����O��ǉ��B
        void AddActionLog(string text)
        {
            ActionLog.Enqueue(text);
            
            if (ActionLog.Count > 10) ActionLog.Dequeue();
        }

        void UpdateExploreRecord(Vector2Int coords)
        {
            ExploreRecord.IncreaseCount(coords);
        }

        static void Sort(List<SharedInformation> list, int left, int right)
        {
            if (left >= right) return;

            float pivot = list[right].Score;
            int current = left;
            for (int i = left; i <= right - 1; i++)
            {
                if (list[i].Score > pivot)
                {
                    Swap(list, current, i);
                    current++;
                }
            }

            Swap(list, current, right);

            Sort(list, left, current - 1);
            Sort(list, current + 1, right);
        }

        static void Swap(List<SharedInformation> list, int a, int b)
        {
            SharedInformation x = list[a];
            list[a] = list[b];
            list[b] = x;
        }
    }
}