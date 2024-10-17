using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class BadgeGroup : MonoBehaviour
    {
        [SerializeField] BadgeUI[] _badges;

        bool[] _used;

        void Awake()
        {
            _used = new bool[_badges.Length];
        }

        public int Provide(IBadgeDisplayStatus status)
        {
            for (int i = 0; i < _used.Length; i++)
            {
                if (_used[i]) continue;

                _used[i] = true;
                //_badges[i].SetStatus(status);

                return i;
            }

            Debug.LogWarning($"����ȏ�o�b�W��񋟏o���Ȃ��B");

            return -1;
        }

        public BadgeUI Get(int id)
        {
            if (IsInArray(id)) return _badges[id];
            else return null;
        }

        //public void UpdateBadge(int id, IBadgeDisplayStatus status)
        //{
        //    if (IsInArray(id))
        //    {
        //        _badges[id].UpdateStatus(status);
        //    }
        //}

        //public void ShowAdventureLine(int id, string line)
        //{
        //    if (IsInArray(id))
        //    {
        //        _badges[id].ShowLine(line);
        //    }
        //}

        public void Return(int id)
        {
            if (!IsInArray(id)) return;

            for (int i = 0; i < _used.Length; i++)
            {
                if (_used[i] && i == id)
                {
                    _used[i] = false;
                    //_badges[i].DeleteStatus();

                    return;
                }
            }

            Debug.LogWarning($"���ɕԋp�ς݂̃o�b�W�B: {id}");
        }

        bool IsInArray(int Index)
        {
            if (0 <= Index && Index < _used.Length) return true;

            Debug.LogWarning($"ID�ɑΉ�����o�b�W�����݂��Ȃ��B: {Index}");
            return false;
        }
    }
}
