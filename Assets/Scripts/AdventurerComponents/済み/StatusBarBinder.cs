using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class StatusBarBinder : MonoBehaviour, IStatusBarDisplayable
    {
        Adventurer _adventurer;
        Blackboard _blackboard;
        StatusBar _statusBar;
        int _id;
        bool _isRegistered;

        Sprite IStatusBarDisplayable.Icon => _adventurer.AdventurerSheet.Icon;
        string IStatusBarDisplayable.DisplayName => _adventurer.AdventurerSheet.DisplayName;
        int IStatusBarDisplayable.MaxHp => _blackboard.MaxHp;
        int IStatusBarDisplayable.CurrentHp => _blackboard.CurrentHp;
        int IStatusBarDisplayable.MaxEmotion => _blackboard.MaxEmotion;
        int IStatusBarDisplayable.CurrentEmotion => _blackboard.CurrentEmotion;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _blackboard = GetComponent<Blackboard>();
            StatusBar.TryFind(out _statusBar);
        }

        public void Register()
        {
            if (_isRegistered)
            {
                Debug.LogWarning("既に登録済み。");
            }
            else
            {
                _id = _statusBar.RegisterStatus(this);
                _isRegistered = true;
            }
        }

        public void Apply()
        {
            if (_isRegistered)
            {
                _statusBar.UpdateStatus(_id, this);
            }
            else
            {
                Debug.LogWarning("登録していない状態で反映しようとした。");
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
                Debug.LogWarning("登録していない状態で台詞を表示しようとした。");
            }
        }

        void OnDestroy()
        {
            if (_statusBar != null) _statusBar.DeleteStatus(_id);

            _isRegistered = false;
        }
    }
}
