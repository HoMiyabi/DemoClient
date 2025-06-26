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
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            SwitchBtn = c.Q<Button>("SwitchBtn");
            SpecialSkillBtn = c.Q<Button>("SpecialSkillBtn");
            DodgeBtn = c.Q<Button>("DodgeBtn");
            AttackBtn = c.Q<Button>("AttackBtn");
            UltimateBtn = c.Q<Button>("UltimateBtn");
        }
        #endregion

        private void Awake()
        {
            InitUI();
        }
    }
}