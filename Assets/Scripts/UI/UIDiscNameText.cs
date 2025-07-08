using TMPro;
using UnityEngine;

namespace Kirara.UI
{
    public class UIDiscNameText : MonoBehaviour
    {
        #region View
        private TextMeshProUGUI Text;
        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            Text  = c.Q<TextMeshProUGUI>(0, "Text");
        }
        #endregion

        private void Awake()
        {
            InitUI();
        }

        public void Set(string _name, int pos)
        {
            Text.text = $"{_name}[{pos}]";
        }
    }
}