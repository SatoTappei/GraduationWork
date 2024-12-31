using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class FootTrapApply : MonoBehaviour
    {
        Adventurer _adventurer;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
        }

        public void Activate()
        {
            foreach (Actor actor in DungeonManager.GetActorsOnCell(_adventurer.Coords))
            {
                if (actor.ID == "Trap" && actor is DungeonEntity e)
                {
                    e.Interact(_adventurer);
                }
            }
        }
    }
}