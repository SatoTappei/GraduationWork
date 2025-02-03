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

        public static StatusBar Find()
        {
            return GameObject.FindGameObjectWithTag("UiManager").GetComponent<StatusBar>();
        }

        public void RegisterStatus(int displayID, IStatusBarDisplayable status)
        {
            if (!IsInArray(displayID)) return;

            if (_used[displayID])
            {
                Debug.LogWarning($"ä˘Ç…ìoò^çœÇ›ÅB{displayID}");
                return;
            }

            _used[displayID] = true;
            _statusBarUI[displayID].SetStatus(status);
        }

        public void UpdateStatus(int displayID, IStatusBarDisplayable status)
        {
            if (IsInArray(displayID))
            {
                _statusBarUI[displayID].UpdateStatus(status);
            }
        }

        public void ShowLine(int displayID, string line)
        {
            if (IsInArray(displayID))
            {
                _statusBarUI[displayID].ShowLine(line);
            }
        }

        public void DeleteStatus(int displayID)
        {
            if (!IsInArray(displayID)) return;

            if (!_used[displayID])
            {
                Debug.LogWarning($"ä˘Ç…çÌèúçœÇ›ÅB{displayID}");
                return;
            }

            _used[displayID] = false;

            if (_statusBarUI[displayID] != null)
            {
                _statusBarUI[displayID].DeleteStatus();
            }
        }

        bool IsInArray(int index)
        {
            if (0 <= index && index < _used.Length) return true;

            Debug.LogWarning($"ëŒâûÇ∑ÇÈ{nameof(StatusBarUI)}Ç™ë∂ç›ÇµÇ»Ç¢ÅB: {index}");
            return false;
        }
    }
}
