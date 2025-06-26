using TMPro;
using UnityEngine;

namespace Kirara.UI
{
    public class UIDiscNameText : MonoBehaviour
    {
        private TextMeshProUGUI Text;

        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            Text = c.Q<TextMeshProUGUI>("Text");
        }

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