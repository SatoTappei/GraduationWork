using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class HelicopterCrashEffect : MonoBehaviour
    {
        [SerializeField] Transform _helicopterParent;
        [SerializeField] Transform _helicopterChildren;
        [SerializeField] Transform _mainRotor;
        [SerializeField] Transform _tailRotor;
        [SerializeField] Transform _missileParent;
        [SerializeField] Transform _missileChildren;
        [SerializeField] ParticleSystem _helicopterExplosionParticle;
        [SerializeField] ParticleSystem _missileTrailParticle;
        [SerializeField] ParticleSystem _missileExplosionParticle;
        [SerializeField] MeshRenderer[] _helicopterRenderers;
        [SerializeField] MeshRenderer _missileRenderer;
        [SerializeField] AudioClip _helicopterSE;
        [SerializeField] AudioClip _helicopterExplosionSE;
        [SerializeField] AudioClip _missileExplosionSE;
        [SerializeField] AudioSource _helicopterAudioSource;
        [SerializeField] AudioSource _missileAudioSource;

        WaitForSeconds _waitHelicopterExplosion;

        public void Play(Vector3 position, Adventurer target)
        {
            transform.position = position;

            StartCoroutine(PlayAsync(target.Coords));
            StartCoroutine(RotorAnimationAsync());
        }

        IEnumerator PlayAsync(Vector2Int targetCoords)
        {
            foreach (MeshRenderer r in _helicopterRenderers) r.enabled = true;
            _missileRenderer.enabled = true;

            // ���o�̒��S�ʒu���炠����x���ꂽ�ʒu�Ƀw���R�v�^�[�ƃ~�T�C����z�u�B
            _helicopterParent.position = GetRandomPositionAround(transform.position);
            _missileParent.position = GetRandomPositionAround(transform.position);

            _helicopterAudioSource.clip = _helicopterSE;
            _helicopterAudioSource.Play();
            _missileTrailParticle.Play();

            // �w���R�v�^�[�ƃ~�T�C�����Փ˂�����B
            yield return CrossAsync();

            _missileRenderer.enabled = false;
            _missileTrailParticle.Stop();
            _missileExplosionParticle.Play();
            _missileAudioSource.clip = _missileExplosionSE;
            _missileAudioSource.Play();

            // ��]���Ȃ���w���R�v�^�[�ė��B
            StartCoroutine(FallingRotateAsync());
            yield return FallingTranslateAsync();

            _helicopterExplosionParticle.Play();
            foreach (MeshRenderer r in _helicopterRenderers) r.enabled = false;

            _helicopterAudioSource.clip = _helicopterExplosionSE;
            _helicopterAudioSource.Play();

            // �w���R�v�^�[���������鉉�o��҂B���Ԃ͓K���Ɏw��B
            yield return _waitHelicopterExplosion ??= new WaitForSeconds(1.5f);

            // �C�x���g���o���Ԃ������̂ŁA�ڕW���C�x���g�����ʒu���痣��Ă���\��������B
            // �Ȃ̂ŁA���ۂɃC�x���g�����ʒu�ɖ`���҂����邩�`�F�b�N�B
            foreach (Actor actor in DungeonManager.GetActors(targetCoords))
            {
                if (actor is Adventurer target)
                {
                    target.Damage(33, targetCoords); // �_���[�W�ʂ͓K���B
                }
            }

            // �v�[���ɖ߂��B
            gameObject.SetActive(false);
        }

        IEnumerator RotorAnimationAsync()
        {
            while (true)
            {
                _mainRotor.Rotate(Vector3.up * Time.deltaTime * 1000.0f);
                yield return null;
            }
        }

        static Vector3 GetRandomPositionAround(Vector3 center)
        {
            const float Distance = 10.0f;

            float r = Random.Range(0f, Mathf.PI * 2.0f);
            float sin = Mathf.Sin(r);
            float cos = Mathf.Cos(r);
            return center + new Vector3(sin * Distance, 0, cos * Distance);
        }

        IEnumerator CrossAsync()
        {
            const float Speed = 0.5f;

            Vector3 helicopterStart = _helicopterParent.position;
            Vector3 missileStart = _missileParent.position;
            Vector3 goal = transform.position;

            // �i�s�����������B
            _helicopterChildren.forward = goal - helicopterStart;
            _missileChildren.forward = goal - missileStart;

            for (float t = 0; t <= 1.0f; t += Time.deltaTime * Speed)
            {
                _helicopterParent.position = Vector3.Lerp(helicopterStart, goal, t);
                _missileParent.position = Vector3.Lerp(missileStart, goal, t);

                yield return null;
            }

            _helicopterParent.position = goal;
            _missileParent.position = goal;
        }

        IEnumerator FallingTranslateAsync()
        {
            const float ForwardDistance = 3.0f;

            // �u���v�̎���`���悤�ɒė�����B
            Vector3 a = _helicopterParent.position;

            Vector3 forward = _helicopterChildren.forward * ForwardDistance;
            forward.y = 0;
            Vector3 b = new Vector3(a.x, a.y / 2, a.z) + forward;

            Vector3 c = new Vector3(a.x, 0, a.z);

            yield return FallingTranslateAsync(a, b);
            yield return FallingTranslateAsync(b, c);
        }

        IEnumerator FallingTranslateAsync(Vector3 from, Vector3 to)
        {
            for (float t = 0; t <= 1.0f; t += Time.deltaTime)
            {
                float x = Mathf.Lerp(from.x, to.x, t);
                float y = Mathf.Lerp(from.y, to.y, Easing(t));
                float z = Mathf.Lerp(from.z, to.z, t);
                _helicopterParent.position = new Vector3(x, y, z);

                yield return null;
            }

            _helicopterParent.position = to;
        }

        static float Easing(float t)
        {
            return t * t * t;
        }

        IEnumerator FallingRotateAsync()
        {
            while (true)
            {
                _helicopterChildren.Rotate(Vector3.up * Time.deltaTime * 360.0f);
                yield return null;
            }
        }
    }
}
