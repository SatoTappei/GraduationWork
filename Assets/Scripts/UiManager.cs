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

        public int AddBadge(Adventure adventure)
        {
            for (int i = 0; i < _used.Length; i++)
            {
                if (_used[i]) continue;

                _used[i] = true;
                _badges[i].SetAdventureData();

                return i;
            }

            Debug.LogWarning($"これ以上バッジを追加出来ない。: {adventure.name}");

            return -1;
        }

        public void UpdateBadge(int id, Adventure adventure)
        {
            _badges[id].UpdateAdventureData();
        }

        public void ShowAdventureLine(int id, string line)
        {

        }

        public void RemoveBadge(int id)
        {
            if (id < 0 || _used.Length <= id)
            {
                Debug.LogWarning($"IDに対応するバッジが存在しない。: {id}");
                return;
            }

            for (int i = 0; i < _used.Length; i++)
            {
                if (_used[i] && i == id)
                {
                    _used[i] = false;
                    _badges[i].DeleteAdventureData();

                    return;
                }
            }

            Debug.LogWarning($"既に削除済みのバッジ。: {id}");
        }
    }
}

// アイコン、表示名、体力、心情
// 名前、キャラクター設定
// 体力と心情の更新
// セリフの表示。