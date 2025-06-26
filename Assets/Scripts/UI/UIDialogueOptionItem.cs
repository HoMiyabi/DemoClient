using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kirara.UI
{
    public class UIDialogueOptionItem : MonoBehaviour
    {
        private TextMeshProUGUI Text;
        private Button Btn;

        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            Text = c.Q<TextMeshProUGUI>("Text");
            Btn = c.Q<Button>("Btn");
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