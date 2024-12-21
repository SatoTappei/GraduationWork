using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class TalkPartnerRecord : MonoBehaviour
    {
        HashSet<string> _partners;

        public IReadOnlyCollection<string> Partners => _partners;

        void Awake()
        {
            _partners = new HashSet<string>();
        }

        public void Record(Adventurer adventurer)
        {
            if (adventurer == null) return;

            _partners.Add(adventurer.AdventurerSheet.FullName);
        }
    }
}
