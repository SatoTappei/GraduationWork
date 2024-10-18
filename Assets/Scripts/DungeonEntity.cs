using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class DungeonEntity : Actor
    {
        Vector2Int _currentCoords;
        Vector2Int _currentDirection;

        public override Vector2Int Coords => _currentCoords;
        public override Vector2Int Direction => _currentDirection;

        public virtual void Interact(Actor user) 
        {
            // äJÇ¢ÇΩÇËìÆÇ¢ÇΩÇËâΩÇÁÇ©ÇÃédä|ÇØÇ™ìÆÇ≠ÅB
        }

        public void Place(int x, int y, Vector2Int direction)
        {
            Vector2Int coords = new Vector2Int(x, y);
            Place(coords, direction);
        }

        public void Place(Vector2Int coords, Vector2Int direction)
        {
            DungeonManager dm = DungeonManager.Find();
            dm.RemoveActorOnCell(_currentCoords, this);

            transform.position = new Vector3(coords.x, 0, coords.y);            
            transform.Rotate(GetEulers(direction));
            _currentCoords = coords;
            _currentDirection = direction;

            dm.AddActorOnCell(_currentCoords, this);
        }

        Vector3 GetEulers(Vector2Int direction)
        {
            if (direction == Vector2Int.up) return Vector3.up * 0;
            if (direction == Vector2Int.down) return Vector3.up * 180;
            if (direction == Vector2Int.left) return Vector3.up * -90;
            if (direction == Vector2Int.right) return Vector3.up * 90;

            return default;
        }
    }
}
