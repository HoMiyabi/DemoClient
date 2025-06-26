/*using Manager;
using UnityEngine.InputSystem;

namespace Kirara
{
    public class CombatAttackRushState : CombatAttackState
    {
        private bool hasAtkCommand;
        private bool enableInputBuffer;
        private bool enableCombo;

        private bool hasSwitchOutCommand;

        public override void OnEnter()
        {
            base.OnEnter();

            hasAtkCommand = false;
            enableCombo = false;
            hasSwitchOutCommand = false;

            ch.PlayAction(ActionName.Attack_Rush, 0.15f, OnEnd);

            numeric = ConfigMgr.tb.TbCharacterNumericConfig.Get(sm.ch.characterId, ActionName.Attack_Rush);

            // 索敌面向敌人
            ch.LookAtMonster(numeric.DetectionDistance);
        }

        public override void ScheduleSwitchOut()
        {
            hasSwitchOutCommand = true;
        }

        private void OnEnd()
        {
            if (hasSwitchOutCommand)
            {
                sm.switchOut.Goto();
                return;
            }
            sm.end.Goto(ActionName.Attack_Rush_End);
        }

        public override void InputActionStarted(InputAction.CallbackContext ctx)
        {
            base.InputActionStarted(ctx);
            if (ctx.action.id == PlayerSystem.Instance.input.Combat.Dodge.id)
            {
                sm.evadeBack.Goto();
            }
            else if (ctx.action.id == PlayerSystem.Instance.input.Combat.SwitchCharactersPrevious.id)
            {
                PlayerSystem.Instance.SwitchCharacterPrev();
            }
            else if (ctx.action.id == PlayerSystem.Instance.input.Combat.SwitchCharactersNext.id)
            {
                PlayerSystem.Instance.SwitchCharacterNext();
            }
        }

        public override void OnEnableInputBuffer()
        {
            base.OnEnableInputBuffer();
            enableInputBuffer = true;
        }

        public override void OnEnableCombo()
        {
            base.OnEnableCombo();
            enableCombo = true;
        }
    }
}*/