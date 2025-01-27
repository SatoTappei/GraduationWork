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
            // �v�[���ɍ݌ɂ������ꍇ�B
            if (!TryGetUI(out CheerCommentUI ui)) return;

            // ��ʂ�4�������Ă���A���̉�ʂɉf��`���҂�0����3�̔ԍ������蓖�Ă���B
            if (0 <= number && number <= 3)
            {
                // ����������ʂ̃����_���Ȉʒu�ɃR�����g��z�u����B
                float x = Random.Range(_ranges[number].BottomLeft.x, _ranges[number].TopRight.x);
                float y = Random.Range(_ranges[number].BottomLeft.y, _ranges[number].TopRight.y);
                ui.transform.position = new Vector2(x, y);

                ui.Play(comment, emotion);
            }
            else
            {
                Debug.LogWarning($"�`���҂̔ԍ����͈͊O�B{number}");
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

            Debug.LogWarning($"�_���W�������ɖ`���҂����Ȃ��B{target}");

            return -1;
        }

        bool TryGetUI(out CheerCommentUI ui)
        {
            foreach (CheerCommentUI c in _cheerCommentUI)
            {
                // ��A�N�e�B�u�Ȃ��̂�Ԃ��B
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
