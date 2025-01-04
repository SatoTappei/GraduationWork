using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class DefeatEnemyGoal : SubGoal
    {
        BilingualString _description;
        Adventurer _adventurer;

        void Awake()
        {
            _description = new BilingualString(
                "é„ÇªÇ§Ç»ìGÇì|Ç∑ÅB", 
                "Defeat weak enemies roaming in the dungeon."
            );
            _adventurer = GetComponent<Adventurer>();
        }

        public override BilingualString Description => _description;

        public override bool IsCompleted()
        {
            // íEèoÇÃìÔà’ìxÇ™è„Ç™Ç¡ÇƒÇµÇ‹Ç§ÇÃÇ≈ÅA1ëÃì|ÇπÇŒè\ï™ÅB
            return _adventurer.Status.DefeatCount >= 1;
        }
    }
}
