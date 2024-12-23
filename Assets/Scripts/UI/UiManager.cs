using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class UiManager : MonoBehaviour
    {
        GameLog _gameLog;
        StatusBar _statusBar;
        ProfileWindow _profileWindow;
        CameraFocus _cameraFocus;
        
        void Awake()
        {
            _gameLog = GetComponent<GameLog>();
            _statusBar = GetComponent<StatusBar>();
            _profileWindow = GetComponent<ProfileWindow>();
            _cameraFocus = GetComponent<CameraFocus>();
        }

        public static bool TryFind(out UiManager result)
        {
            result = GameObject.FindGameObjectWithTag("UiManager").GetComponent<UiManager>();
            return result != null;
        }

        public int RegisterToStatusBar(IStatusBarDisplayStatus status)
        {
            return _statusBar.RegisterStatus(status);
        }

        public void UpdateStatusBarStatus(int id, IStatusBarDisplayStatus status)
        {
            _statusBar.UpdateStatus(id, status);
        }

        public void DeleteStatusBarStatus(int id)
        {
            if (_statusBar != null) _statusBar.DeleteStatus(id);
        }

        public void ShowLine(int statusBarID, string line)
        {
            _statusBar.ShowLine(statusBarID, line);
        }

        public void AddLog(string label, string text, GameLogColor color)
        {
            _gameLog.Add(label, text, color);
        }

        public int RegisterToProfileWindow(IProfileWindowDisplayStatus status)
        {
            return _profileWindow.RegisterStatus(status);
        }

        public void UpdateProfileWindowStatus(int id, IProfileWindowDisplayStatus status)
        {
            _profileWindow.UpdateStatus(id, status);
        }

        public void DeleteProfileWindowStatus(int id)
        {
            if (_profileWindow != null) _profileWindow.DeleteStatus(id);
        }

        public int RegisterCameraFocusTarget(GameObject target)
        {
            return _cameraFocus.RegisterTarget(target);
        }

        public void DeleteCameraFocusTarget(int id)
        {
            _cameraFocus.DeleteTarget(id);
        }
    }
}
