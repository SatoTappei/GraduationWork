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
                new Vector2Int(11, 14), // 座標
                new Information[1]
                {
                    new Information(
                        "西は長い廊下が、東には曲がり角がある。",
                        "West is a long corridor. East is a bend.",
                        "システム",
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
                        "この部屋の東側にドアがある。",
                        "There is a door on the east side of this room.",
                        "システム",
                        1.0f,
                        3
                    ),
                    new Information(
                        "西側には長い廊下がある。",
                        "On the west side is a long corridor.",
                        "システム",
                        1.0f,
                        3
                    ),
                    new Information(
                        "南側には長い廊下がある。",
                        "On the south side is a long corridor.",
                        "システム",
                        1.0f,
                        3
                    ),
                    new Information(
                        "この部屋の北側にドアがある。",
                        "There is a door on the north side of this room.",
                        "システム",
                        1.0f,
                        3
                    )
                }
            }
        };

        public static bool TryGet(Vector2Int coords, out IReadOnlyList<Information> result)
        {
            // 呼び出し元で中身の値を弄っても影響が出ないよう、コピーして渡す。
            List<Information> copy = new List<Information>();

            if (coords.y < 0 || Map.GetLength(0) <= coords.y || 
                coords.x < 0 || Map.GetLength(1) <= coords.x)
            {
                Debug.LogWarning($"範囲外の座標を指している。: {coords}");

                result = null;
                return false;
            }

            if (Map[coords.y][coords.x] == '#')
            {
                Debug.LogWarning($"地形の特徴を取得しようとした座標が壁。:{coords}");
            }

            if ()
        }
    }
}
