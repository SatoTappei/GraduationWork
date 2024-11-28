using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CommentPlacer
    {
        class Row
        {
            public float Height { get; set; }
            public float PositionX { get; set; }

            public Vector2 Position => new Vector2(PositionX, Height);
        }

        Row[] _rows;
        List<DisplayedComment> _placed;

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

            ResetPositionX();

            _placed = new List<DisplayedComment>();
        }

        // �R�����g���m�����Ȃ����A�����_���Ȉʒu�ɔz�u����B
        public void Set(DisplayedComment comment)
        {
            Row row = _rows[Random.Range(0, _rows.Length)];
            comment.SetPosition(row.Position);
            row.PositionX += comment.TextSize + 55.0f;

            _placed.Add(comment);
        }

        // �z�u�����R�����g�𗬂��B
        public void Play()
        {
            _placed.ForEach(c => c.Play());
            _placed.Clear();

            ResetPositionX();
        }

        void ResetPositionX()
        {
            // �E���̉�ʊO�B
            const float OffScreen = 2000.0f;
            // 1�s���ɏ����ʒu�̍��E�𑽏������_���ɂ��炷�B
            const float RandomOffset = 60.0f;

            foreach (Row row in _rows)
            {
                row.PositionX = OffScreen + Random.Range(0, RandomOffset);
            }
        }
    }
}