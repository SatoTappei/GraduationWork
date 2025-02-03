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
                Debug.LogWarning("Šù‚É“o˜^Ï‚İB");
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
                Debug.LogWarning("“o˜^‚µ‚Ä‚¢‚È‚¢ó‘Ô‚Å”½‰f‚µ‚æ‚¤‚Æ‚µ‚½B");
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
                Debug.LogWarning("“o˜^‚µ‚Ä‚¢‚È‚¢ó‘Ô‚Å‘äŒ‚ğ•\¦‚µ‚æ‚¤‚Æ‚µ‚½B");
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
