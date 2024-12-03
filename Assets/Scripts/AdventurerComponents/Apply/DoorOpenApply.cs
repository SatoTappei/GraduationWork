using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Game
{
    public class DoorOpenApply : MonoBehaviour
    {
        DungeonManager _dungeonManager;
        Adventurer _adventurer;

        void Awake()
        {
            DungeonManager.TryFind(out _dungeonManager);
            _adventurer = GetComponent<Adventurer>();
        }

        public void Open(Vector2Int coords)
        {
            // ダンジョン生成時、ドアを生成するセルは 上(8),下(2),左(4),右(6) で向きを指定している。
            bool isDoorPlaced = "2468".Contains(Blueprint.Doors[coords.y][coords.x]);
            Actor actor = _dungeonManager.GetCell(coords).GetActors().Where(c => c.ID == "Door").FirstOrDefault();
            if (isDoorPlaced && actor is Door door)
            {
                door.Interact(_adventurer);
            }
        }
    }
}
