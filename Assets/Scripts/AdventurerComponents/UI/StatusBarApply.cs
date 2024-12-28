using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class StatusBarApply : MonoBehaviour, IStatusBarDisplayStatus
    {
        Blackboard _blackboard;
        StatusBar _statusBar;
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
            StatusBar.TryFind(out _statusBar);
        }

        public void Register()
        {
            _id = _statusBar.RegisterStatus(this);
            _isRegistered = true;
        }

        public void Apply()
        {
            if (_isRegistered) _statusBar.UpdateStatus(_id, this);
        }

        public void ShowLine(string line)
        {
            if (_isRegistered) _statusBar.ShowLine(_id, line);
        }

        void OnDestroy()
        {
            if (_statusBar != null)
            {
                _statusBar.DeleteStatus(_id);
                _isRegistered = false;
            }
        }
    }
}
