using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class DungeonEntity : Actor
    {
        Vector2Int _coords;
        Vector2Int _direction;

        public override Vector2Int Coords => _coords;
        public override Vector2Int Direction => _direction;

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
            DungeonManager.RemoveActor(Coords, this);

            transform.position = new Vector3(coords.x, 0, coords.y);            
            transform.Rotate(GetEulers(direction));
            _coords = coords;
            _direction = direction;

            DungeonManager.AddActor(Coords, this);
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
