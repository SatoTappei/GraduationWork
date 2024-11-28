using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class PlacedActors
    {
        Dictionary<string, List<Actor>> _value;

        public PlacedActors()
        {
            _value = new Dictionary<string, List<Actor>>();
        }

        public IReadOnlyList<Actor> this[string id]
        {
            get => _value.ContainsKey(id) ? _value[id] : null;
        }

        public void Add(Actor actor)
        {
            if (!_value.ContainsKey(actor.ID))
            {
                _value.Add(actor.ID, new List<Actor>());
            }

            if (_value[actor.ID].Contains(actor))
            {
                Debug.LogWarning($"{actor.ID}ÇÕä˘Ç…îzíuçœÇ›ÅB");
            }
            else _value[actor.ID].Add(actor);
        }

        public void Remove(Actor actor)
        {
            if (_value.TryGetValue(actor.ID, out List<Actor> actors))
            {
                actors.Remove(actor);
            }
        }
    }
}