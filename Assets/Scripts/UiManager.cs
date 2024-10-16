using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class UiManager : MonoBehaviour
    {
        [SerializeField] BadgeUI[] _badges;

        bool[] _used;

        void Awake()
        {
            _used = new bool[_badges.Length];
        }

        void Start()
        {

        }

        public static UiManager Find()
        {
            return FindAnyObjectByType<UiManager>();
        }

        public void AddBadge(Adventure adventure)
        {
            for (int i = 0; i < _used.Length; i++)
            {
                if (!_used[i])
                {
                    _used[i] = true;
                    _badges[i].SetAdventureData();
                    break;
                }
            }
        }
    }
}
