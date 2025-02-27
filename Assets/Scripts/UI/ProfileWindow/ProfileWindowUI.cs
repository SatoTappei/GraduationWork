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
        [SerializeField] Text _effect;
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

        public void SetStatus(IProfileWindowDisplayable status)
        {
            SetName(status.FullName);
            SetJob(status.Job);
            SetBackground(status.Background);
            SetGoal(status.CurrentSubGoal);
            SetItem(status.Item);
            SetEffect(status.Effect);
            SetInfomation(status.Information);
        }

        public void UpdateStatus(IProfileWindowDisplayable status)
        {
            SetGoal(status.CurrentSubGoal);
            SetItem(status.Item);
            SetEffect(status.Effect);
            SetInfomation(status.Information);
        }

        public void DeleteStatus()
        {
            SetName("--");
            SetJob("--");
            SetBackground("--");
            SetGoal(null);
            SetItem(null);
            SetEffect(null);
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

        void SetGoal(SubGoal goal)
        {
            if (goal == default) _goal.text = "--";
            else _goal.text = goal.Description.Japanese;
        }

        void SetItem(IEnumerable<ItemEntry> items)
        {
            // 持ち物欄は最大3つ表示可能なデザインになっている。
            const int Max = 3;

            _item.text = string.Empty;

            if (items == null)
            {
                for (int i = 0; i < Max; i++)
                {
                    _item.text += "--、";
                }

                _item.text = _item.text.TrimEnd('、');
            }
            else
            {
                foreach (ItemEntry e in items.Take(Max))
                {
                    _item.text += $"{e.Name}、";
                }

                int emptyCount = Max - items.Count();
                for (int i = 0; i < emptyCount; i++)
                {
                    _item.text += "--、";
                }

                _item.text = _item.text.TrimEnd('、');
            }
        }

        void SetEffect(IEnumerable<string> effects)
        {
            // 効果欄は最大3つ表示可能なデザインになっている。
            const int Max = 3;
            
            _effect.text = string.Empty;

            if (effects == null)
            {
                for (int i = 0; i < Max; i++)
                {
                    _effect.text += "--、";
                }

                _effect.text = _effect.text.TrimEnd('、');
            }
            else
            {
                foreach (string e in effects)
                {
                    _effect.text += $"{e}、";
                }

                int emptyCount = Max - effects.Count();
                for (int i = 0; i < emptyCount; i++)
                {
                    _effect.text += "--、";
                }

                _effect.text = _effect.text.TrimEnd('、');
            }
        }

        void SetInfomation(IReadOnlyList<Information> information)
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
                        _information.text += $"{information[i].Text.Japanese}\n";
                    }
                }
            }
        }
    }
}
