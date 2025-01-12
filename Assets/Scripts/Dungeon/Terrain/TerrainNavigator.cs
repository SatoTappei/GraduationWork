using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class TerrainNavigator
    {
        // 東と西はそのまま右と左に対応しているが、南と北は上下が逆(南が上、北が下)になっている。
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
