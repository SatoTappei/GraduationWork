using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class UiManager : MonoBehaviour
    {
        StatusBar _statusBar;
        GameLog _gameLog;
        
        void Awake()
        {
            _statusBar = GetComponent<StatusBar>();
            _gameLog = GetComponent<GameLog>();
        }

        public static UiManager Find()
        {
            return GameObject.FindGameObjectWithTag("UiManager").GetComponent<UiManager>();
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
            _statusBar.DeleteStatus(id);
        }

        public void ShowLine(int statusBarID, string line)
        {
            _statusBar.ShowLine(statusBarID, line);
        }

        public void AddLog(string message)
        {
            _gameLog.Add(message);
        }
    }
}
