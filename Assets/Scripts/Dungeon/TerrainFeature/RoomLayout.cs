using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class RoomLayout
    {
        // 0: 1: 2: 3: 4: 5: 6: 7: 
        public static readonly string[] Map =
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
            "#________##_#####7####_#",
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
            // �Ăяo�����Œ��g�̒l��M���Ă��e�����o�Ȃ��悤�A�R�s�[���ēn���B
            List<Information> copy = new List<Information>();

            if (coords.y < 0 || Map.GetLength(0) <= coords.y || 
                coords.x < 0 || Map.GetLength(1) <= coords.x)
            {
                Debug.LogWarning($"�͈͊O�̍��W���w���Ă���B: {coords}");

                result = null;
                return false;
            }

            if (Map[coords.y][coords.x] == '#')
            {
                Debug.LogWarning($"�n�`�̓������擾���悤�Ƃ������W���ǁB:{coords}");
            }

            if ()
        }
    }
}
