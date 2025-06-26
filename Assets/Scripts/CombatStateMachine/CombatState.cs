/*namespace Kirara
{
    public abstract class CombatState : IState
    {
        public EActionState state;
        public abstract EActionPriority Priority { get; }
        protected CombatStateMachine sm;
        protected readonly ChCtrl ch;

        protected CombatState(CombatStateMachine sm, EActionState state)
        {
            this.sm = sm;
            ch = sm.ch;
            this.state = state;
        }

        public virtual void OnEnter()
        {
        }

        public virtual void OnExit()
        {
        }

        public virtual void Update()
        {
        }
    }
}*/