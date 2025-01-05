using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class MovementPath : MonoBehaviour
    {
        Pathfinding _pathfinding;
        List<Cell> _path;
        string _target;
        int _currentIndex;

        void Awake()
        {
            _pathfinding = Pathfinding.Find();
            _path = new List<Cell>();
        }

        public string Target => _target;

        public void Clear()
        {
            _path.Clear();
            _target = string.Empty;
            _currentIndex = 0;
        }

        public void Finding(string target, Vector2Int start, Vector2Int goal)
        {
            _path.Clear();
            _pathfinding.CalculatePath(start, goal, _path);
            _target = target;
            _currentIndex = 0;
        }

        public void Create(string target, params Cell[] cells)
        {
            _path.Clear();
            _path.AddRange(cells);
            _target = target;
            _currentIndex = 0;
        }

        public Cell GetCurrent()
        {
            if (_path.Count == 0)
            {
                Debug.LogWarning("�o�H��������ԂŌ��݂̈ړ���̃Z�����擾���悤�Ƃ����B");
                return null;
            }
            else if (_path[_currentIndex] == null)
            {
                Debug.LogWarning("�o�H�̈ړ���̃Z����null�ɂȂ��Ă���B");
                return null;
            }
            else
            {
                return _path[_currentIndex];
            }
        }

        public void SetNext()
        {
            if (_path.Count == 0)
            {
                Debug.LogWarning("�o�H��������ԂŎ��̈ړ���̃Z�����Z�b�g���悤�Ƃ����B");
            }
            else
            {
                _currentIndex++;
                _currentIndex = Mathf.Min(_currentIndex, _path.Count - 1);
            }
        }
    }
}
