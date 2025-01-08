using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class DefeatBossGoal : SubGoal
    {
        BilingualString _description;
        Adventurer _adventurer;

        void Awake()
        {
            _description = new BilingualString(
                "ã≠óÕÇ»ìGÇì|Ç∑ÅB", 
                "Defeat strong enemies roaming in the dungeon."
            );
            _adventurer = GetComponent<Adventurer>();
        }

        public override BilingualString Description => _description;

        public override bool IsCompleted()
        {
            // íEèoÇÃìÔà’ìxÇ™è„Ç™Ç¡ÇƒÇµÇ‹Ç§ÇÃÇ≈ÅA1ëÃì|ÇπÇŒè\ï™ÅB
            return _adventurer.Status.DefeatCount >= 1;
        }

        public override bool IsRetire()
        {
            return _adventurer.Status.ElapsedTurn > 100;
        }
    }
}
