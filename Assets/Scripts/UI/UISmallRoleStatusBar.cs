using cfg.main;
using Kirara.Model;
using Manager;
using UnityEngine;
using YooAsset;

namespace Kirara.UI
{
    public class UISmallRoleStatusBar : MonoBehaviour
    {
        #region View
        private bool _isBound;
        private UnityEngine.UI.Image HpBar;
        private UnityEngine.UI.Image EnergyBar;
        private UnityEngine.UI.Image RoleIcon;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var c     = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            HpBar     = c.Q<UnityEngine.UI.Image>(0, "HpBar");
            EnergyBar = c.Q<UnityEngine.UI.Image>(1, "EnergyBar");
            RoleIcon  = c.Q<UnityEngine.UI.Image>(2, "RoleIcon");
        }
        #endregion

        public Color energyLackColor = Color.white;
        public Color energyEnoughColor = Color.white;

        private Role Role { get; set; }
        private AssetHandle handle;

        private void Awake()
        {
            BindUI();
        }

        private void OnDestroy()
        {
            Clear();
        }

        private void Clear()
        {
            handle?.Release();
            handle = null;
            Role = null;
        }

        public void Set(Role role)
        {
            Clear();
            Role = role;

            UpdateHP();
            UpdateEnergy();

            handle = AssetMgr.Instance.package.LoadAssetSync<Sprite>(role.Config.IconRoleGeneralLoc);
            RoleIcon.sprite = handle.AssetObject as Sprite;
        }

        private void UpdateHP()
        {
            double currHp = Role.Set[EAttrType.CurrHp];
            double maxHP = Role.Set[EAttrType.Hp];

            HpBar.fillAmount = (float)(currHp / maxHP);
        }

        private void UpdateEnergy()
        {
            double currEnergy = Role.Set[EAttrType.CurrEnergy];

            // todo)) 有点太脏了
            int actionId = Role.Config.Id * 100;
            var chNumeric = ConfigMgr.tb.TbChActionNumericConfig[actionId];

            float exSpecialEnergy = chNumeric.EnergyCost;
            EnergyBar.color = currEnergy < exSpecialEnergy ? energyLackColor : energyEnoughColor;

            double maxEnergy = ConfigMgr.tb.TbGlobalConfig.ChMaxEnergy;

            EnergyBar.fillAmount = (float)(currEnergy / maxEnergy);
        }

        public void Update()
        {
            if (Role == null) return;
            UpdateHP();
            UpdateEnergy();
        }
    }
}