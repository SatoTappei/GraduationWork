using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ProfileWindow : MonoBehaviour
    {
        [SerializeField] ProfileWindowUI[] _profileWindowUI;

        bool[] _used;

        void Awake()
        {
            _used = new bool[_profileWindowUI.Length];
        }

        public static ProfileWindow Find()
        {
            return GameObject.FindGameObjectWithTag("UiManager").GetComponent<ProfileWindow>();
        }

        public void RegisterStatus(int displayID, IProfileWindowDisplayable status)
        {
            if (!IsInArray(displayID)) return;

            if (_used[displayID])
            {
                Debug.LogWarning($"ä˘Ç…ìoò^çœÇ›ÅB{displayID}");
                return;
            }

            _used[displayID] = true;
            _profileWindowUI[displayID].SetStatus(status);
        }

        public void UpdateStatus(int displayID, IProfileWindowDisplayable status)
        {
            if (IsInArray(displayID))
            {
                _profileWindowUI[displayID].UpdateStatus(status);
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

            if (_profileWindowUI[displayID] != null)
            {
                _profileWindowUI[displayID].DeleteStatus();
            }
        }

        bool IsInArray(int index)
        {
            if (0 <= index && index < _used.Length) return true;

            Debug.LogWarning($"ëŒâûÇ∑ÇÈ{nameof(ProfileWindow)}Ç™ë∂ç›ÇµÇ»Ç¢ÅB: {index}");
            return false;
        }
    }
}
