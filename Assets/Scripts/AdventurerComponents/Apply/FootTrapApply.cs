using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class FootTrapApply : MonoBehaviour
    {
        Adventurer _adventurer;
        DungeonManager _dungeonManager;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _dungeonManager = DungeonManager.Find();
        }

        public void Activate()
        {
            foreach (Actor actor in _dungeonManager.GetActorsOnCell(_adventurer.Coords))
            {
                if (actor.ID == "Trap" && actor is DungeonEntity e)
                {
                    e.Interact(_adventurer);
                }
            }
        }
    }
}