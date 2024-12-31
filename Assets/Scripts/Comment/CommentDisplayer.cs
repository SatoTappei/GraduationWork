using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CommentDisplayer : MonoBehaviour
    {
        CommentSpreadSheetLoader _commentLoader;

        // �Q�[���J�n���A�f�[�^�x�[�X����񓯊��ŃR�����g��ǂݍ���ł���B
        public bool IsReady => !_commentLoader.IsLoading;

        void Awake()
        {
            _commentLoader = GetComponent<CommentSpreadSheetLoader>();
        }

        public static bool TryFind(out CommentDisplayer commentDisplayer)
        {
            commentDisplayer = GameObject.FindGameObjectWithTag("CommentDisplayer").GetComponent<CommentDisplayer>();
            return commentDisplayer != null;
        }

        // �`���҂̖��O�ɑΉ������R�����g�𗬂��B
        public IReadOnlyCollection<CommentSpreadSheetData> Display(string fullName)
        {
            // �R�����g�̃��[�h���I����Ă��Ȃ��ꍇ�B
            if (_commentLoader.IsLoading) return null;

            // �`���҂ւ̃R�����g�������ꍇ�B
            if (!_commentLoader.TryGetComment(fullName, out IReadOnlyCollection<CommentSpreadSheetData> comment))
            {
                return null;
            }
            
            // ���O�ɒǉ��B
            foreach (CommentSpreadSheetData data in comment)
            {
                GameLog.Add($"{data.Name}�ւ̃R�����g", data.Comment, GameLogColor.Green);
            }

            return comment;
        }
    }
}
