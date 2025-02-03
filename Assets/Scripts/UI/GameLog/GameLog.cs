using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GameLog : MonoBehaviour
    {
        static GameLog _instance;

        [SerializeField] GameLogUI[] _gameLogUI;

        void Awake()
        {
            if (_instance == null) _instance = this;
            else Destroy(this);
        }

        void OnDestroy()
        {
            if (_instance == this) _instance = null;
        }

        // 4分割した画面の番号を0~3で指定。この番号は冒険者生成時に割り当てられている。
        // 指定しない場合の-1は全ての画面で表示される。
        public static void Add(string label, string value, LogColor color, int displayID = -1)
        {
            if (displayID == -1)
            {
                foreach (GameLogUI ui in _instance._gameLogUI)
                {
                    ui.Add(label, value, color);
                }

                return;
            }

            // 冒険者の番号に対応するUIがあるかチェック。
            if (displayID < 0 || _instance._gameLogUI.Length <= displayID)
            {
                Debug.LogWarning($"番号に対応するUIが無い。{displayID}");
                return;
            }

            _instance._gameLogUI[displayID].Add(label, value, color);
        }
    }
}
