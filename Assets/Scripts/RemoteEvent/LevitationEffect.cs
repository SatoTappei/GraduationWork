using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class LevitationEffect : MonoBehaviour
    {
        [SerializeField] Vector3 _startPosition;
        [SerializeField] Vector3 _goalPosition;
        [SerializeField] ParticleSystem _stampParicle;

        WaitForSeconds _waitJumpInterval;
        bool _isPlaying;

        void Start()
        {
            // 次のジャンプまでちょっと待つ。時間は適当に指定。
            _waitJumpInterval = new WaitForSeconds(0.25f);
        }

        public void Play()
        {
            if (_isPlaying) return;

            StartCoroutine(PlayAsync());
        }

        IEnumerator PlayAsync()
        {
            _isPlaying = true;
            transform.position = _startPosition;
            transform.forward = _goalPosition - _startPosition;

            yield return JumpAsync();
            yield return _waitJumpInterval;
            yield return JumpAsync();
            yield return _waitJumpInterval;
            yield return JumpAsync();
            yield return _waitJumpInterval;
            yield return FlyAsync();
            yield return MoveAsync();

            _isPlaying = false;
            transform.position = _goalPosition;

            gameObject.SetActive(false);
        }

        IEnumerator JumpAsync()
        {
            Vector3 start = transform.position;
            Vector3 goal = start + transform.forward;
            for (float t = 0; t <= 1.0f; t += Time.deltaTime)
            {
                float x = Mathf.Lerp(start.x, goal.x, t);
                float y = Mathf.Lerp(start.y, goal.y, Easing(t)) + Mathf.Sin(Mathf.PI * Easing(t));
                float z = Mathf.Lerp(start.z, goal.z, t);
                transform.position = new Vector3(x, y, z);
                yield return null;
            }

            _stampParicle.Play();
        }

        IEnumerator FlyAsync()
        {
            Vector3 start = transform.position;
            Vector3 goal = start + transform.forward;
            for (float t = 0; t <= 0.5f; t += Time.deltaTime)
            {
                float x = Mathf.Lerp(start.x, goal.x, t);
                float y = Mathf.Lerp(start.y, goal.y, Easing(t)) + Mathf.Sin(Mathf.PI * Easing(t));
                float z = Mathf.Lerp(start.z, goal.z, t);
                transform.position = new Vector3(x, y, z);
                yield return null;
            }
        }

        IEnumerator MoveAsync()
        {
            const float Speed = 0.1f;

            Vector3 start = transform.position;
            Vector3 goal = new Vector3(_goalPosition.x, start.y, _goalPosition.z);
            for (float t = 0; t <= 1.0f; t += Time.deltaTime * Speed)
            {
                transform.position = Vector3.Lerp(start, goal, t);
                yield return null;
            }

            transform.position = goal;
        }

        float Easing(float t)
        {
            return t;
        }
    }
}
