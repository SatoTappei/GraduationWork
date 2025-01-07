using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class TerrainNavigator
    {
        // 0: 1: 2: 3: 4: 5: 6: 7: 
        static readonly string[] Map =
{
            "########################",
            "#_#000#000#____________#",
            "#_4111_3335_##########_#",
            "#_#222#222#_#000#0000#_#",
            "#_#########_4111#33335_#",
            "#_#000000##_#222#2222#_#",
            "#_#011330##_##########_#",
            "#_#6####6#___#_______#_#",
            "#______________#_#_#___#",
            "##_#######___#_______#_#",
            "#________##7#####7####_#",
            "#_######_#32111#321#___#",
            "#_#0111#_#30111#321#_###",
            "#_#0#__#_##6####301#___#",
            "#_#6####_____###301###_#",
            "#________###___41135___#",
            "####_############_####_#",
            "#__#_#__#__####00000##_#",
            "#____#__#__###0000000#_#",
            "####_##_##_###33333335_#",
            "#__#_______###2222222#_#",
            "#____##########22222####",
            "########################"
        };

        static Dictionary<Vector2Int, Information[]> Data = new Dictionary<Vector2Int, Information[]>()
        {
            {
                new Vector2Int(11, 14), // ���W
                new Information[1]
                {
                    new Information(
                        "���͒����L�����A���ɂ͋Ȃ���p������B",
                        "West is a long corridor. East is a bend.",
                        "�V�X�e��",
                        1.0f,
                        2
                    ),
                }
            },
            {
                new Vector2Int(11, 8),
                new Information[4]
                {
                    new Information(
                        "���̕����̓����Ƀh�A������B",
                        "There is a door on the east side of this room.",
                        "�V�X�e��",
                        1.0f,
                        3
                    ),
                    new Information(
                        "�����ɂ͒����L��������B",
                        "On the west side is a long corridor.",
                        "�V�X�e��",
                        1.0f,
                        3
                    ),
                    new Information(
                        "�쑤�ɂ͒����L��������B",
                        "On the south side is a long corridor.",
                        "�V�X�e��",
                        1.0f,
                        3
                    ),
                    new Information(
                        "���̕����̖k���Ƀh�A������B",
                        "There is a door on the north side of this room.",
                        "�V�X�e��",
                        1.0f,
                        3
                    )
                }
            }
        };

        public static bool TryGet(Vector2Int coords, out IReadOnlyList<Information> result)
        {
            if (coords.y < 0 || Map.Length <= coords.y || 
                coords.x < 0 || Map[0].Length <= coords.x)
            {
                Debug.LogWarning($"�͈͊O�̍��W���w���Ă���B: {coords}");

                result = null;
                return false;
            }

            if (Map[coords.y][coords.x] == '#')
            {
                Debug.LogWarning($"�n�`�̓������擾���悤�Ƃ������W���ǁB:{coords}");
            }

            // �Ăяo�����Œ��g�̒l��M���Ă��e�����o�Ȃ��悤�A�R�s�[���ēn���B
            List<Information> copy = new List<Information>();
            if (Data.TryGetValue(coords, out Information[] information))
            {
                foreach (Information e in information)
                {
                    copy.Add(new Information(
                        e.Text.Japanese,
                        e.Text.English,
                        e.Source,
                        e.Score,
                        e.Turn
                    ));
                }
            }

            if (Map[coords.y][coords.x] == '0')
            {
                copy.Add(new Information(
                    "��ֈړ�����ƕ����̏o��������B",
                    "",
                    "�V�X�e��",
                    1.0f,
                    1
                ));
            }
            else if (Map[coords.y][coords.x] == '1')
            {
                copy.Add(new Information(
                    "�k�ֈړ�����ƕ����̏o��������B",
                    "",
                    "�V�X�e��",
                    1.0f,
                    1
                ));
            }
            else if (Map[coords.y][coords.x] == '2')
            {
                copy.Add(new Information(
                    "���ֈړ�����ƕ����̏o��������B",
                    "",
                    "�V�X�e��",
                    1.0f,
                    1
                ));
            }
            else if (Map[coords.y][coords.x] == '3')
            {
                copy.Add(new Information(
                    "���ֈړ�����ƕ����̏o��������B",
                    "",
                    "�V�X�e��",
                    1.0f,
                    1
                ));
            }
            else if (Map[coords.y][coords.x] == '4')
            {
                copy.Add(new Information(
                    "�����͕����̏o������B�k�ֈړ�����ƘL���ɏo��B",
                    "",
                    "�V�X�e��",
                    1.0f,
                    1
                ));
            }
            else if (Map[coords.y][coords.x] == '5')
            {
                copy.Add(new Information(
                    "�����͕����̏o������B��ֈړ�����ƘL���ɏo��B",
                    "",
                    "�V�X�e��",
                    1.0f,
                    1
                ));
            }
            else if (Map[coords.y][coords.x] == '6')
            {
                copy.Add(new Information(
                    "�����͕����̏o������B��ֈړ�����ƘL���ɏo��B",
                    "",
                    "�V�X�e��",
                    1.0f,
                    1
                ));
            }
            else if (Map[coords.y][coords.x] == '7')
            {
                copy.Add(new Information(
                    "�����͕����̏o������B��ֈړ�����ƘL���ɏo��B",
                    "",
                    "�V�X�e��",
                    1.0f,
                    1
                ));
            }

            result = copy;
            return copy.Count > 0;
        }
    }
}
