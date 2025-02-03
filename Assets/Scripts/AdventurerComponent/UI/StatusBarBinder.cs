using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class StatusBarBinder : MonoBehaviour, IStatusBarDisplayable
    {
        Adventurer _adventurer;
        StatusBar _statusBar;
        bool _isRegistered;

        Sprite IStatusBarDisplayable.Icon => _adventurer.AdventurerSheet.Icon;
        string IStatusBarDisplayable.DisplayName => _adventurer.AdventurerSheet.DisplayName;
        int IStatusBarDisplayable.MaxHp => _adventurer.Status.MaxHp;
        int IStatusBarDisplayable.CurrentHp => _adventurer.Status.CurrentHp;
        int IStatusBarDisplayable.MaxEmotion => _adventurer.Status.MaxEmotion;
        int IStatusBarDisplayable.CurrentEmotion => _adventurer.Status.CurrentEmotion;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _statusBar = StatusBar.Find();
        }

        public void Register()
        {
            if (_isRegistered)
            {
                Debug.LogWarning("既に登録済み。");
            }
            else
            {
                _statusBar.RegisterStatus(_adventurer.AdventurerSheet.Number, this);
                _isRegistered = true;
            }
        }

        public void Apply()
        {
            if (_isRegistered)
            {
                _statusBar.UpdateStatus(_adventurer.AdventurerSheet.Number, this);
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
                _statusBar.ShowLine(_adventurer.AdventurerSheet.Number, line);
            }
            else
            {
                Debug.LogWarning("登録していない状態で台詞を表示しようとした。");
            }
        }

        void OnDestroy()
        {
            if (_statusBar != null)
            {
                _statusBar.DeleteStatus(_adventurer.AdventurerSheet.Number);
            }

            _isRegistered = false;
        }
    }
}
