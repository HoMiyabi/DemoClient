using cfg.main;
using Kirara.Model;
using Manager;
using UnityEngine;
using YooAsset;

namespace Kirara.UI
{
    public class UIBigRoleStatusBar : MonoBehaviour
    {
        private static readonly int Hp = Shader.PropertyToID("_Hp");

        #region View
        private bool _isBound;
        private UnityEngine.UI.Image  RoleIcon;
        private TMPro.TextMeshProUGUI CurrHpText;
        private TMPro.TextMeshProUGUI MaxHpText;
        private UnityEngine.UI.Image  HpBar;
        private UnityEngine.UI.Image  EnergyBar;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var c      = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            RoleIcon   = c.Q<UnityEngine.UI.Image>(0, "RoleIcon");
            CurrHpText = c.Q<TMPro.TextMeshProUGUI>(1, "CurrHpText");
            MaxHpText  = c.Q<TMPro.TextMeshProUGUI>(2, "MaxHpText");
            HpBar      = c.Q<UnityEngine.UI.Image>(3, "HpBar");
            EnergyBar  = c.Q<UnityEngine.UI.Image>(4, "EnergyBar");
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
            double maxHp = Role.Set[EAttrType.Hp];

            HpBar.material.SetFloat(Hp, (float)(currHp / maxHp));

            CurrHpText.text = currHp.ToString("F0");
            MaxHpText.text = maxHp.ToString("F0");
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

        private void Update()
        {
            if (Role == null) return;
            UpdateHP();
            UpdateEnergy();
        }
    }
}