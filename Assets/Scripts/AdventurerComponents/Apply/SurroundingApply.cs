using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class SurroundingApply : MonoBehaviour
    {
        Adventurer _adventurer;
        AvailableActions _availableActions;
        DungeonManager _dungeonManager;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _availableActions = GetComponent<AvailableActions>();
            DungeonManager.TryFind(out  _dungeonManager);
        }

        public void Check()
        {
            Apply(Vector2Int.up, "Move North");
            Apply(Vector2Int.down, "Move South");
            Apply(Vector2Int.right, "Move East");
            Apply(Vector2Int.left, "Move West");
        }

        void Apply(Vector2Int direction, string action)
        {
            if (IsAdventurerExist(direction))
            {
                _availableActions.Remove(action);
            }
            else
            {
                _availableActions.Add(action);
            }
        }

        bool IsAdventurerExist(Vector2Int direction)
        {
            Cell cell = _dungeonManager.GetCell(_adventurer.Coords + direction);
            foreach (Actor actor in cell.GetActors())
            {
                if (actor is Adventurer) return true;
            }

            return false;
        }
    }
}
