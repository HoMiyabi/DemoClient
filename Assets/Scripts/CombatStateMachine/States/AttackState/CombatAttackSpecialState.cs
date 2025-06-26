/*using cfg.main;
using Manager;
using UnityEngine.InputSystem;

namespace Kirara
{
    public class CombatAttackSpecialState : CombatAttackState
    {
        private string actionName;

        private bool hasSwitchOutCommand;

        public override void ScheduleSwitchOut()
        {
            hasSwitchOutCommand = true;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            hasSwitchOutCommand = false;

            actionName = ActionName.Attack_Ex_Special;
            numeric = ConfigMgr.tb.TbCharacterNumericConfig.Get(sm.ch.characterId, actionName);
            var currEnergyAttr = ch.ChModel.ae.GetAttr(EAttrType.CurrEnergy);
            if (currEnergyAttr.Evaluate() >= numeric.EnergyCost)
            {
                currEnergyAttr.BaseValue -= numeric.EnergyCost;
            }
            else
            {
                actionName = ActionName.Attack_Special;
                numeric = ConfigMgr.tb.TbCharacterNumericConfig.Get(sm.ch.characterId, actionName);
            }

            ch.PlayAction(actionName, 0.15f, OnEnd);

            // 索敌面向敌人
            ch.LookAtMonster(numeric.DetectionDistance);
        }

        public override void InputActionStarted(InputAction.CallbackContext ctx)
        {
            base.InputActionStarted(ctx);
            if (ctx.action.id == PlayerSystem.Instance.input.Combat.SwitchCharactersPrevious.id)
            {
                PlayerSystem.Instance.SwitchCharacterPrev();
            }
            else if (ctx.action.id == PlayerSystem.Instance.input.Combat.SwitchCharactersNext.id)
            {
                PlayerSystem.Instance.SwitchCharacterNext();
            }
        }

        public override void OnToEnd()
        {
            base.OnToEnd();
            if (hasSwitchOutCommand)
            {
                sm.switchOut.Goto();
                return;
            }

            sm.end.Goto(null);
        }

        private void OnEnd()
        {
            if (hasSwitchOutCommand)
            {
                sm.switchOut.Goto();
                return;
            }

            string endSkillName = actionName + "_End";
            if (sm.ch.ActionCtrl.ActionDict.ContainsKey(endSkillName))
            {
                sm.end.Goto(endSkillName);
            }
            else
            {
                sm.idle.Goto();
            }
        }
    }
}*/