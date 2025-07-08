using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kirara.UI
{
    public class UIQuestChainItem : MonoBehaviour
    {
        #region View
        private Button          Btn;
        private TextMeshProUGUI Text;
        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            Btn   = c.Q<Button>(0, "Btn");
            Text  = c.Q<TextMeshProUGUI>(1, "Text");
        }
        #endregion

        private void Awake()
        {
            InitUI();
        }

        public void Set(string text, UnityAction onClick)
        {
            Text.text = text;
            Btn.onClick.RemoveAllListeners();
            Btn.onClick.AddListener(onClick);
        }
    }
}