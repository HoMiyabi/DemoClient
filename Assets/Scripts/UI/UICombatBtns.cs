using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI
{
    public class UICombatBtns : MonoBehaviour
    {
        #region View
        private Button SwitchBtn;
        private Button SpecialSkillBtn;
        private Button DodgeBtn;
        private Button AttackBtn;
        private Button UltimateBtn;
        private void InitUI()
        {
            var c           = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            SwitchBtn       = c.Q<Button>(0, "SwitchBtn");
            SpecialSkillBtn = c.Q<Button>(1, "SpecialSkillBtn");
            DodgeBtn        = c.Q<Button>(2, "DodgeBtn");
            AttackBtn       = c.Q<Button>(3, "AttackBtn");
            UltimateBtn     = c.Q<Button>(4, "UltimateBtn");
        }
        #endregion

        private void Awake()
        {
            InitUI();
        }
    }
}