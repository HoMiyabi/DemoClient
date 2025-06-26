using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kirara.UI
{
    public class UIQuestChainItem : MonoBehaviour
    {
        private Button Btn;
        private TextMeshProUGUI Text;

        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            Btn = c.Q<Button>("Btn");
            Text = c.Q<TextMeshProUGUI>("Text");
        }

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