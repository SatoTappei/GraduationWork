using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class ContainerBind
    {
        class Data
        {
            public float Interval;
            public string[] Items;
        }

        static Dictionary<Vector2Int, Data> Bind = new Dictionary<Vector2Int, Data>()
        {
            {
                new Vector2Int(3, 1),
                new Data()
                {
                    Interval = 30, // �P�ʂ͕b�B 
                    Items = new string[] { "�N���b�J�[", "��֒e", "�ו�", "�K���N�^" }
                }
            },
            {
                new Vector2Int(7, 5),
                new Data()
                {
                    Interval = 120,
                    Items = new string[] { "�K�т���", "�w�����b�g" }
                }
            },
            {
                new Vector2Int(8, 5),
                new Data()
                {
                    Interval = 30,
                    Items = new string[] { "��֒e", "�ו�", "�K���N�^" }
                }
            },
            {
                new Vector2Int(18, 5),
                new Data()
                {
                    Interval = 30,
                    Items = new string[] { "��ꂽ�", "�ו�" }
                }
            },
            {
                new Vector2Int(14, 11),
                new Data()
                {
                    Interval = 10,
                    Items = new string[] { "��֒e", "�K���N�^" }
                }
            },
            {
                new Vector2Int(15, 3),
                new Data()
                {
                    Interval = 30,
                    Items = new string[] { "��֒e", "�ו�", "�K���N�^" }
                }
            },
            {
                new Vector2Int(20, 3),
                new Data()
                {
                    Interval = 30,
                    Items = new string[] { "�K���N�^" }
                }
            },
            {
                new Vector2Int(17, 5),
                new Data()
                {
                    Interval = 30,
                    Items = new string[] { "�؂ꂽ�d��", "�ו�" }
                }
            },
            {
                new Vector2Int(16, 13),
                new Data()
                {
                    Interval = 30,
                    Items = new string[] { "�ו�", "��֒e" }
                }
            },
            {
                new Vector2Int(16, 14),
                new Data()
                {
                    Interval = 30,
                    Items = new string[] { "�N���b�J�[", "�K���N�^" }
                }
            },
            {
                new Vector2Int(10, 17),
                new Data()
                {
                    Interval = 10,
                    Items = new string[] { "�N���b�J�[", "�K�т���", "�؂ꂽ�d��", "�w�����b�g" }
                }
            },
        };

        public static float GetInterval(Vector2Int coords)
        {
            if (Bind.TryGetValue(coords, out Data data))
            {
                return data.Interval;
            }
            else
            {
                return 0;
            }
        }

        public static IReadOnlyList<string> GetItems(Vector2Int coords)
        {
            if (Bind.TryGetValue(coords, out Data data))
            {
                return data.Items;
            }
            else
            {
                return new List<string>();
            }
        }
    }
}