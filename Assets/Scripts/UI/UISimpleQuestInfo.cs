using System;
using Kirara.Quest;
using Manager;
using TMPro;
using UnityEngine;

namespace Kirara.UI
{
    public class UISimpleQuestInfo : MonoBehaviour
    {
        private TextMeshProUGUI QuestChainText;
        private TextMeshProUGUI QuestText;

        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            QuestChainText = c.Q<TextMeshProUGUI>("QuestChainText");
            QuestText = c.Q<TextMeshProUGUI>("QuestText");
        }

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