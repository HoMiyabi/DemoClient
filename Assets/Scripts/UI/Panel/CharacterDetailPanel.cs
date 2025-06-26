using Kirara.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI.Panel
{
    public class CharacterDetailPanel : BasePanel
    {
        private UITabController UITabController;
        private Button UIBackBtn;
        private UICharacterBasicStat UICharacterBasicStat;
        private UICharacterEquipment UICharacterEquipment;

        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            UITabController = c.Q<UITabController>("UITabController");
            UIBackBtn = c.Q<Button>("UIBackBtn");
            UICharacterBasicStat = c.Q<UICharacterBasicStat>("UICharacterBasicStat");
            UICharacterEquipment = c.Q<UICharacterEquipment>("UICharacterEquipment");
        }

        private CharacterModel ch;

        private void Awake()
        {
            InitUI();
            UIBackBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));
        }

        public void Set(CharacterModel ch)
        {
            this.ch = ch;

            UICharacterBasicStat.Set(ch);
            UICharacterEquipment.Set(ch);
        }
    }
}