using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.ItemData;
using VTNConnect;

namespace Game
{
    public class Artifact : DungeonEntity, IScavengeable, IVantanConnectEventReceiver
    {
        [SerializeField] Transform _fbx;
        [SerializeField] Renderer _renderer;
        [SerializeField] ParticleSystem[] _particles;

        CameraManager _camera;
        Coroutine _rotate;
        bool _isEmpty;
        bool _isActive;

        public bool IsEmpty => _isEmpty;
        public bool IsActive => _isActive;

        void Awake()
        {
            _camera = CameraManager.Find();

            // �����𖞂������Ƃŏo������̂ŁA������Ԃł͓���ł��Ȃ��悤�t���O�𗧂ĂĂ����B
            _isEmpty = true;

            // �C�x���g����M���邽�߂ɕK�v�B
            VantanConnect.RegisterEventReceiver(this);
            _isActive = true;
        }

        void Start()
        {
            // �����𖞂����܂ŉ�ʂɉf��Ȃ��悤�ɂ��Ă����B
            _renderer.enabled = false;
            foreach (ParticleSystem p in _particles)
            {
                p.gameObject.SetActive(false);
            }
        }

        public void OnEventCall(EventData data)
        {
            Debug.Log($"WebSocket Event Received: {data.EventCode}");

            if (EventDefine.Artifact01 <= data.EventCode && data.EventCode <= EventDefine.Artifact03)
            {
                Refill();
            }
        }

        public string Scavenge(Actor _, out Item item)
        {
            if (IsEmpty)
            {
                item = null;
                return "Empty";
            }

            _isEmpty = true;

            // �����𖞂����܂ŉ�ʂɉf��Ȃ��悤�ɂ��Ă����B
            _renderer.enabled = false;
            foreach (ParticleSystem p in _particles)
            {
                p.gameObject.SetActive(false);
            }

            // ���f������]������B
            if (_rotate != null) StopCoroutine(_rotate);

            item = new ItemData.Artifact();
            return "Get";
        }

        void Refill()
        {
            if (!IsEmpty) return;

            // ��ʂɕ\��������B
            _renderer.enabled = true;
            foreach (ParticleSystem p in _particles)
            {
                p.gameObject.SetActive(true);
            }

            // ���f������]������B
            _rotate = StartCoroutine(RotateAsync());

            // �o�����o�B
            _camera.Shake();
            GameLog.Add("�V�X�e��", "�A�[�e�B�t�@�N�g���o���I", LogColor.Yellow);

            _isEmpty = false;
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
