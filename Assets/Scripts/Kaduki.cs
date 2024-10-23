using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Game
{
    // 何かしらイベントが起きた場合はAIが次の行動を判断する。(移動中に敵に殴られたなど)
    // 何もない場合はその行動を継続する。(目標に向けて移動するなど)
    //  目標に向けて移動する。
    //   宝箱、敵、曲がり角、入口。
    //    経路探索->向く->移動。
    //  周囲にインタラクト。
    //   敵に攻撃、宝箱などを調べる。他のキャラクターに話しかける。
    //  待機。
    // 結果を知る。
    // 攻撃されたり話しかけられたりなど他キャラクターからの接触。
    //  1歩ごとに中断判定(ダメージや死亡など)が必要
    // 死んだり脱出した場合はここで分岐。
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

        // 隣のセルに移動。
        async UniTask MoveAsync(string target)
        {
            PathfindingIfTargetChanged(target);
            await MoveNextCellAsync();
        }

        // 現在の経路と違う目標を選択した場合は再度経路探索。
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

        // ダンジョン内を適当にうろうろする場合、それらしい移動先が必要？
        void PathfindingToCheckPoint()
        {
            // 
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

            // 目の前のセルに自身を登録。他のキャラクターとの交差は許容するので回避セルには登録しない。
            _dungeonManager.RemoveActorOnCell(_currentCoords, this);
            _currentCoords = _path[_currentPathIndex].Coords;
            _dungeonManager.AddActorOnCell(_currentCoords, this);

            _animator.Play("Walk");

            // 移動。
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

            if (targetDamageable == null) return;

            // 目標を向く。
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
            _uiManager.ShowLine(_statusBarID, "攻撃する。");

            string result = targetDamageable.Damage(ID, "パンチ", 34, _currentCoords);
            if (result == "Defeated")
            {
                _uiManager.ShowLine(_statusBarID, "殺した。");
                _uiManager.AddLog("KadukiがBlackKadukiを殺した。");
                _defeatCount++;
            }
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

            if (targetScavengeable == null) return;
            
            // 目標を向く。
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
                _uiManager.ShowLine(_statusBarID, "宝を入手。");
            }
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

            if (_currentHp > 0) return false;

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
            if (_currentHp <= 0) return "Corpse";

            _currentHp -= value;
            _currentHp = Mathf.Max(0, _currentHp);
            _uiManager.UpdateStatusBarStatus(_statusBarID, this);
            _uiManager.ShowLine(_statusBarID, "ダメージを受けた。");

            if (!_isKnockback) StartCoroutine(HitEffectAsync(coords));

            if (_currentHp <= 0) return "Defeated";
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

            if (_treasureCount == 0 && _defeatCount == 0) return false;
            else if (Blueprint.Interaction[_currentCoords.y][_currentCoords.x] != '<') return false;

            _animator.Play("Jump");
            _uiManager.ShowLine(_statusBarID, "目的を達成して脱出。");
            _uiManager.AddLog("Kadukiがダンジョンから脱出した。");

            await UniTask.WaitForSeconds(AnimationLength);

            _dungeonManager.RemoveActorOnCell(_currentCoords, this);

            return true;
        }

        // 行動の結果をAIに報告。
        void ReportActionResult()
        {
            Cell upCell = _dungeonManager.GetCell(_currentCoords + Vector2Int.up);
            Cell downCell = _dungeonManager.GetCell(_currentCoords + Vector2Int.down);
            Cell leftCell = _dungeonManager.GetCell(_currentCoords + Vector2Int.left);
            Cell rightCell = _dungeonManager.GetCell(_currentCoords + Vector2Int.right);
        }
    }
}