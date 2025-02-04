using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Experimental
{
    public static class Choice
    {
        public static IReadOnlyList<string> Get(Situation situation)
        {
            List<string> choices = new List<string>();

            choices.Add("ExploreThisRoom");
            choices.Add("MoveForward");

            return choices;
        }
    }
}
