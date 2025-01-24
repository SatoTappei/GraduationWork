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

        // 4����������ʂ̔ԍ���0~3�Ŏw��B���̔ԍ��͖`���Ґ������Ɋ��蓖�Ă��Ă���B
        // �w�肵�Ȃ��ꍇ��-1�͑S�Ẳ�ʂŕ\�������B
        public static void Add(string label, string value, LogColor color, int id = -1)
        {
            if (id == -1)
            {
                foreach (GameLogUI ui in _instance._gameLogUI)
                {
                    ui.Add(label, value, color);
                }

                return;
            }

            // �`���҂̔ԍ��ɑΉ�����UI�����邩�`�F�b�N�B
            if (id < 0 || _instance._gameLogUI.Length <= id)
            {
                Debug.LogWarning($"�ԍ��ɑΉ�����UI�������B{id}");
                return;
            }

            _instance._gameLogUI[id].Add(label, value, color);
        }
    }
}
