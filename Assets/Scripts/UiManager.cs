using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Game
{
    public class UiManager : MonoBehaviour
    {
        BadgeGroup _badgeGroup;

        

        Queue<string> _log;
        StringBuilder _stringBuilder;

        void Awake()
        {
            
        }

        public static UiManager Find()
        {
            return FindAnyObjectByType<UiManager>();
        }



        public void AddLog(string message)
        {
            _log ??= new Queue<string>();
            _stringBuilder ??= new StringBuilder();
            _stringBuilder.Clear();

            _log.Enqueue(message);
            if (_log.Count > 4) _log.Dequeue();

            foreach (string s in _log) _stringBuilder.Append(s);

            //_log
        }

        public int SetStatusToNewBadge(IBadgeDisplayStatus status)
        {
            // BadgeGroupクラスには 借りる と 返却する とGetメソッドがある。
            // しかし、これだと任意の値を更新したりするのには借りてidを取得。
            // GetでBadgeを取得して値を更新する必要がある。
            // 今まではBadgeの配列はPrivateだったのにpublicになる必要があり複雑化してしまう。
            // BadgeGroup内で値の操作は完了し、外部にBadgeクラスは渡さない方がSimpleなのでは？
            // そもそもステータスバーという名前のUIパーツらしい。
            int id = _badgeGroup.Provide(status);
        }

        public void UpdateBadgeStatus()
        {

        }

        public void DeleteBadgeStatus()
        {

        }
    }
}
