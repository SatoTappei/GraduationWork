using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Experimental.FSM
{
    public class Result
    {
        public Result(State nextState, string selectedAction)
        {
            NextState = nextState;
            SelectedAction = selectedAction;
        }

        public State NextState { get; }
        public string SelectedAction { get; }
    }
}
