using System.Collections;
using System.Collections.Generic;
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
            SetItem(status.Item);
            SetInfomation(status.Information);
        }

        public void UpdateStatus(IProfileWindowDisplayStatus status)
        {
            SetGoal(status.Goal);
            SetItem(status.Item);
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

        void SetItem(IReadOnlyList<string> item)
        {
            // 持ち物欄は最大3つ表示可能なデザインになっている。
            const int Max = 3;

            _item.text = string.Empty;

            if (item == null)
            {
                for (int i = 0; i < Max; i++)
                {
                    _item.text += "--\n";
                }
            }
            else
            {
                for (int i = 0; i < Max; i++)
                {
                    if (item[i] == default) _item.text += "--\n";
                    else _item.text += $"{item[i]}\n";
                }
            }
        }

        void SetInfomation(IReadOnlyList<SharedInformation> information)
        {
            // 知っている情報欄は最大4つ表示可能なデザインになっている。
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
                    if (information[i] == default) _information.text += "--\n";
                    else _information.text += $"{information[i].Text.Japanese}\n";
                }
            }
        }
    }
}
