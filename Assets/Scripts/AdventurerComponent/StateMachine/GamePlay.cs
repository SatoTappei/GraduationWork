using Cysharp.Threading.Tasks;
using Game.FSM;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game.Experimental
{
    public class GamePlay : MonoBehaviour
    {
        Dictionary<string, State> _states;
        State _currentState;

        void Awake()
        {
            _states = new Dictionary<string, State>
            {
                { nameof(IdleState), gameObject.AddComponent<IdleState>() }
            };

            _currentState = _states[nameof(IdleState)];
        }

        public void PreInitialize()
        {
            //
        }

        public async UniTask<string> RequestAsync(CancellationToken token)
        {
            Result result = await _currentState.UpdateAsync(token);
            _currentState = result.NextState;

            return result.SelectedAction;
        }
    }
}
