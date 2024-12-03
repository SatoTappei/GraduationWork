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
            // �_���W�����������A�h�A�𐶐�����Z���� ��(8),��(2),��(4),�E(6) �Ō������w�肵�Ă���B
            bool isDoorPlaced = "2468".Contains(Blueprint.Doors[coords.y][coords.x]);
            Actor actor = _dungeonManager.GetCell(coords).GetActors().Where(c => c.ID == "Door").FirstOrDefault();
            if (isDoorPlaced && actor is Door door)
            {
                door.Interact(_adventurer);
            }
        }
    }
}
