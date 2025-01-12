using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class TerrainNavigator
    {
        // ���Ɛ��͂��̂܂܉E�ƍ��ɑΉ����Ă��邪�A��Ɩk�͏㉺���t(�삪��A�k����)�ɂȂ��Ă���B
        static readonly string[] Map =
{
            "########################",
            "#_#___#___#____________#",
            "#___________##########_#",
            "#_#___#___#_#___#____#_#",
            "#_#########_____#______#",
            "#_#______##_#___#____#_#",
            "#_#______##_##########_#",
            "#_#_####_#___#_______#_#",
            "#__________a___#_#_#___#",
            "##_#######___#_______#_#",
            "#________##_#####_####_#",
            "#_######_#_____#___#___#",
            "#_#____#_#_____#___#_###",
            "#_#_#__#_##_####___#___#",
            "#_#_####___b_###___###_#",
            "#________###___________#",
            "####_############_####_#",
            "#__#_#__#__####_____##_#",
            "#____#__#__###_______#_#",
            "####_##_##_###_________#",
            "#__#_______###_______#_#",
            "#____##########_____####",
            "########################"
        };

        static Dictionary<char, Information[]> Mapping = new Dictionary<char, Information[]>()
        {
            {
                'a',
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
            },
            {
                'b',
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
                'n',
                new Information[1]
                {
                    new Information(
                        "�k�ֈړ�����ƕ����̏o��������B",
                        "Moving north, there is an exit to the room.",
                        "�V�X�e��",
                        1.0f,
                        1
                    )
                }
            },
            {
                's',
                new Information[1]
                {
                    new Information(
                        "��ֈړ�����ƕ����̏o��������B",
                        "Moving south, there is an exit to the room.",
                        "�V�X�e��",
                        1.0f,
                        1
                    )
                }
            },
            {
                'e',
                new Information[1]
                {
                    new Information(
                        "���ֈړ�����ƕ����̏o��������B",
                        "Moving east, there is an exit to the room.",
                        "�V�X�e��",
                        1.0f,
                        1
                    )
                }
            },
            {
                'w',
                new Information[1]
                {
                    new Information(
                        "���ֈړ�����ƕ����̏o��������B",
                        "Moving west, there is an exit to the room.",
                        "�V�X�e��",
                        1.0f,
                        1
                    )
                }
            },
            {
                'N',
                new Information[1]
                {
                    new Information(
                        "�����̏o������ɗ����Ă���B",
                        "Standing at the entrance to the room. To the north is the corridor.",
                        "�V�X�e��",
                        1.0f,
                        1
                    )
                }
            },
            {
                'S',
                new Information[1]
                {
                    new Information(
                        "�����̏o������ɗ����Ă���B",
                        "Standing at the entrance to the room. To the south is the corridor.",
                        "�V�X�e��",
                        1.0f,
                        1
                    )
                }
            },
            {
                'E',
                new Information[1]
                {
                    new Information(
                        "�����̏o������ɗ����Ă���B",
                        "Standing at the entrance to the room. To the east is the corridor.",
                        "�V�X�e��",
                        1.0f,
                        1
                    )
                }
            },
            {
                'W',
                new Information[1]
                {
                    new Information(
                        "�����̏o������ɗ����Ă���B",
                        "Standing at the entrance to the room. To the west is the corridor.",
                        "�V�X�e��",
                        1.0f,
                        1
                    )
                }
            },
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
            char key = Map[coords.y][coords.x];
            if (Mapping.TryGetValue(key, out Information[] information))
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

            result = copy;
            return copy.Count > 0;
        }
    }
}
