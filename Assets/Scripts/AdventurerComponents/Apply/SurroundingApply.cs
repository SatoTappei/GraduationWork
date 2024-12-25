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

        public void RemoveAction()
        {
            if (IsAdventurerExist(Vector2Int.up))
            {
                _availableActions.Remove("Move North");
            }

            if (IsAdventurerExist(Vector2Int.down))
            {
                _availableActions.Remove("Move South");
            }

            if (IsAdventurerExist(Vector2Int.right))
            {
                _availableActions.Remove("Move East");
            }

            if (IsAdventurerExist(Vector2Int.left))
            {
                _availableActions.Remove("Move West");
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
