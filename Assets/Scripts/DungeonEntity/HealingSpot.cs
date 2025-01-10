using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class HealingSpot : DungeonEntity, IFootTriggerable
    {
        [SerializeField] Transform _fbx;
        [SerializeField] Renderer _renderer;
        [SerializeField] ParticleSystem _particle;

        WaitForSeconds _waitRefill;

        void Start()
        {
            StartCoroutine(RotateAsync());
        }

        public override void Interact(Actor user)
        {
            StartCoroutine(InteractAsync(user));
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

        IEnumerator InteractAsync(Actor user)
        {
            if (user != null && user.TryGetComponent(out HealReceiver heal))
            {
                heal.Heal(33, default); // 効果量は適当。
            }

            _renderer.enabled = false;
            _particle.gameObject.SetActive(false);
            DungeonManager.RemoveActor(Coords, this);

            yield return _waitRefill ??= new WaitForSeconds(30.0f); // 時間は適当に指定。

            _renderer.enabled = true;
            _particle.gameObject.SetActive(true);
            DungeonManager.AddActor(Coords, this);
        }
    }
}
