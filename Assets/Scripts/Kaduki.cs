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

        Vector2Int _currentCoords;
        Vector2Int _currentDirection;
        string _pathTarget;
        int _currentPathIndex;
        List<Cell> _path;
        int _statusBarID;
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
            _uiManager.ShowLine(_statusBarID, "こんにちは。");
            _uiManager.AddLog("Kadukiがダンジョンにやってきた。");

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
                ElapsedTurn++;

                string selected = await _adventurerAI.SelectNextActionAsync();
                if (selected == "Move North") await MoveAsync(Vector2Int.up);
                else if (selected == "Move South") await MoveAsync(Vector2Int.down);
                else if (selected == "Move East") await MoveAsync(Vector2Int.right);
                else if (selected == "Move West") await MoveAsync(Vector2Int.left);
                else if (selected == "Return To Entrance") await MoveAsync("Entrance");
                else if (selected == "Attack Surrounding") await AttackAsync();
                else if (selected == "Scavenge Surrounding") await ScavAsync();
                else if (selected == "Talk Surrounding") await TalkAsync();

                if (await DeathAsync() || await EscapeAsync()) break;

                await UniTask.Yield();
            }

            Destroy(gameObject);
        }

        // 隣のセルに移動。
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
                // 経路探索が出来ないので直接更新。移動せず向きだけ変えるために移動処理を行う。
                _path.Clear();
                _path.Add(targetCell);
                _pathTarget = $"{direction}(IsImpassable)";
            }

            _currentPathIndex = 0;

            await MoveNextCellAsync();
        }

        // 経路に沿って移動。
        async UniTask MoveAsync(string target)
        {
            // 現在の経路と違う目標を選択した場合は再度経路探索。
            if (_pathTarget != target)
            {
                if (target == "Treasure") PathfindingToTreasure();
                else if (target == "Enemy") PathfindingToEnemy();
                else if (target == "Entrance") PathfindingToEntrance();
                else Debug.LogWarning($"対応する目標が存在しないため経路探索が出来ない。: {target}");

                _pathTarget = target;
                _currentPathIndex = 0;
            }

            await MoveNextCellAsync();
        }

        // とりあえず、経路探索するごとにランダムな宝箱を選ぶようにしておく。
        // 後々、行動中断後、再度探索する際に同じ宝箱を選ぶような処理にしたい。
        void PathfindingToTreasure()
        {
            List<Cell> targetCells = _dungeonManager.GetCells("Treasure").ToList();
            int i = Random.Range(0,targetCells.Count);
            Actor treasure = targetCells[i].GetActors().Where(a => a.ID == "Treasure").First();

            // 宝箱のマスへは経路探索が出来ないので、正面の位置までの経路探索。
            Vector2Int goalCoords = treasure.Coords;
            if (treasure.Direction == Vector2Int.up) goalCoords += Vector2Int.up;
            else if (treasure.Direction == Vector2Int.down) goalCoords += Vector2Int.down;
            else if (treasure.Direction == Vector2Int.left) goalCoords += Vector2Int.left;
            else if (treasure.Direction == Vector2Int.right) goalCoords += Vector2Int.right;

            _dungeonManager.Pathfinding(_currentCoords, goalCoords, _path);
        }

        // とりあえず、経路探索するごとにランダムな敵を選ぶようにしておく。
        // 後々、行動中断後、再度探索する際に同じ敵を選ぶような処理にしたい。
        void PathfindingToEnemy()
        {
            List<Cell> targetCells = _dungeonManager.GetCells("BlackKaduki").ToList();
            int i = Random.Range(0, targetCells.Count);
            Actor enemy = targetCells[i].GetActors().Where(a => a.ID == "BlackKaduki").First();

            // 敵のマスへは経路探索が出来ないので、周囲の位置までの経路探索。
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

        // 入口への経路探索。
        void PathfindingToEntrance()
        {
            List<Cell> targetCells = _dungeonManager.GetCells("Entrance").ToList();
            int i = Random.Range(0, targetCells.Count);
            Actor entrance = targetCells[i].GetActors().Where(a => a.ID == "Entrance").First();

            _dungeonManager.Pathfinding(_currentCoords, entrance.Coords, _path);
        }

        // 次のセルに移動。
        async UniTask MoveNextCellAsync()
        {
            if (_path.Count == 0) return;

            // 自身の向いている方向更新。
            _currentDirection = _path[_currentPathIndex].Coords - Coords;

            // 目の前のセルの文字を確認してドアかチェックして開ける。
            Vector2Int frontCoords = _currentCoords + _currentDirection;
            if ("2468".Contains(Blueprint.Doors[frontCoords.y][frontCoords.x]))
            {
                Actor actor = _dungeonManager.GetActorsOnCell(frontCoords).Where(c => c.ID == "Door").FirstOrDefault();
                if (actor != null && actor is DungeonEntity door)
                {
                    door.Interact(actor);
                }
            }

            if (_path[_currentPathIndex].IsPassable())
            {
                // 目の前のセルに自身を登録。他のキャラクターとの交差は許容するので回避セルには登録しない。
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

                _adventurerAI.ReportActionResult($"turn{ElapsedTurn}: move from {_currentCoords} to {frontCoords} success.");
                _adventurerAI.ReportExploredCell(_currentCoords);
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

                _adventurerAI.ReportActionResult($"turn{ElapsedTurn}: move from {_currentCoords} to {frontCoords} failure.");
            }
        }

        // 周囲のActorに攻撃する。
        async UniTask AttackAsync()
        {
            // 周囲のセルから目標を選ぶ。
            IDamageable targetDamageable = null;
            Actor targetActor = null;
            for (int i = -1; i <= 1; i++)
            {
                for (int k = -1; k <= 1; k++)
                {
                    // 上下左右の4方向のみ。
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
                _adventurerAI.ReportActionResult($"turn{ElapsedTurn}: attack neighbour target failure.");
                return;
            }

            // 目標を向く。
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
            _uiManager.ShowLine(_statusBarID, "攻撃する。");

            string result = targetDamageable.Damage(ID, "パンチ", 34, _currentCoords);
            if (result == "Defeated")
            {
                _uiManager.ShowLine(_statusBarID, "殺した。");
                _uiManager.AddLog("KadukiがBlackKadukiを殺した。");
                DefeatCount++;
                _adventurerAI.ReportActionResult($"turn{ElapsedTurn}: we attacked the enemy. And defeated them.");
            }

            _adventurerAI.ReportActionResult($"turn{ElapsedTurn}: we attacked the enemy. The enemy is still alive.");
        }

        // 周囲のActorを漁る。
        async UniTask ScavAsync()
        {
            // 周囲のセルから目標を選ぶ。
            IScavengeable targetScavengeable = null;
            Actor targetActor = null;
            for (int i = -1; i <= 1; i++)
            {
                for (int k = -1; k <= 1; k++)
                {
                    // 上下左右の4方向のみ。
                    if ((i == 0 && k == 0) || Mathf.Abs(i * k) > 0) continue;

                    Vector2Int neighbourCoords = new Vector2Int(_currentCoords.x + k, _currentCoords.y + i);
                    Cell cell = _dungeonManager.GetCell(neighbourCoords);

                    foreach (Actor actor in cell.GetActors())
                    {
                        // 宝箱を優先して漁るようにしたいので、見つけた時点でループから抜ける。
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
                _adventurerAI.ReportActionResult($"turn{ElapsedTurn}: scavenge neighbour target failure.");
                return;
            }
            
            // 目標を向く。
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
                _uiManager.ShowLine(_statusBarID, "宝を入手。");
            }

            _adventurerAI.ReportActionResult($"turn{ElapsedTurn}: scavenge neighbour target success.");
        }

        // 周囲のAdventureと会話する。
        async UniTask TalkAsync()
        {
            //
        }

        // 死亡している場合は演出を再生しtrueを返す。生きている場合は何もせずfalseを返す。
        async UniTask<bool> DeathAsync()
        {
            const float AnimationLength = 2.5f;

            if (CurrentHp > 0) return false;

            _animator.Play("Death");
            _audioSource.clip = _deathSE;
            _audioSource.Play();
            _uiManager.ShowLine(_statusBarID, "死亡した。");

            await UniTask.WaitForSeconds(AnimationLength);
            
            _dungeonManager.RemoveActorOnCell(_currentCoords, this);

            return true;
        }

        // ダメージを受ける。
        public override string Damage(string id, string weapon, int value, Vector2Int coords)
        {
            if (CurrentHp <= 0) return "Corpse";

            CurrentHp -= value;
            CurrentHp = Mathf.Max(0, CurrentHp);
            _uiManager.UpdateStatusBarStatus(_statusBarID, this);
            _uiManager.ShowLine(_statusBarID, "ダメージを受けた。");

            if (!_isKnockback) StartCoroutine(HitEffectAsync(coords));

            if (CurrentHp <= 0) return "Defeated";
            else return "Hit";
        }

        // 攻撃が自身にヒットした演出。
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

        // ノックバック。
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

        // 脱出条件を満たしている場合は演出を再生しtrueを返す。それ以外の場合は何もせずfalseを返す。
        async UniTask<bool> EscapeAsync()
        {
            const float AnimationLength = 1.0f * 2;

            if (TreasureCount == 0 && DefeatCount == 0) return false;
            else if (Blueprint.Interaction[_currentCoords.y][_currentCoords.x] != '<') return false;

            _animator.Play("Jump");
            _uiManager.ShowLine(_statusBarID, "目的を達成して脱出。");
            _uiManager.AddLog("Kadukiがダンジョンから脱出した。");

            await UniTask.WaitForSeconds(AnimationLength);

            _dungeonManager.RemoveActorOnCell(_currentCoords, this);

            return true;
        }
    }
}