using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class TurnEvaluateAI
    {
        public TurnEvaluateAI(IReadOnlyAdventurerContext _)
        {
            // ��������AI���g���Ă��Ȃ��̂ŃR���X�g���N�^�̈������g��Ȃ��B
        }

        public async UniTask<int> EvaluateAsync(SharedInformation information, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            // await����K�v�Ȃ����A�x���΍�ňꉞ���Ă����B
            await UniTask.Yield(cancellationToken: token);

            // ���̃X�R�A�ƃQ�[���̏�Ԃ���L���^�[������]������v�����v�g���v�����Ȃ��B
            // �Ƃ肠�����X�R�A�ɉ������^�[������Ԃ��悤�ɂ��Ă����B
            return Mathf.RoundToInt(information.Score * 10);
        }
    }
}
