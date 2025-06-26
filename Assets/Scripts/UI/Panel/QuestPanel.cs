using System;
using Kirara.Quest;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI.Panel
{
    public class QuestPanel : BasePanel
    {
        #region View
        private Button UIBackBtn;
        private Button TrackTargetBtn;
        private TextMeshProUGUI QuestChainNameText;
        private TextMeshProUGUI QuestNameText;
        private TextMeshProUGUI QuestDescText;
        private TextMeshProUGUI TrackTargetBtnText;
        private TextMeshProUGUI QuestProgressText;
        private UIQuestRewordBar UIQuestRewordBar;
        private LoopVerticalScrollRect LoopScroll;
        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            UIBackBtn = c.Q<Button>("UIBackBtn");
            TrackTargetBtn = c.Q<Button>("TrackTargetBtn");
            QuestChainNameText = c.Q<TextMeshProUGUI>("QuestChainNameText");
            QuestNameText = c.Q<TextMeshProUGUI>("QuestNameText");
            QuestDescText = c.Q<TextMeshProUGUI>("QuestDescText");
            TrackTargetBtnText = c.Q<TextMeshProUGUI>("TrackTargetBtnText");
            QuestProgressText = c.Q<TextMeshProUGUI>("QuestProgressText");
            UIQuestRewordBar = c.Q<UIQuestRewordBar>("UIQuestRewordBar");
            LoopScroll = c.Q<LoopVerticalScrollRect>("LoopScroll");
        }
        #endregion

        [SerializeField] private GameObject UIQuestChainItemPrefab;

        private QuestChain selectedQuestChain;
        public QuestChain SelectedQuestChain
        {
            get => selectedQuestChain;
            set
            {
                selectedQuestChain = value;
                UpdateTrackTargetBtnView();
                UpdateQuestChainInfoView();
                UIQuestRewordBar.Set(selectedQuestChain.Rewords);
            }
        }

        private void Awake()
        {
            InitUI();

            QuestSystem.Instance.OnTrackingChainChanged += UpdateTrackTargetBtnView;

            UIBackBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));

            LoopScroll.prefabSource = new LoopScrollPool(UIQuestChainItemPrefab, transform);
            LoopScroll.dataSource = new LoopScrollDataSourceHandler(ProvideData);
        }

        private void Start()
        {
            LoopScroll.totalCount = QuestSystem.Instance.chains.Count;
            LoopScroll.RefillCells();
            InitSelected();
        }

        private void OnDestroy()
        {
            QuestSystem.Instance.OnTrackingChainChanged -= UpdateTrackTargetBtnView;
        }

        private void ProvideData(Transform tra, int idx)
        {
            var item = tra.GetComponent<UIQuestChainItem>();
            var chain = QuestSystem.Instance.chains[idx];
            item.Set(chain.Name, () => SelectedQuestChain = chain);
        }

        private void InitSelected()
        {
            if (QuestSystem.Instance.TrackingChain != null)
            {
                SelectedQuestChain = QuestSystem.Instance.TrackingChain;
            }
            else if (QuestSystem.Instance.chains.Count > 0)
            {
                SelectedQuestChain = QuestSystem.Instance.chains[0];
            }
            else
            {
                SelectedQuestChain = null;
            }
        }

        private void StopTrackingQuestChain()
        {
            QuestSystem.Instance.TrackingChain = null;
        }

        private void TrackSelectedQuestChain()
        {
            QuestSystem.Instance.TrackingChain = SelectedQuestChain;
        }

        private void UpdateTrackTargetBtnView()
        {
            if (SelectedQuestChain == null)
            {
                TrackTargetBtn.onClick.RemoveAllListeners();
                TrackTargetBtn.interactable = false;
                TrackTargetBtnText.text = "追踪目标";
            }
            else if (SelectedQuestChain == QuestSystem.Instance.TrackingChain)
            {
                TrackTargetBtn.onClick.RemoveAllListeners();
                TrackTargetBtn.onClick.AddListener(StopTrackingQuestChain);
                TrackTargetBtn.interactable = true;
                TrackTargetBtnText.text = "停止追踪";
            }
            else
            {
                TrackTargetBtn.onClick.RemoveAllListeners();
                TrackTargetBtn.onClick.AddListener(TrackSelectedQuestChain);
                TrackTargetBtn.interactable = true;
                TrackTargetBtnText.text = "追踪目标";
            }
        }

        private void UpdateQuestChainInfoView()
        {
            if (SelectedQuestChain == null)
            {
                QuestChainNameText.text = "当前没有任务";
                QuestNameText.text = "";
                QuestDescText.text = "";
                return;
            }

            QuestChainNameText.text = SelectedQuestChain.Name;

            var currentQuest = SelectedQuestChain.Quest;
            QuestNameText.text = currentQuest.Name;
            QuestDescText.text = currentQuest.Desc;
            if (currentQuest is ProgressQuest progressQuest)
            {
                QuestProgressText.gameObject.SetActive(true);
                QuestProgressText.text = $"进度 {progressQuest.Progress}/{progressQuest.Count}";
            }
            else
            {
                QuestProgressText.gameObject.SetActive(false);
            }
        }
    }
}