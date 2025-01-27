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
            // �s���͎��ۂɉ�ʂɕ\�������R�����g�̕����̑傫��������������B
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

        // �R�����g���m�����Ȃ����A�����_���Ȉʒu�ɔz�u����B
        public void Set(CommentUI comment)
        {
            if (comment == null)
            {
                Debug.LogWarning("�z�u���悤�Ƃ����R�����g��null�������B");
            }
            else
            {
                Row row = _rows[Random.Range(0, _rows.Length)];
                comment.SetPosition(row.Position);
                row.X += comment.TextSize + 55.0f;

                _placed.Add(comment);
            }
        }

        // �z�u�����R�����g�𗬂��B
        public void Play()
        {
            _placed.ForEach(c => c.Flow());
            _placed.Clear();

            ResetPosition();
        }

        void ResetPosition()
        {
            // �E���̉�ʊO�B
            const float OffScreen = 2000.0f;
            // 1�s���ɏ����ʒu�̍��E�𑽏������_���ɂ��炷�B
            const float RandomOffset = 60.0f;

            foreach (Row row in _rows)
            {
                row.X = OffScreen + Random.Range(0, RandomOffset);
            }
        }
    }
}