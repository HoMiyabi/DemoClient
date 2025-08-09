using System;
using Kirara.Quest;
using Manager;
using TMPro;
using UnityEngine;

namespace Kirara.UI
{
    public class UISimpleQuestInfo : MonoBehaviour
    {
        #region View
        private TextMeshProUGUI QuestChainText;
        private TextMeshProUGUI QuestText;
        private void InitUI()
        {
            var c          = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            QuestChainText = c.Q<TextMeshProUGUI>(0, "QuestChainText");
            QuestText      = c.Q<TextMeshProUGUI>(1, "QuestText");
        }
        #endregion

        private void Awake()
        {
            InitUI();
        }

        private void UpdateView()
        {
            var chain = QuestSystem.Instance.TrackingChain;
            if (chain != null && chain.Quest != null)
            {
                QuestChainText.text = chain.Name;
                var quest = chain.Quest;
                string text = quest.Name;
                if (quest is ProgressQuest progressQuest)
                {
                    text += $" {progressQuest.Progress}/{progressQuest.Count}";
                }
                QuestText.text = text;
            }
            else
            {
                QuestChainText.text = "";
                QuestText.text = "";
            }
        }

        private void Update()
        {
            UpdateView();
        }
    }
}