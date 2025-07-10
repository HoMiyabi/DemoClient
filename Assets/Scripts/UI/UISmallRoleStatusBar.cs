using cfg.main;
using Kirara.Model;
using Manager;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

namespace Kirara.UI
{
    public class UISmallRoleStatusBar : MonoBehaviour
    {
        #region View
        private Image HPBar;
        private Image DecibelBar;
        private Image EnergyBar;
        private Image CharacterIcon;
        private void InitUI()
        {
            var c         = GetComponent<KiraraRuntimeComponents>();
            HPBar         = c.Q<Image>(0, "HPBar");
            DecibelBar    = c.Q<Image>(1, "DecibelBar");
            EnergyBar     = c.Q<Image>(2, "EnergyBar");
            CharacterIcon = c.Q<Image>(3, "CharacterIcon");
        }
        #endregion

        public Color energyLackColor = Color.white;
        public Color energyEnoughColor = Color.white;

        private Role Role { get; set; }
        private AssetHandle handle;

        private void Awake()
        {
            InitUI();
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

            handle = AssetMgr.Instance.package.LoadAssetSync<Sprite>(role.config.IconLoc);
            CharacterIcon.sprite = handle.AssetObject as Sprite;
        }

        private void UpdateHP()
        {
            double currHP = Role.AttrSet[EAttrType.CurrHp];
            double maxHP = Role.AttrSet[EAttrType.Hp];

            HPBar.fillAmount = (float)(currHP / maxHP);
        }

        private void UpdateEnergy()
        {
            double currEnergy = Role.AttrSet[EAttrType.CurrEnergy];

            // todo)) 有点太脏了
            int actionId = Role.config.Id * 100;
            var chNumeric = ConfigMgr.tb.TbChActionNumericConfig[actionId];

            float exSpecialEnergy = chNumeric.EnergyCost;
            EnergyBar.color = currEnergy < exSpecialEnergy ? energyLackColor : energyEnoughColor;

            double maxEnergy = ConfigMgr.tb.TbGlobalConfig.ChMaxEnergy;

            EnergyBar.fillAmount = (float)(currEnergy / maxEnergy);
        }

        private void SetDecibel(float decibel)
        {
            float maxDecibel = 3000;
            DecibelBar.fillAmount = decibel / maxDecibel;
        }
    }
}