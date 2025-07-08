using Kirara.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI.Panel
{
    public class CharacterDetailPanel : BasePanel
    {
        #region View
        private UITabController UITabController;
        private Button UIBackBtn;
        private UICharacterBasicStat UICharacterBasicStat;
        private UICharacterEquipment UICharacterEquipment;
        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            UITabController = c.Q<UITabController>(0, "UITabController");
            UIBackBtn = c.Q<Button>(1, "UIBackBtn");
            UICharacterBasicStat = c.Q<UICharacterBasicStat>(2, "UICharacterBasicStat");
            UICharacterEquipment = c.Q<UICharacterEquipment>(3, "UICharacterEquipment");
        }
        #endregion

        private Role ch;

        private void Awake()
        {
            InitUI();
            UIBackBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));
        }

        public void Set(Role ch)
        {
            this.ch = ch;

            UICharacterBasicStat.Set(ch);
            UICharacterEquipment.Set(ch);
        }
    }
}