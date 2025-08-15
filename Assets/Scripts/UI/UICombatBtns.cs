using UnityEngine;

namespace Kirara.UI
{
    public class UICombatBtns : MonoBehaviour
    {
        #region View
        private bool _isBound;
        private UnityEngine.UI.Button SwitchBtn;
        private UnityEngine.UI.Button SpecialSkillBtn;
        private UnityEngine.UI.Button DodgeBtn;
        private UnityEngine.UI.Button AttackBtn;
        private UnityEngine.UI.Button UltimateBtn;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var c           = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            SwitchBtn       = c.Q<UnityEngine.UI.Button>(0, "SwitchBtn");
            SpecialSkillBtn = c.Q<UnityEngine.UI.Button>(1, "SpecialSkillBtn");
            DodgeBtn        = c.Q<UnityEngine.UI.Button>(2, "DodgeBtn");
            AttackBtn       = c.Q<UnityEngine.UI.Button>(3, "AttackBtn");
            UltimateBtn     = c.Q<UnityEngine.UI.Button>(4, "UltimateBtn");
        }
        #endregion

        private void Awake()
        {
            BindUI();
        }
    }
}