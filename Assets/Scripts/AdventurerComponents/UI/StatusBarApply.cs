using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class StatusBarApply : MonoBehaviour, IStatusBarDisplayable
    {
        Blackboard _blackboard;
        StatusBar _statusBar;
        int _id;
        bool _isRegistered;

        Sprite IStatusBarDisplayable.Icon => _blackboard.Icon;
        string IStatusBarDisplayable.DisplayName => _blackboard.DisplayName;
        int IStatusBarDisplayable.MaxHp => _blackboard.MaxHp;
        int IStatusBarDisplayable.CurrentHp => _blackboard.CurrentHp;
        int IStatusBarDisplayable.MaxEmotion => _blackboard.MaxEmotion;
        int IStatusBarDisplayable.CurrentEmotion => _blackboard.CurrentEmotion;

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
            if (_isRegistered)
            {
                _statusBar.UpdateStatus(_id, this);
            }
            else
            {
                Debug.LogWarning("“o˜^‚µ‚Ä‚¢‚È‚¢ó‘Ô‚Å”½‰f‚µ‚æ‚¤‚Æ‚µ‚½B");
            }
        }

        public void ShowLine(string line)
        {
            if (_isRegistered)
            {
                _statusBar.ShowLine(_id, line);
            }
            else
            {
                Debug.LogWarning("“o˜^‚µ‚Ä‚¢‚È‚¢ó‘Ô‚Å‘äŒ‚ğ•\¦‚µ‚æ‚¤‚Æ‚µ‚½B");
            }
        }

        void OnDestroy()
        {
            if (_statusBar != null) _statusBar.DeleteStatus(_id);

            _isRegistered = false;
        }
    }
}
