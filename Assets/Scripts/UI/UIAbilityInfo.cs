using System.Text;
using cfg.main;
using Manager;
using TMPro;
using UnityEngine;

namespace Kirara.UI
{
    public class UIAbilityInfo : MonoBehaviour
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
            var set = frontRole.Set;

            sb.AppendLine("属性：");
            foreach (var type in attrTypes)
            {
                sb.AppendFormat("{0}: {1:F4}\n", ConfigMgr.tb.TbAttrShowConfig[type].ShowName, set[type]);
            }

            sb.AppendLine("效果：");
            foreach (var ability in set.Abilities)
            {
                sb.AppendFormat("{0}: {1}/{2}层, 剩余{3:F2}/{4:F2}秒\n",
                    ability.name, ability.stackCount, ability.stackLimit,
                    ability.getMinRemainingTime(), ability.duration);
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