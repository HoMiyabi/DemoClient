/*using System.Collections.Generic;

namespace Kirara
{
    public class CombatStateMachine : StateMachine<CombatState>
    {
        public ChCtrl ch { get; private set; }

        public Dictionary<EActionState, CombatState> States { get; set; } = new();

        private void Awake()
        {
            ch = GetComponent<ChCtrl>();

            InitState();
        }

        public void ChangeState(EActionState state)
        {
            base.ChangeState(States[state]);
        }

        private void InitState()
        {
            States[EActionState.End] = new CombatEndState(this, EActionState.End);
            States[EActionState.Idle] = new CombatIdleState(this, EActionState.Idle);
            States[EActionState.Walk] = new CombatWalkState(this, EActionState.Walk);
            States[EActionState.Run] = new CombatRunState(this, EActionState.Run);
            States[EActionState.Switch] = new CombatSwitchState(this, EActionState.Switch);
            States[EActionState.Attack_Normal] = new CombatAttackNormalState(this, EActionState.Attack_Normal);
            States[EActionState.Dodge_Back] = new CombatDodgeBackState(this, EActionState.Dodge_Back);
            States[EActionState.Dodge_Front] = new CombatDodgeFrontState(this, EActionState.Dodge_Front);
            States[EActionState.Attack_Special] = new CombatAttackSpecialState(this, EActionState.Attack_Special);
            States[EActionState.Attack_Ex_Special] = new CombatAttackExSpecialState(this, EActionState.Attack_Ex_Special);
            States[EActionState.Attack_ParryAid] = new CombatAttackParryAidState(this, EActionState.Attack_ParryAid);
            States[EActionState.Attack_Ultimate] = new CombatAttackUltimateState(this, EActionState.Attack_Ultimate);
        }
    }
}*/