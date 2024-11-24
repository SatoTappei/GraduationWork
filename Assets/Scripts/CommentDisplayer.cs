using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CommentDisplayer : MonoBehaviour
    {
        CommentSpreadSheetLoader _commentLoader;
        CommentPool _commentPool;
        CommentPlacer _commentPlacer;

        // �Q�[���J�n���A�X�v���b�h�V�[�g����񓯊��ŃR�����g��ǂݍ���ł���B
        public bool IsReady => !_commentLoader.IsLoading;

        void Awake()
        {
            _commentLoader = GetComponent<CommentSpreadSheetLoader>();
            _commentPool = GetComponent<CommentPool>();
            _commentPlacer = new CommentPlacer();
        }

        public static CommentDisplayer Find()
        {
            return GameObject.FindGameObjectWithTag("CommentDisplayer").GetComponent<CommentDisplayer>();
        }

        public static bool TryFind(out CommentDisplayer commentDisplayer)
        {
            commentDisplayer = Find();
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
            
            // �R�����g���m�����Ȃ��悤�ɉ�ʂɔz�u�B
            foreach (CommentSpreadSheetData data in comment)
            {
                if (_commentPool.TryPop(out DisplayedComment displayedComment))
                {
                    displayedComment.SetText(data.Comment);
                    _commentPlacer.Set(displayedComment);
                }
            }

            // �z�u�����R�����g�𗬂��B
            _commentPlacer.Play();

            return comment;
        }
    }
}
