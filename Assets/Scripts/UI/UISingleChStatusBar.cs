using System;
using cfg.main;
using Kirara.Model;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

namespace Kirara.UI
{
    public class UISingleChStatusBar : MonoBehaviour
    {
        #region View
        private Image CharacterIcon;
        private TextMeshProUGUI HPText;
        private TextMeshProUGUI MaxHPText;
        private Image HPBar;
        private Image EnergyBar;
        private Image DecibelBar;
        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            CharacterIcon = c.Q<Image>("CharacterIcon");
            HPText = c.Q<TextMeshProUGUI>("HPText");
            MaxHPText = c.Q<TextMeshProUGUI>("MaxHPText");
            HPBar = c.Q<Image>("HPBar");
            EnergyBar = c.Q<Image>("EnergyBar");
            DecibelBar = c.Q<Image>("DecibelBar");
        }
        #endregion

        public Color energyLackColor = Color.white;
        public Color energyEnoughColor = Color.white;

        private CharacterModel ch;
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
            if (ch != null)
            {
                ch.ae.GetAttr(EAttrType.CurrHp).OnBaseValueChanged -= SetHP;
                ch.ae.GetAttr(EAttrType.CurrEnergy).OnBaseValueChanged -= SetEnergy;
            }
            handle?.Release();
            handle = null;
            ch = null;
        }

        public void Set(CharacterModel ch)
        {
            Clear();
            this.ch = ch;

            var currHpAttr = ch.ae.GetAttr(EAttrType.CurrHp);
            var currEnergyAttr = ch.ae.GetAttr(EAttrType.CurrEnergy);
            SetHP(currHpAttr.Evaluate());
            SetEnergy(currEnergyAttr.Evaluate());
            currHpAttr.OnBaseValueChanged += SetHP;
            currEnergyAttr.OnBaseValueChanged += SetEnergy;

            handle = AssetMgr.Instance.package.LoadAssetSync<Sprite>(ch.config.IconLoc);
            CharacterIcon.sprite = handle.AssetObject as Sprite;
        }

        private void SetHP(float hp)
        {
            float maxHP = ch.ae.GetAttr(EAttrType.Hp).Evaluate();

            HPBar.fillAmount = hp / maxHP;

            if (HPText != null)
            {
                HPText.text = hp.ToString("F0");
            }
            if (MaxHPText != null)
            {
                MaxHPText.text = maxHP.ToString("F0");
            }
        }

        private void SetEnergy(float energy)
        {
            // todo)) 有点太脏了
            int actionId = ch.config.Id * 100;
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