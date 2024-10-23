using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    // 冒険者とセルが重ならないようにしないといけない。
    //  キャラクターは移動前に移動先が予約済みかどうかチェックするようなロジックにすることで重なりを防ぐ？
    //   幅1マスの通路だと、お互いがを避けられず積んでしまう。ローグライクならこの状況をプレイヤーがどちらかを殺すことで打破できるが…
    // 生成位置から動かなければ問題は解決するが…
    // 生成位置を中心に3*3の範囲内をうろうろするようにし、冒険者が警戒範囲に入った場合、生成位置に戻って警戒するとか？
    // ターン制にする？
    public class BlackKaduki : Enemy
    {
        [SerializeField] AudioClip _punchHitSE;
        [SerializeField] AudioClip _deathSE;

        DungeonManager _dungeonManager;
        UiManager _uiManager;
        Animator _animator;
        AudioSource _audioSource;

        Vector2Int _placeCoords;
        Vector2Int _currentCoords;
        Vector2Int _currentDirection;
        List<Cell> _path;
        bool _isKnockback;
        int _currentHp;

        public override Vector2Int Coords => _currentCoords;
        public override Vector2Int Direction => _currentDirection;

        void Awake()
        {
            _dungeonManager = DungeonManager.Find();
            _uiManager = UiManager.Find();
            _animator = GetComponentInChildren<Animator>();
            _audioSource = GetComponent<AudioSource>();
            _path = new List<Cell>();
            _currentHp = 100;
        }

        void Start()
        {
            UpdateAsync().Forget();
        }

        public override void Place(Vector2Int coords)
        {
            _dungeonManager.RemoveActorOnCell(_currentCoords, this);
            
            _placeCoords = coords;
            _currentCoords = coords;
            _currentDirection = Vector2Int.up;
            
            _dungeonManager.AddActorOnCell(_currentCoords, this);
            
            Cell cell = _dungeonManager.GetCell(_currentCoords);
            transform.position = cell.Position;
        }

        async UniTask UpdateAsync()
        {
            while (true)
            {
                if (GetNeighbourAdventure() == null)
                {
                    await WalkAsync();
                }
                else
                {
                    await AttackAsync();
                }

                if (await DeathAsync()) break;

                await UniTask.Yield(); // 無限ループ防止。
            }
        }

        // 自身の上下左右の座標にいる冒険者を返す。
        Adventure GetNeighbourAdventure()
        {
            Adventure target = null;
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
                        if (actor is Adventure adventure)
                        {
                            target = adventure;
                            break;
                        }
                    }

                    if (target != null) break;
                }

                if (target != null) break;
            }

            return target;
        }

        // 自身の上下左右の座標にいる何れかの冒険者を攻撃。
        async UniTask AttackAsync()
        {
            const float AnimationLength = 1.1f;

            Adventure target = GetNeighbourAdventure();

            // 目標を向く。
            Vector3 position = _dungeonManager.GetCell(_currentCoords).Position;
            Vector3 targetPosition = _dungeonManager.GetCell(target.Coords).Position;
            Transform axis = transform.Find("ForwardAxis");
            Vector3 goalDirection = (targetPosition - position).normalized;
            Quaternion startRotation = axis.rotation;
            Quaternion goalRotation = Quaternion.LookRotation(goalDirection);
            for (float t = 0; t <= 1; t += Time.deltaTime * 1.0f)
            {
                axis.rotation = Quaternion.Lerp(startRotation, goalRotation, t * 4);

                await UniTask.Yield();
            }

            target.Damage(ID, "パンチ", 11, Coords);

            _animator.Play("Attack");
            await UniTask.WaitForSeconds(AnimationLength);
        }

        // 配置座標を中心とした3*3の範囲内の何れかの座標に移動。
        async UniTask WalkAsync()
        {
            Cell cell = GetSpawnCoordsRandomAroundCell();

            // 移動予定のセルに自身を登録。
            _dungeonManager.RemoveActorOnCell(_currentCoords, this);
            _currentCoords = cell.Coords;
            _dungeonManager.AddActorOnCell(_currentCoords, this);

            _animator.Play("Walk");

            // 移動。
            Vector3 startPosition = transform.position;
            Vector3 goalPosition = cell.Position;
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
        }

        // 現在のセルから上下左右移動で移動できるランダムなセルを返す。
        Cell GetSpawnCoordsRandomAroundCell()
        {
            List<Vector2Int> choices = GetPlaceCoordsAroundCoords().Where(v => 
            {
                return Vector2Int.Distance(v, _currentCoords) < 1.4f;
            }).ToList();

            Vector2Int coords = choices[Random.Range(0, choices.Count)];
            return _dungeonManager.GetCell(coords);
        }

        // 隣接する座標のみではなく、配置座標も含める。
        IEnumerable<Vector2Int> GetPlaceCoordsAroundCoords()
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int k = -1; k <= 1; k++)
                {
                    int nx = _placeCoords.x + k;
                    int ny = _placeCoords.y + i;

                    if (Blueprint.Base[ny][nx] == '_')
                    {
                        yield return new Vector2Int(nx, ny);
                    }
                }
            }
        }

        // 死亡している場合は演出を再生しtrueを返す。生きている場合は何もせずfalseを返す。
        async UniTask<bool> DeathAsync()
        {
            const float AnimationLength = 2.5f;

            if (_currentHp > 0) return false;

            _animator.Play("Death");
            _audioSource.clip = _deathSE;
            _audioSource.Play();

            await UniTask.WaitForSeconds(AnimationLength);

            _dungeonManager.RemoveActorOnCell(_currentCoords, this);

            return true;
        }

        public override string Damage(string id, string weapon, int value, Vector2Int coords)
        {
            if (_currentHp <= 0) return "Corpse";

            _currentHp -= value;
            _currentHp = Mathf.Max(0, _currentHp);

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
    }
}
