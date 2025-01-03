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
            Pathfinding.TryFind(out _pathfinding);
            _path = new List<Cell>();
        }

        public Cell Current => _path[_currentIndex];
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

        public void CreateManually(string target, params Cell[] cells)
        {
            _path.Clear();
            _path.AddRange(cells);
            _target = target;
            _currentIndex = 0;
        }

        public void SetNext()
        {
            _currentIndex++;
            _currentIndex = Mathf.Min(_currentIndex, _path.Count - 1);
        }
    }
}
