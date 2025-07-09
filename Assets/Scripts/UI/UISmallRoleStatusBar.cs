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

        private Role role;
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
            if (role != null)
            {
                role.AttrSet.GetAttr(EAttrType.CurrHp).OnBaseValueChanged -= SetHP;
                role.AttrSet.GetAttr(EAttrType.CurrEnergy).OnBaseValueChanged -= SetEnergy;
            }
            handle?.Release();
            handle = null;
            role = null;
        }

        public void Set(Role role)
        {
            Clear();
            this.role = role;

            var currHpAttr = role.AttrSet.GetAttr(EAttrType.CurrHp);
            var currEnergyAttr = role.AttrSet.GetAttr(EAttrType.CurrEnergy);
            SetHP(currHpAttr.Evaluate());
            SetEnergy(currEnergyAttr.Evaluate());
            currHpAttr.OnBaseValueChanged += SetHP;
            currEnergyAttr.OnBaseValueChanged += SetEnergy;

            handle = AssetMgr.Instance.package.LoadAssetSync<Sprite>(role.config.IconLoc);
            CharacterIcon.sprite = handle.AssetObject as Sprite;
        }

        private void SetHP(double hp)
        {
            double maxHP = role.AttrSet[EAttrType.Hp];

            HPBar.fillAmount = (float)(hp / maxHP);
        }

        private void SetEnergy(float energy)
        {
            // todo)) 有点太脏了
            int actionId = role.config.Id * 100;
            var chNumeric = ConfigMgr.tb.TbChActionNumericConfig[actionId];

            float exSpecialEnergy = chNumeric.EnergyCost;
            EnergyBar.color = energy < exSpecialEnergy ? energyLackColor : energyEnoughColor;

            float maxEnergy = ConfigMgr.tb.TbGlobalConfig.ChMaxEnergy;

            EnergyBar.fillAmount = energy / maxEnergy;
        }

        private void SetDecibel(float decibel)
        {
            float maxDecibel = 3000;
            DecibelBar.fillAmount = decibel / maxDecibel;
        }
    }
}