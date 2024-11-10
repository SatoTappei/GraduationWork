using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class ProfileWindowUI : MonoBehaviour
    {
        [SerializeField] Text _name;
        [SerializeField] Text _job;
        [SerializeField] Text _background;
        [SerializeField] Text _goal;
        [SerializeField] Text _item;
        [SerializeField] Text _information;

        CanvasGroup _canvasGroup;
        AudioSource _audioSource;

        void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _audioSource = GetComponent<AudioSource>();

            Close();
            DeleteStatus();
        }

        public void SetStatus(IProfileWindowDisplayStatus status)
        {
            SetName(status.FullName);
            SetJob(status.Job);
            SetBackground(status.Background);
            SetGoal(status.Goal);
            SetItem(status.ItemInventory);
            SetInfomation(status.Information);
        }

        public void UpdateStatus(IProfileWindowDisplayStatus status)
        {
            SetGoal(status.Goal);
            SetItem(status.ItemInventory);
            SetInfomation(status.Information);
        }

        public void DeleteStatus()
        {
            SetName("--");
            SetJob("--");
            SetBackground("--");
            SetGoal("--");
            SetItem(null);
            SetInfomation(null);
        }

        public void Open()
        {
            _canvasGroup.alpha = 1.0f;
            _audioSource.Play();
        }

        public void Close()
        {
            _canvasGroup.alpha = 0;
        }

        void SetName(string fullName)
        {
            _name.text = fullName;
        }

        void SetJob(string job)
        {
            _job.text = job;
        }

        void SetBackground(string background)
        {
            _background.text = background;
        }

        void SetGoal(string goal)
        {
            if (goal == default) _goal.text = "--";
            else _goal.text = goal;
        }

        void SetItem(IReadOnlyInventory itemInventory)
        {
            // ���������͍ő�3�\���\�ȃf�U�C���ɂȂ��Ă���B
            const int Max = 3;

            _item.text = string.Empty;

            if (itemInventory == null)
            {
                for (int i = 0; i < Max; i++)
                {
                    _item.text += "--\n";
                }
            }
            else
            {
                IEnumerable<InventoryItem> allItem = itemInventory.GetAllInventoryItem();
                foreach (InventoryItem item in allItem)
                {
                    _item.text += $"{item.Name}\n";
                }

                int emptyCount = Max - allItem.Count();
                for (int i = 0; i < emptyCount; i++)
                {
                    _item.text += "--\n";
                }
            }
        }

        void SetInfomation(IReadOnlyList<SharedInformation> information)
        {
            // �m���Ă����񗓂͍ő�4�\���\�ȃf�U�C���ɂȂ��Ă���B
            const int Max = 4;

            _information.text = string.Empty;

            if (information == null)
            {
                for (int i = 0; i < Max; i++)
                {
                    _information.text += "--\n";
                }
            }
            else
            {
                for (int i = 0; i < Max; i++)
                {
                    if (information.Count <= i)
                    {
                        _information.text += "--\n";
                    }
                    else if (information[i] == null)
                    {
                        _information.text += "--\n";
                    }
                    else
                    {
                        _information.text += $"{information[i].Text.Japanese} �c��{information[i].RemainingTurn}�^�[��\n";
                    }
                }
            }
        }
    }
}