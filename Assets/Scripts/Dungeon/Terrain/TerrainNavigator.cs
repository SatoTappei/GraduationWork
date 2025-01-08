using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class TerrainNavigator
    {
        static readonly string[] Map =
{
            "########################",
            "#_#nnn#nnn#____________#",
            "#_Wwww_eeeE_##########_#",
            "#_#sss#sss#_#nnn#nnnn#_#",
            "#_#########_Wwww#eeeeE_#",
            "#_#nnnnnn##_#sss#ssss#_#",
            "#_#nwween##_##########_#",
            "#_#N####N#___#_______#_#",
            "#__________a___#_#_#___#",
            "##_#######___#_______#_#",
            "#________##S#####S####_#",
            "#_######_#eswww#esw#___#",
            "#_#nwww#_#enwww#esw#_###",
            "#_#n#__#_##N####enw#___#",
            "#_#N####___b_###enw###_#",
            "#________###___WwweE___#",
            "####_############_####_#",
            "#__#_#__#__####nnnnn##_#",
            "#____#__#__###nnnnnnn#_#",
            "####_##_##_###eeeeeeeE_#",
            "#__#_______###sssssss#_#",
            "#____##########sssss####",
            "########################"
        };

        static Dictionary<char, Information[]> Mapping = new Dictionary<char, Information[]>()
        {
            {
                'a',
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
            },
            {
                'b',
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
                'n',
                new Information[1]
                {
                    new Information(
                        "北へ移動すると部屋の出口がある。",
                        "Moving north, there is an exit to the room.",
                        "システム",
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
                        "南へ移動すると部屋の出口がある。",
                        "Moving south, there is an exit to the room.",
                        "システム",
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
                        "東へ移動すると部屋の出口がある。",
                        "Moving east, there is an exit to the room.",
                        "システム",
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
                        "西へ移動すると部屋の出口がある。",
                        "Moving west, there is an exit to the room.",
                        "システム",
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
                        "部屋の出入り口に立っている。",
                        "Standing at the entrance to the room. To the north is the corridor.",
                        "システム",
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
                        "部屋の出入り口に立っている。",
                        "Standing at the entrance to the room. To the south is the corridor.",
                        "システム",
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
                        "部屋の出入り口に立っている。",
                        "Standing at the entrance to the room. To the east is the corridor.",
                        "システム",
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
                        "部屋の出入り口に立っている。",
                        "Standing at the entrance to the room. To the west is the corridor.",
                        "システム",
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
                Debug.LogWarning($"範囲外の座標を指している。: {coords}");

                result = null;
                return false;
            }

            if (Map[coords.y][coords.x] == '#')
            {
                Debug.LogWarning($"地形の特徴を取得しようとした座標が壁。:{coords}");
            }

            // 呼び出し元で中身の値を弄っても影響が出ないよう、コピーして渡す。
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
