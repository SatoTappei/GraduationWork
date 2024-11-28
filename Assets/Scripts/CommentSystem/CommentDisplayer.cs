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

        // ゲーム開始時、スプレッドシートから非同期でコメントを読み込んでいる。
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

        // 冒険者の名前に対応したコメントを流す。
        public IReadOnlyCollection<CommentSpreadSheetData> Display(string fullName)
        {
            // コメントのロードが終わっていない場合。
            if (_commentLoader.IsLoading) return null;

            // 冒険者へのコメントが無い場合。
            if (!_commentLoader.TryGetComment(fullName, out IReadOnlyCollection<CommentSpreadSheetData> comment))
            {
                return null;
            }
            
            // コメント同士が被らないように画面に配置。
            foreach (CommentSpreadSheetData data in comment)
            {
                if (_commentPool.TryPop(out DisplayedComment displayedComment))
                {
                    displayedComment.SetText(data.Comment);
                    _commentPlacer.Set(displayedComment);
                }
            }

            // 配置したコメントを流す。
            _commentPlacer.Play();

            return comment;
        }
    }
}
