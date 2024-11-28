using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    // �ړ���U���ȂǁA�L�����N�^�[�̊e�s�����s���N���X�͂��̃N���X���p������B
    // �ǂ̍s���ɂ����ʂ��Ďg�p�o����"�ʒu�̈ړ�"��"��]"�̏��������B
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
