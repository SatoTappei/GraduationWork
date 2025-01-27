using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CheerCommentEvent : MonoBehaviour
    {
        [System.Serializable]
        class Range
        {
            public Vector2 BottomLeft;
            public Vector2 TopRight;
        }

        [SerializeField] Range[] _ranges;
        [SerializeField] CheerCommentUI[] _cheerCommentUI;

        AdventurerSpawner _spawner;

        void Awake()
        {
            _spawner = AdventurerSpawner.Find();
        }

        public void Execute(string target, string comment, int emotion = 0)
        {
            Execute(GetTargetNumber(target), comment, emotion);
        }

        public void Execute(int number, string comment, int emotion = 0)
        {
            // プールに在庫が無い場合。
            if (!TryGetUI(out CheerCommentUI ui)) return;

            // 画面を4分割しており、その画面に映る冒険者に0から3の番号が割り当てられる。
            if (0 <= number && number <= 3)
            {
                // 分割した画面のランダムな位置にコメントを配置する。
                float x = Random.Range(_ranges[number].BottomLeft.x, _ranges[number].TopRight.x);
                float y = Random.Range(_ranges[number].BottomLeft.y, _ranges[number].TopRight.y);
                ui.transform.position = new Vector2(x, y);

                ui.Play(comment, emotion);
            }
            else
            {
                Debug.LogWarning($"冒険者の番号が範囲外。{number}");
            }
        }

        int GetTargetNumber(string target)
        {
            foreach (Adventurer a in _spawner.Spawned)
            {
                if (a.AdventurerSheet.FullName == target)
                {
                    return a.AdventurerSheet.Number;
                }
            }

            Debug.LogWarning($"ダンジョン内に冒険者がいない。{target}");

            return -1;
        }

        bool TryGetUI(out CheerCommentUI ui)
        {
            foreach (CheerCommentUI c in _cheerCommentUI)
            {
                // 非アクティブなものを返す。
                if (!c.gameObject.activeSelf)
                {
                    c.gameObject.SetActive(true);

                    ui = c;
                    return true;
                }
            }

            ui = null;
            return false;
        }
    }
}
