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

        Sprite IStatusBarDisplayable.Icon => _adventurer.Sheet.Icon;
        string IStatusBarDisplayable.DisplayName => _adventurer.Sheet.DisplayName;
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
                Debug.LogWarning("���ɓo�^�ς݁B");
            }
            else
            {
                _statusBar.RegisterStatus(_adventurer.Sheet.DisplayID, this);
                _isRegistered = true;
            }
        }

        public void Apply()
        {
            if (_isRegistered)
            {
                _statusBar.UpdateStatus(_adventurer.Sheet.DisplayID, this);
            }
            else
            {
                Debug.LogWarning("�o�^���Ă��Ȃ���ԂŔ��f���悤�Ƃ����B");
            }
        }

        public void ShowLine(string line)
        {
            if (_isRegistered)
            {
                _statusBar.ShowLine(_adventurer.Sheet.DisplayID, line);
            }
            else
            {
                Debug.LogWarning("�o�^���Ă��Ȃ���Ԃő䎌��\�����悤�Ƃ����B");
            }
        }

        void OnDestroy()
        {
            if (_statusBar != null)
            {
                _statusBar.DeleteStatus(_adventurer.Sheet.DisplayID);
            }

            _isRegistered = false;
        }
    }
}
