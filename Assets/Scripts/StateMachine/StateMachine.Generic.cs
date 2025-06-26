using System;
using UnityEngine;

namespace Kirara
{
    public class StateMachine<TState> : MonoBehaviour where TState : IState
    {
        public TState state { get; private set; }

        public void ChangeState(TState newState)
        {
            state?.OnExit();
            state = newState;
            state?.OnEnter();
        }

        public void Update()
        {
            state?.Update();
        }
    }
}