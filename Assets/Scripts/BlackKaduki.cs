using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    // �`���҂ƃZ�����d�Ȃ�Ȃ��悤�ɂ��Ȃ��Ƃ����Ȃ��B
    //  �L�����N�^�[�͈ړ��O�Ɉړ��悪�\��ς݂��ǂ����`�F�b�N����悤�ȃ��W�b�N�ɂ��邱�Ƃŏd�Ȃ��h���H
    //   ��1�}�X�̒ʘH���ƁA���݂����������ꂸ�ς�ł��܂��B���[�O���C�N�Ȃ炱�̏󋵂��v���C���[���ǂ��炩���E�����ƂőŔj�ł��邪�c
    // �����ʒu���瓮���Ȃ���Ζ��͉������邪�c
    // �����ʒu�𒆐S��3*3�͈͓̔������낤�낷��悤�ɂ��A�`���҂��x���͈͂ɓ������ꍇ�A�����ʒu�ɖ߂��Čx������Ƃ��H
    // �^�[�����ɂ���H
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

                await UniTask.Yield(); // �������[�v�h�~�B
            }
        }

        // ���g�̏㉺���E�̍��W�ɂ���`���҂�Ԃ��B
        Adventure GetNeighbourAdventure()
        {
            Adventure target = null;
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

        // ���g�̏㉺���E�̍��W�ɂ��鉽�ꂩ�̖`���҂��U���B
        async UniTask AttackAsync()
        {
            const float AnimationLength = 1.1f;

            Adventure target = GetNeighbourAdventure();

            // �ڕW�������B
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

            target.Damage(ID, "�p���`", 11, Coords);

            _animator.Play("Attack");
            await UniTask.WaitForSeconds(AnimationLength);
        }

        // �z�u���W�𒆐S�Ƃ���3*3�͈͓̔��̉��ꂩ�̍��W�Ɉړ��B
        async UniTask WalkAsync()
        {
            Cell cell = GetSpawnCoordsRandomAroundCell();

            // �ړ��\��̃Z���Ɏ��g��o�^�B
            _dungeonManager.RemoveActorOnCell(_currentCoords, this);
            _currentCoords = cell.Coords;
            _dungeonManager.AddActorOnCell(_currentCoords, this);

            _animator.Play("Walk");

            // �ړ��B
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

        // ���݂̃Z������㉺���E�ړ��ňړ��ł��郉���_���ȃZ����Ԃ��B
        Cell GetSpawnCoordsRandomAroundCell()
        {
            List<Vector2Int> choices = GetPlaceCoordsAroundCoords().Where(v => 
            {
                return Vector2Int.Distance(v, _currentCoords) < 1.4f;
            }).ToList();

            Vector2Int coords = choices[Random.Range(0, choices.Count)];
            return _dungeonManager.GetCell(coords);
        }

        // �אڂ�����W�݂̂ł͂Ȃ��A�z�u���W���܂߂�B
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

        // ���S���Ă���ꍇ�͉��o���Đ���true��Ԃ��B�����Ă���ꍇ�͉�������false��Ԃ��B
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
    }
}
