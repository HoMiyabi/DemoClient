using System;
using UnityEngine;

namespace Kirara
{
    public class StateMachine : MonoBehaviour
    {
        public IState state { get; private set; }

        public void ChangeState(IState newState)
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