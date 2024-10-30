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

        void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        void Start()
        {
            Close();
        }

        public void SetStatus(IProfileWindowDisplayStatus status)
        {
            _name.text = status.FullName;
            _job.text = status.Job;
            _background.text = status.Background;
            
            if (status.Goal == string.Empty)
            {
                _goal.text = "--";
            }
            else
            {
                _goal.text = status.Goal;
            }

            foreach (string s in status.Items)
            {
                if (s == string.Empty) _item.text += "--\n";
                else _item.text += $"{s}\n";
            }

            foreach (string s in status.Infomation)
            {
                if (s == string.Empty) _infomation.text += "--\n";
                else _infomation.text += $"{s}\n";
            }
        }

        public void UpdateStatus(IProfileWindowDisplayStatus status)
        {
            //
        }

        public void DeleteStatus()
        {
            //
        }

        public void Open()
        {
            _canvasGroup.alpha = 1.0f;
        }

        public void Close()
        {
            _canvasGroup.alpha = 0;
        }
    }
}
