using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kirara.UI
{
    public class UIDialogueOptionItem : MonoBehaviour
    {
        #region View
        private TextMeshProUGUI Text;
        private Button          Btn;
        private void InitUI()
        {
            var c = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            Text  = c.Q<TextMeshProUGUI>(0, "Text");
            Btn   = c.Q<Button>(1, "Btn");
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