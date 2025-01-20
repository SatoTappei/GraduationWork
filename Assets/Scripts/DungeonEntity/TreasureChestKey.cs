using Game.ItemData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class TreasureChestKey : DungeonEntity, IScavengeable
    {
        // ����A�y���� �� �d���� �͌����ڈȊO�ɈႢ�������̂ŗ񋓌^�ł̔���ŏ\���B
        enum Type { Light, Heavy }

        [SerializeField] Transform _fbx;
        [SerializeField] ParticleSystem _particle;
        [SerializeField] Type _type;

        bool _isUsed;
        float _elapsed;

        void Start()
        {
            StartCoroutine(RotateAsync());
            _particle.Play();
        }

        void Update()
        {
            // ��ʂɑ�ʂɑ��݂���󋵂�h�����߁A��莞�Ԍo�߂Ŏg�p�ς݃t���O�𗧂āA�폜����B
            _elapsed += Time.deltaTime;
            if (_elapsed > 10.0f) _isUsed = true; // �K���Ɏ��Ԃ��w��B

            // ���Ԍo�߂������͎擾����āA�g�p�ς݃t���O���������ꍇ�͍폜�����B
            if (_isUsed)
            {
                DungeonManager.RemoveActor(Coords, this);
                Destroy(gameObject);
            }
        }

        public string Scavenge(Actor user, out Item item)
        {
            if (_isUsed)
            {
                item = null;
                return "Empty";
            }

            _isUsed = true;

            if (_type == Type.Light)
            {
                item = new LightKey();
            }
            else
            {
                item = new HeavyKey();
            }

            return "Get";
        }

        IEnumerator RotateAsync()
        {
            Vector3 basePosition = _fbx.localPosition;
            while (true)
            {
                // �A�C�e������]�����V���Ă��銴���̓����B�l�͓K���B
                _fbx.Rotate(Vector3.up * Time.deltaTime * 20.0f);
                _fbx.localPosition = basePosition + Vector3.up * Mathf.Sin(Time.time) * 0.2f;

                yield return null;
            }
        }
    }
}
