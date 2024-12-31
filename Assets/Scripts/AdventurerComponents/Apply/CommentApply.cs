using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class CommentApply : MonoBehaviour
    {
        // �X�v���b�h�V�[�g����R�����g��ǂݍ��ޏ����͔񓯊��Ŏ��s����Ă���B
        // ���̂��߁A�ǂݍ��݊�������ɂ��̃��\�b�h���Ăяo���ƁA�R�����g������Ȃ��̂Œ��ӁB
        public void Reaction()
        {
            ReactionAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        async UniTask ReactionAsync(CancellationToken token)
        {
            if (!CommentDisplayer.TryFind(out CommentDisplayer commentDisplayer)) return;

            // ���g�ւ̃R�����g����ʂɗ����B
            TryGetComponent(out Blackboard blackboard);
            IReadOnlyCollection<CommentSpreadSheetData> comment = commentDisplayer.Display(blackboard.FullName);

            // ���g�ւ̃R�����g�������ꍇ�B
            if (comment == null || comment.Count == 0) return;

            //if (!TryGetComponent(out CommentReactionAI ai)) return;

            // ���g�ւ̃R�����g�ɑ΂���䎌�ƁA���̃R�����g���󂯂Ă̐S��̕ω���AI�ɏo�͂�����B
            //CommentReactionAI.Response response = await ai.RequestReactionAsync(comment, token);

            float score = 1; // �R�����g�̎d�l��������܂ŉ��̒l�B

            // AI�����ɖ\�����排����ɑ΂���-1�΂���o�͂��邱�Ƃ�z�肵�āA�S��ւ̉e���͂قǂقǂɂ��Ă����B
            // �Ƃ肠����20%�㉺����悤�ɂ��Ă����B�K�v�ɉ����Ē����B
            float add = (blackboard.MaxEmotion / 100.0f) * (20.0f * score);
            blackboard.CurrentEmotion += Mathf.CeilToInt(add);

            // UI�ɔ��f�B
            if (TryGetComponent(out StatusBarApply statusBar)) statusBar.Apply();
        }
    }
}