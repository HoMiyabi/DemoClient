using Kirara.Quest;
using Manager;
using UnityEngine;

namespace Kirara.UI.Panel
{
    public class QuestPanel : BasePanel
    {
        #region View
        private bool _isBound;
        private UnityEngine.UI.Button             UIBackBtn;
        private UnityEngine.UI.Button             TrackTargetBtn;
        private TMPro.TextMeshProUGUI             QuestChainNameText;
        private TMPro.TextMeshProUGUI             QuestNameText;
        private TMPro.TextMeshProUGUI             QuestDescText;
        private TMPro.TextMeshProUGUI             TrackTargetBtnText;
        private TMPro.TextMeshProUGUI             QuestProgressText;
        private Kirara.UI.UIQuestRewordBar        UIQuestRewordBar;
        private KiraraLoopScroll.LinearScrollView ScrollView;
        public override void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var c              = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            UIBackBtn          = c.Q<UnityEngine.UI.Button>(0, "UIBackBtn");
            TrackTargetBtn     = c.Q<UnityEngine.UI.Button>(1, "TrackTargetBtn");
            QuestChainNameText = c.Q<TMPro.TextMeshProUGUI>(2, "QuestChainNameText");
            QuestNameText      = c.Q<TMPro.TextMeshProUGUI>(3, "QuestNameText");
            QuestDescText      = c.Q<TMPro.TextMeshProUGUI>(4, "QuestDescText");
            TrackTargetBtnText = c.Q<TMPro.TextMeshProUGUI>(5, "TrackTargetBtnText");
            QuestProgressText  = c.Q<TMPro.TextMeshProUGUI>(6, "QuestProgressText");
            UIQuestRewordBar   = c.Q<Kirara.UI.UIQuestRewordBar>(7, "UIQuestRewordBar");
            ScrollView         = c.Q<KiraraLoopScroll.LinearScrollView>(8, "ScrollView");
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

        protected override void Awake()
        {
            base.Awake();

            QuestSystem.Instance.OnTrackingChainChanged += UpdateTrackTargetBtnView;

            UIBackBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));

            ScrollView.SetGOSource(new LoopScrollGOPool(UIQuestChainItemPrefab, transform));
            ScrollView.provideData = ProvideData;
            ScrollView._totalCount = QuestSystem.Instance.chains.Count;
        }

        private void Start()
        {
            InitSelected();
        }

        private void OnDestroy()
        {
            QuestSystem.Instance.OnTrackingChainChanged -= UpdateTrackTargetBtnView;
        }

        private void ProvideData(GameObject go, int index)
        {
            var item = go.GetComponent<UIQuestChainItem>();
            var chain = QuestSystem.Instance.chains[index];
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