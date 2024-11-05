using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class HoldedInformation
    {
        const int Max = 4;

        List<SharedInformation> _contents;
        
        public HoldedInformation(params SharedInformation[] information)
        {
            _contents = new List<SharedInformation>();
            _contents.AddRange(information);
        }

        public IReadOnlyList<SharedInformation> Contents => _contents;

        public void Add(SharedInformation newInfo)
        {
            // 既に知っている情報の場合はスコアのみを更新する。
            foreach (SharedInformation info in _contents)
            {
                if (info.Text.Japanese == newInfo.Text.Japanese)
                {
                    info.Score = Mathf.Max(info.Score, newInfo.Score);
                    return;
                }
            }

            _contents.Add(newInfo);
            Sort(_contents);
            
            if (_contents.Count > Max)
            {
                _contents.RemoveAt(Max - 1);
            }
        }

        public void Remove()
        {
            //
        }

        static void Sort(List<SharedInformation> list)
        {
            Sort(list, 0, list.Count - 1);
        }

        static void Sort(List<SharedInformation> list, int left, int right)
        {
            if (left >= right) return;

            float pivot = list[right].Score;
            int current = left;
            for (int i = left; i <= right - 1; i++)
            {
                if (list[i].Score > pivot)
                {
                    Swap(list, current, i);
                    current++;
                }
            }

            Swap(list, current, right);

            Sort(list, left, current - 1);
            Sort(list, current + 1, right);
        }

        static void Swap(List<SharedInformation> list, int a, int b)
        {
            SharedInformation x = list[a];
            list[a] = list[b];
            list[b] = x;
        }
    }
}
