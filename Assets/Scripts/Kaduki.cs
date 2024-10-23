using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Game
{
    // ��������C�x���g���N�����ꍇ��AI�����̍s���𔻒f����B(�ړ����ɓG�ɉ���ꂽ�Ȃ�)
    // �����Ȃ��ꍇ�͂��̍s�����p������B(�ڕW�Ɍ����Ĉړ�����Ȃ�)
    //  �ڕW�Ɍ����Ĉړ�����B
    //   �󔠁A�G�A�Ȃ���p�A�����B
    //    �o�H�T��->����->�ړ��B
    //  ���͂ɃC���^���N�g�B
    //   �G�ɍU���A�󔠂Ȃǂ𒲂ׂ�B���̃L�����N�^�[�ɘb��������B
    //  �ҋ@�B
    // ���ʂ�m��B
    // �U�����ꂽ��b��������ꂽ��ȂǑ��L�����N�^�[����̐ڐG�B
    //  1�����Ƃɒ��f����(�_���[�W�⎀�S�Ȃ�)���K�v
    // ���񂾂�E�o�����ꍇ�͂����ŕ���B
    public class Kaduki : Adventure, IStatusBarDisplayStatus
    {
        [SerializeField] Vector2Int _spawnCoords;
        [SerializeField] AudioClip _punchHitSE;
        [SerializeField] AudioClip _deathSE;
        [SerializeField] Sprite _icon;

        DungeonManager _dungeonManager;
        UiManager _uiManager;
        Animator _animator;
        AudioSource _audioSource;
        AdventureAI _adventureAI;

        Vector2Int _currentCoords;
        Vector2Int _currentDirection;
        string _pathTarget;
        int _currentPathIndex;
        List<Cell> _path;
        int _statusBarID;
        bool _isKnockback;
        int _currentHp;
        int _currentEmotion;
        int _treasureCount;
        int _defeatCount;

        public override Vector2Int Coords => _currentCoords;
        public override Vector2Int Direction => _currentDirection;

        public Sprite Icon => _icon;
        public string DisplayName => "Kaduki";
        public int MaxHp => 100;
        public int CurrentHp => _currentHp;
        public int MaxEmotion => 100;
        public int CurrentEmotion => _currentEmotion;

        void Awake()
        {
            _dungeonManager = DungeonManager.Find();
            _uiManager = UiManager.Find();
            _animator = GetComponentInChildren<Animator>();
            _audioSource = GetComponent<AudioSource>();
            _adventureAI = GetComponent<AdventureAI>();
            _currentHp = MaxHp;
            _currentEmotion = MaxEmotion;
        }

        void Start()
        {
            _currentCoords = _spawnCoords;
            _currentDirection = Vector2Int.up;
            _path = new List<Cell>();

            _dungeonManager.AddActorOnCell(_currentCoords, this);
            Cell cell = _dungeonManager.GetCell(_currentCoords);
            transform.position = cell.Position;

            _statusBarID = _uiManager.RegisterToStatusBar(this);
            _uiManager.ShowLine(_statusBarID, "����ɂ��́B");
            _uiManager.AddLog("Kaduki���_���W�����ɂ���Ă����B");

            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        void OnDestroy()
        {
            if (_uiManager != null) _uiManager.DeleteStatusBarStatus(_statusBarID);
        }

        async UniTask UpdateAsync(CancellationToken token)
        {
            while (true)
            {
                string selected = await _adventureAI.SelectNextActionAsync();
                if (selected == "Move Treasure") await MoveAsync("Treasure");
                else if (selected == "Move Enemy") await MoveAsync("Enemy");
                else if (selected == "Move Entrance") await MoveAsync("Entrance");
                else if (selected == "Interact Attack") await AttackAsync();
                else if (selected == "Interact Scav") await ScavAsync();
                else if (selected == "Interact Talk") await TalkAsync();

                ReportActionResult();

                if (await DeathAsync() || await EscapeAsync()) break;

                await UniTask.Yield();
            }

            Destroy(gameObject);
        }

        // �ׂ̃Z���Ɉړ��B
        async UniTask MoveAsync(string target)
        {
            PathfindingIfTargetChanged(target);
            await MoveNextCellAsync();
        }

        // ���݂̌o�H�ƈႤ�ڕW��I�������ꍇ�͍ēx�o�H�T���B
        void PathfindingIfTargetChanged(string target)
        {
            if (_pathTarget != target)
            {
                if (target == "Treasure") PathfindingToTreasure();
                else if (target == "Enemy") PathfindingToEnemy();
                else if (target == "Entrance") PathfindingToEntrance();

                _pathTarget = target;
                _currentPathIndex = 0;
            }
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

        // �_���W��������K���ɂ��낤�낷��ꍇ�A����炵���ړ��悪�K�v�H
        void PathfindingToCheckPoint()
        {
            // 
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

            // �ڂ̑O�̃Z���Ɏ��g��o�^�B���̃L�����N�^�[�Ƃ̌����͋��e����̂ŉ���Z���ɂ͓o�^���Ȃ��B
            _dungeonManager.RemoveActorOnCell(_currentCoords, this);
            _currentCoords = _path[_currentPathIndex].Coords;
            _dungeonManager.AddActorOnCell(_currentCoords, this);

            _animator.Play("Walk");

            // �ړ��B
            Vector3 startPosition = transform.position;
            Vector3 goalPosition = _path[_currentPathIndex].Position;
            Transform axis = transform.Find("ForwardAxis");
            Vector3 goalDirection = (goalPosition - startPosition).normalized;
            Quaternion startRotation = axis.rotation;
            Quaternion goalRotation = Quaternion.LookRotation(goalDirection);
            for (float t = 0; t <= 1; t += Time.deltaTime * 1.0f)
            {
                axis.rotation = Quaternion.Lerp(startRotation, goalRotation, t * 4);
                transform.position = Vector3.Lerp(startPosition, goalPosition, t);

                await UniTask.Yield();
            }

            _animator.Play("Idle");

            _currentPathIndex++;
            _currentPathIndex = Mathf.Min(_currentPathIndex, _path.Count - 1);
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

            if (targetDamageable == null) return;

            // �ڕW�������B
            Vector3 position = _dungeonManager.GetCell(_currentCoords).Position;
            Vector3 targetPosition = _dungeonManager.GetCell(targetActor.Coords).Position;
            Transform axis = transform.Find("ForwardAxis");
            Vector3 goalDirection = (targetPosition - position).normalized;
            Quaternion startRotation = axis.rotation;
            Quaternion goalRotation = Quaternion.LookRotation(goalDirection);
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
                _defeatCount++;
            }
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

            if (targetScavengeable == null) return;
            
            // �ڕW�������B
            Vector3 position = _dungeonManager.GetCell(_currentCoords).Position;
            Vector3 targetPosition = _dungeonManager.GetCell(targetActor.Coords).Position;
            Transform axis = transform.Find("ForwardAxis");
            Vector3 goalDirection = (targetPosition - position).normalized;
            Quaternion startRotation = axis.rotation;
            Quaternion goalRotation = Quaternion.LookRotation(goalDirection);
            for (float t = 0; t <= 1; t += Time.deltaTime * 1.0f)
            {
                axis.rotation = Quaternion.Lerp(startRotation, goalRotation, t * 4);

                await UniTask.Yield();
            }

            _animator.Play("Scav");

            targetScavengeable.Scavenge();
            if (targetActor.ID == "Treasure")
            {
                _treasureCount++;
                _uiManager.ShowLine(_statusBarID, "������B");
            }
        }

        // ���͂�Adventure�Ɖ�b����B
        async UniTask TalkAsync()
        {
            //
        }

        // ���S���Ă���ꍇ�͉��o���Đ���true��Ԃ��B�����Ă���ꍇ�͉�������false��Ԃ��B
        async UniTask<bool> DeathAsync()
        {
            const float AnimationLength = 2.5f;

            if (_currentHp > 0) return false;

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
            if (_currentHp <= 0) return "Corpse";

            _currentHp -= value;
            _currentHp = Mathf.Max(0, _currentHp);
            _uiManager.UpdateStatusBarStatus(_statusBarID, this);
            _uiManager.ShowLine(_statusBarID, "�_���[�W���󂯂��B");

            if (!_isKnockback) StartCoroutine(HitEffectAsync(coords));

            if (_currentHp <= 0) return "Defeated";
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

            if (_treasureCount == 0 && _defeatCount == 0) return false;
            else if (Blueprint.Interaction[_currentCoords.y][_currentCoords.x] != '<') return false;

            _animator.Play("Jump");
            _uiManager.ShowLine(_statusBarID, "�ړI��B�����ĒE�o�B");
            _uiManager.AddLog("Kaduki���_���W��������E�o�����B");

            await UniTask.WaitForSeconds(AnimationLength);

            _dungeonManager.RemoveActorOnCell(_currentCoords, this);

            return true;
        }

        // �s���̌��ʂ�AI�ɕ񍐁B
        void ReportActionResult()
        {
            Cell upCell = _dungeonManager.GetCell(_currentCoords + Vector2Int.up);
            Cell downCell = _dungeonManager.GetCell(_currentCoords + Vector2Int.down);
            Cell leftCell = _dungeonManager.GetCell(_currentCoords + Vector2Int.left);
            Cell rightCell = _dungeonManager.GetCell(_currentCoords + Vector2Int.right);
        }
    }
}