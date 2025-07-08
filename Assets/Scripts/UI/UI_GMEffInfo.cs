using System.Text;
using cfg.main;
using Manager;
using TMPro;
using UnityEngine;

namespace Kirara.UI
{
    public class UI_GMEffInfo : MonoBehaviour
    {
        #region View
        private TextMeshProUGUI Text;
        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            Text  = c.Q<TextMeshProUGUI>(0, "Text");
        }
        #endregion

        private readonly StringBuilder sb = new();
        private string cacheText = "";

        private readonly EAttrType[] attrTypes = {EAttrType.Atk, EAttrType.CritRate, EAttrType.CritDmg, EAttrType.Impact};

        private void Awake()
        {
            InitUI();
        }

        private void UpdateEffectText()
        {
            sb.Clear();

            var aet = PlayerSystem.Instance.FrontRoleCtrl.Role.ae;

            sb.AppendLine("属性：");
            foreach (var type in attrTypes)
            {
                float value = aet.GetAttr(type).Evaluate();
                sb.AppendFormat("{0}: {1}\n", ConfigMgr.tb.TbAttrShowConfig[type].ShowName, value);
            }

            sb.AppendLine("效果：");
            var effs = aet.effects.Values;
            foreach (var eff in effs)
            {
                sb.AppendFormat("{0}: {1}/{2}层\n", eff.name, eff.StackCount, eff.StackLimit);
            }
            string text = sb.ToString();
            if (text != cacheText)
            {
                Debug.Log($"{text.GetHashCode()} {text}");
                cacheText = text;
                Text.text = cacheText;
            }
        }

        private void Update()
        {
            UpdateEffectText();
        }
    }
}