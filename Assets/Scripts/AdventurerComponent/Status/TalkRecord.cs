using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class TalkRecord
    {
        HashSet<string> _record;

        public TalkRecord()
        {
            _record = new HashSet<string>();
        }

        public IReadOnlyCollection<string> Record => _record;

        public void Add(Adventurer adventurer)
        {
            if (adventurer == null)
            {
                Debug.LogWarning("会話相手を記録しようとしたがnullだった。");
            }
            else
            {
                _record.Add(adventurer.AdventurerSheet.FullName);
            }
        }
    }
}
