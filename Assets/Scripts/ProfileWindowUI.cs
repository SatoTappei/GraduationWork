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
        [SerializeField] Text _infomation;

        CanvasGroup _canvasGroup;
        AudioSource _audioSource;

        void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _audioSource = GetComponent<AudioSource>();

            Close();
            DeleteStatus();
        }

        void Start()
        {
            
        }

        public void SetStatus(IProfileWindowDisplayStatus status)
        {
            _name.text = status.FullName;
            _job.text = status.Job;
            _background.text = status.Background;
            
            if (status.Goal == default)
            {
                _goal.text = "--";
            }
            else
            {
                _goal.text = status.Goal;
            }

            _item.text = string.Empty;
            foreach (string s in status.Items)
            {
                if (s == default) _item.text += "--\n";
                else _item.text += $"{s}\n";
            }

            _infomation.text = string.Empty;
            foreach (string s in status.Infomation)
            {
                if (s == default) _infomation.text += "--\n";
                else _infomation.text += $"{s}\n";
            }
        }

        public void UpdateStatus(IProfileWindowDisplayStatus status)
        {
            //
        }

        public void DeleteStatus()
        {
            _name.text = "--";
            _job.text = "--";
            _background.text = "--";
            _goal.text = "--";

            // 持ち物欄は最大3つ表示可能なデザインになっている。
            _item.text = string.Empty;
            for (int i = 0; i < 3; i++)
            {
                _item.text += "--\n";
            }

            // 知っている情報欄は最大4つ表示可能なデザインになっている。
            _infomation.text = string.Empty;
            for (int i = 0; i < 4; i++)
            {
                _item.text += "--\n";
            }
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
    }
}
