using cfg.main;
using Kirara.Model;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

namespace Kirara.UI
{
    public class UIBigRoleStatusBar : MonoBehaviour
    {
        #region View
        private Image           CharacterIcon;
        private TextMeshProUGUI HPText;
        private TextMeshProUGUI MaxHPText;
        private Image           HPBar;
        private Image           EnergyBar;
        private Image           DecibelBar;
        private void InitUI()
        {
            var c         = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            CharacterIcon = c.Q<Image>(0, "CharacterIcon");
            HPText        = c.Q<TextMeshProUGUI>(1, "HPText");
            MaxHPText     = c.Q<TextMeshProUGUI>(2, "MaxHPText");
            HPBar         = c.Q<Image>(3, "HPBar");
            EnergyBar     = c.Q<Image>(4, "EnergyBar");
            DecibelBar    = c.Q<Image>(5, "DecibelBar");
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

            handle = AssetMgr.Instance.package.LoadAssetSync<Sprite>(role.Config.IconLoc);
            CharacterIcon.sprite = handle.AssetObject as Sprite;
        }

        private void UpdateHP()
        {
            double currHP = Role.Set[EAttrType.CurrHp];
            double maxHP = Role.Set[EAttrType.Hp];

            HPBar.fillAmount = (float)(currHP / maxHP);

            HPText.text = currHP.ToString("F0");
            MaxHPText.text = maxHP.ToString("F0");
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

        private void SetDecibel(float decibel)
        {
            float maxDecibel = 3000;
            DecibelBar.fillAmount = decibel / maxDecibel;
        }

        private void Update()
        {
            UpdateHP();
            UpdateEnergy();
        }
    }
}