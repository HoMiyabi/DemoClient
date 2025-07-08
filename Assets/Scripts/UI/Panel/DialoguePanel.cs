using System.Collections.Generic;
using cfg.main;
using Kirara.System;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI.Panel
{
    public class DialoguePanel : BasePanel
    {
        #region View
        private TextMeshProUGUI NameText;
        private RectTransform OptionParent;
        private Button Btn;
        private UIDialogueText UIDialogueText;
        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            NameText = c.Q<TextMeshProUGUI>(0, "NameText");
            OptionParent = c.Q<RectTransform>(1, "OptionParent");
            Btn = c.Q<Button>(2, "Btn");
            UIDialogueText = c.Q<UIDialogueText>(3, "UIDialogueText");
        }
        #endregion

        [SerializeField] private GameObject optionItemPrefab;
        private SimpleGOPool pool;

        private const int MinOptionsSubCid = 10001;
        private const int ExitSubCid = -1;

        private int dialogueCid;

        private DialogueGraphSentenceConfig graphSentenceConfig;
        private DialogueGraphOptionConfig graphOptionConfig;

        private int subCid;

        private DialogueSentenceConfig sentenceConfig;
        private DialogueOptionsConfig optionsConfig;

        private Dictionary<string, int> blackBoard;

        private void Awake()
        {
            InitUI();

            pool = new SimpleGOPool(optionItemPrefab, transform);
            pool.ReleaseChildren(OptionParent);

            Btn.onClick.AddListener(Btn_onClick);
        }

        private static bool IsExitSubCid(int subCid)
        {
            return subCid == ExitSubCid;
        }

        private static bool IsSentenceSubCid(int subCid)
        {
            return subCid < MinOptionsSubCid;
        }

        private void ExecuteCmd(string cmd, string arg1, string arg2, string arg3)
        {
            if (string.IsNullOrEmpty(cmd))
            {
                return;
            }
            switch (cmd)
            {
                case "push_panel":
                {
                    UIMgr.Instance.PushPanel(arg1);
                    break;
                }
                case "set":
                {
                    blackBoard[arg1] = int.Parse(arg2);
                    break;
                }
                default:
                {
                    Debug.LogWarning($"未知命令: {cmd}, 参数: {arg1}, {arg2}, {arg3}");
                    break;
                }
            }
        }

        private void To(int nextSubCid)
        {
            // Debug.Log($"dialogue to id={id}");
            if (IsExitSubCid(nextSubCid))
            {
                // 退出
                UIMgr.Instance.PopPanel(this);
                DialogueSystem.Instance.OnDialogueFinish?.Invoke(dialogueCid, blackBoard);
            }
            else if (IsSentenceSubCid(nextSubCid))
            {
                // 对话句子
                if (nextSubCid == 0)
                {
                    nextSubCid = subCid + 1;
                }

                sentenceConfig = graphSentenceConfig.Sentences.Find(
                    it => it.SubCid == nextSubCid);
                optionsConfig = null;

                NameText.text = sentenceConfig.Name;
                UIDialogueText.PlayType(sentenceConfig.Content);

                pool.ReleaseChildren(OptionParent);
            }
            else
            {
                // 对话选项

                sentenceConfig = null;
                optionsConfig = graphOptionConfig.OptionsList.Find(
                    it =>it.SubCid == nextSubCid);

                foreach (var option in optionsConfig.Options)
                {
                    var uiOptionItem = pool.Get<UIDialogueOptionItem>(OptionParent, false);
                    uiOptionItem.Set(option.Content, () => HandleOptionClick(option));
                }
            }
            subCid = nextSubCid;
        }

        private void HandleOptionClick(DialogueOptionConfig option)
        {
            ExecuteCmd(option.ClickCmd, option.Arg1, option.Arg2, option.Arg3);
            To(option.NextSubCid);
        }

        private void Btn_onClick()
        {
            if (sentenceConfig == null) return;

            if (UIDialogueText.IsPlaying)
            {
                UIDialogueText.ForceFinish();
            }
            else
            {
                ExecuteCmd(sentenceConfig.ExitCmd, sentenceConfig.Arg1, sentenceConfig.Arg2, sentenceConfig.Arg3);
                To(sentenceConfig.NextSubCid);
            }
        }

        private void OnDestroy()
        {
            DialogueSystem.Instance.DisableDialogueVCam();
        }

        public void Set(int dialogueCid, Transform a, Transform b)
        {
            this.dialogueCid = dialogueCid;
            blackBoard = new Dictionary<string, int>();

            graphSentenceConfig = ConfigMgr.tb.TbDialogueGraphSentenceConfig.GetOrDefault(dialogueCid);
            graphOptionConfig = ConfigMgr.tb.TbDialogueGraphOptionConfig.GetOrDefault(dialogueCid);

            To(1);

            DialogueSystem.Instance.EnableDialogueVCam(a, b);
        }
    }
}