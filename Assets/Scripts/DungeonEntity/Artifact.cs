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

            // 条件を満たすことで出現するので、初期状態では入手できないようフラグを立てておく。
            _isEmpty = true;

            // イベントを受信するために必要。
            VantanConnect.RegisterEventReceiver(this);
            _isActive = true;
        }

        void Start()
        {
            // 条件を満たすまで画面に映らないようにしておく。
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

            // 条件を満たすまで画面に映らないようにしておく。
            _renderer.enabled = false;
            foreach (ParticleSystem p in _particles)
            {
                p.gameObject.SetActive(false);
            }

            // モデルを回転させる。
            if (_rotate != null) StopCoroutine(_rotate);

            item = new ItemData.Artifact();
            return "Get";
        }

        void Refill()
        {
            if (!IsEmpty) return;

            // 画面に表示させる。
            _renderer.enabled = true;
            foreach (ParticleSystem p in _particles)
            {
                p.gameObject.SetActive(true);
            }

            // モデルを回転させる。
            _rotate = StartCoroutine(RotateAsync());

            // 出現演出。
            _camera.Shake();
            GameLog.Add("システム", "アーティファクトが出現！", LogColor.Yellow);

            _isEmpty = false;
        }

        IEnumerator RotateAsync()
        {
            Vector3 basePosition = _fbx.localPosition;
            while (true)
            {
                // アイテムが回転しつつ浮遊している感じの動き。値は適当。
                _fbx.Rotate(Vector3.up * Time.deltaTime * 20.0f);
                _fbx.localPosition = basePosition + Vector3.up * Mathf.Sin(Time.time) * 0.2f;

                yield return null;
            }
        }
    }
}
