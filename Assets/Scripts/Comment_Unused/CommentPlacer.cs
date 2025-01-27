using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Unused
{
    public class CommentPlacer
    {
        class Row
        {
            public float Height { get; set; }
            public float X { get; set; }

            public Vector2 Position => new Vector2(X, Height);
        }

        Row[] _rows;
        List<CommentUI> _placed;

        public CommentPlacer()
        {
            // 行数は実際に画面に表示されるコメントの文字の大きさを見つつ調整する。
            _rows = new Row[16];
            for (int i = 0; i < _rows.Length; i++)
            {
                const float LineSpace = 60.0f;
                const float LineOffset = 100.0f;

                _rows[i] = new Row();
                _rows[i].Height = LineSpace * i + LineOffset;
            }

            ResetPosition();

            _placed = new List<CommentUI>();
        }

        // コメント同士が被らないかつ、ランダムな位置に配置する。
        public void Set(CommentUI comment)
        {
            if (comment == null)
            {
                Debug.LogWarning("配置しようとしたコメントがnullだった。");
            }
            else
            {
                Row row = _rows[Random.Range(0, _rows.Length)];
                comment.SetPosition(row.Position);
                row.X += comment.TextSize + 55.0f;

                _placed.Add(comment);
            }
        }

        // 配置したコメントを流す。
        public void Play()
        {
            _placed.ForEach(c => c.Flow());
            _placed.Clear();

            ResetPosition();
        }

        void ResetPosition()
        {
            // 右側の画面外。
            const float OffScreen = 2000.0f;
            // 1行毎に初期位置の左右を多少ランダムにずらす。
            const float RandomOffset = 60.0f;

            foreach (Row row in _rows)
            {
                row.X = OffScreen + Random.Range(0, RandomOffset);
            }
        }
    }
}