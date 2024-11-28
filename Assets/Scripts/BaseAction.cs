using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    // 移動や攻撃など、キャラクターの各行動を行うクラスはこのクラスを継承する。
    // どの行動にも共通して使用出来る"位置の移動"と"回転"の処理を持つ。
    public class BaseAction : MonoBehaviour
    {
        Transform _transform;
        Transform _forwardAxis;

        protected async UniTask TranslateAsync(float speed, Vector3 targetPosition, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            _transform ??= transform;
            Vector3 start = _transform.position;
            Vector3 goal = targetPosition;
            for (float t = 0; t <= 1; t += Time.deltaTime * speed)
            {
                _transform.position = Vector3.Lerp(start, goal, t);
                await UniTask.Yield(cancellationToken: token);
            }

            _transform.position = goal;
        }

        protected async UniTask RotateAsync(float speed, Vector3 targetPosition, CancellationToken token)
        {
            Quaternion rot = CalculateRotation(transform.position, targetPosition);
            await RotateAsync(speed, rot, token);
        }
 
        protected async UniTask RotateAsync(float speed, Quaternion targetRotation, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            _forwardAxis ??= transform.FindChildRecursive("ForwardAxis");
            Quaternion start = _forwardAxis.rotation;
            Quaternion goal = targetRotation;
            for (float t = 0; t <= 1; t += Time.deltaTime * speed)
            {
                _forwardAxis.rotation = Quaternion.Lerp(start, goal, t);
                await UniTask.Yield(cancellationToken: token);
            }

            _forwardAxis.rotation = goal;
        }

        static Quaternion CalculateRotation(Vector3 a, Vector3 b)
        {
            Vector3 dir = (b - a).normalized;
            if (dir == Vector3.zero) return Quaternion.identity;
            else return Quaternion.LookRotation(dir);
        }
    }
}
