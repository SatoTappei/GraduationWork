using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class StatusBar : MonoBehaviour
    {
        [SerializeField] StatusBarUI[] _statusBarUI;

        bool[] _used;

        void Awake()
        {
            _used = new bool[_statusBarUI.Length];
        }

        public int RegisterStatus(IStatusBarDisplayStatus status)
        {
            for (int i = 0; i < _used.Length; i++)
            {
                if (_used[i]) continue;

                _used[i] = true;
                _statusBarUI[i].SetStatus(status);
                
                return i;
            }

            Debug.LogWarning($"未使用の{nameof(StatusBarUI)}がない。");

            return -1;
        }

        public void UpdateStatus(int id, IStatusBarDisplayStatus status)
        {
            if (IsWithinArray(id))
            {
                _statusBarUI[id].UpdateStatus(status);
            }
        }

        public void DeleteStatus(int id)
        {
            if (!IsWithinArray(id)) return;

            for (int i = 0; i < _used.Length; i++)
            {
                // 既に未使用もしくはIDが違う場合。
                if (!_used[i] || i != id) continue;

                _used[i] = false;

                if (_statusBarUI[i] != null) _statusBarUI[i].DeleteStatus();

                return;
            }

            Debug.LogWarning($"既に削除済みの{nameof(StatusBarUI)}。: {id}");
        }

        public void ShowLine(int id, string line)
        {
            if (IsWithinArray(id))
            {
                _statusBarUI[id].ShowLine(line);
            }
        }

        bool IsWithinArray(int index)
        {
            if (0 <= index && index < _used.Length) return true;

            Debug.LogWarning($"IDに対応する{nameof(StatusBarUI)}が存在しない。: {index}");
            return false;
        }
    }
}
