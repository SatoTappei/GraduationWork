using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CommentDisplayer : MonoBehaviour
    {
        CommentLoader _commentLoader;

        // ゲーム開始時、データベースから非同期でコメントを読み込んでいる。
        public bool IsReady => !_commentLoader.IsLoading;

        void Awake()
        {
            _commentLoader = GetComponent<CommentLoader>();
        }

        public static CommentDisplayer Find()
        {
            return GameObject.FindGameObjectWithTag("CommentDisplayer").GetComponent<CommentDisplayer>();
        }

        // 冒険者の名前に対応したコメントを流す。
        public IReadOnlyCollection<CommentData> Display(string fullName)
        {
            // コメントのロードが終わっていない場合。
            if (_commentLoader.IsLoading) return null;

            // 冒険者へのコメントが無い場合。
            if (!_commentLoader.TryGetComment(fullName, out IReadOnlyCollection<CommentData> comment))
            {
                return null;
            }
            
            // ログに追加。
            foreach (CommentData data in comment)
            {
                GameLog.Add($"{data.Name}へのコメント", data.Comment, GameLogColor.Green);
            }

            return comment;
        }
    }
}
