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

            var frontRole = PlayerSystem.Instance.FrontRoleCtrl.Role;
            var attrSet = frontRole.AttrSet;

            sb.AppendLine("属性：");
            foreach (var type in attrTypes)
            {
                sb.AppendFormat("{0}: {1}\n", ConfigMgr.tb.TbAttrShowConfig[type].ShowName, attrSet[type]);
            }

            var abilitySet = frontRole.AbilitySet;

            sb.AppendLine("效果：");
            var abilities = abilitySet.Abilities;
            foreach (var ability in abilities.Values)
            {
                sb.AppendFormat("{0}: {1}/{2}层\n", ability.name, ability.stackCount, ability.stackLimit);
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