using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class StatusBarApply : MonoBehaviour, IStatusBarDisplayStatus
    {
        Blackboard _blackboard;
        UiManager _uiManager;
        int _id;
        bool _isRegistered;

        Sprite IStatusBarDisplayStatus.Icon => _blackboard.Icon;
        string IStatusBarDisplayStatus.DisplayName => _blackboard.DisplayName;
        int IStatusBarDisplayStatus.MaxHp => _blackboard.MaxHp;
        int IStatusBarDisplayStatus.CurrentHp => _blackboard.CurrentHp;
        int IStatusBarDisplayStatus.MaxEmotion => _blackboard.MaxEmotion;
        int IStatusBarDisplayStatus.CurrentEmotion => _blackboard.CurrentEmotion;

        void Awake()
        {
            _blackboard = GetComponent<Blackboard>();
            _uiManager = UiManager.Find();
        }

        public void Register()
        {
            _id = _uiManager.RegisterToStatusBar(this);
            _isRegistered = true;
        }

        public void Apply()
        {
            if (_isRegistered) _uiManager.UpdateStatusBarStatus(_id, this);
        }

        public void ShowLine(string line)
        {
            if (_isRegistered) _uiManager.ShowLine(_id, line);
        }

        void OnDestroy()
        {
            if (_uiManager != null)
            {
                _uiManager.DeleteStatusBarStatus(_id);
                _isRegistered = false;
            }
        }
    }
}
