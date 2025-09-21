using System.Text;
using cfg.main;
using Manager;
using UnityEngine;

namespace Kirara.UI
{
    public class UIBuffInfo : MonoBehaviour
    {
        #region View
        private bool _isBound;
        private TMPro.TextMeshProUGUI Text;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var b = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            Text  = b.Q<TMPro.TextMeshProUGUI>(0, "Text");
        }
        #endregion

        private readonly StringBuilder sb = new();

        private readonly EAttrType[] attrTypes = {
            EAttrType.Atk, EAttrType.CritRate, EAttrType.CritDmg, EAttrType.ZengShang};

        private void Awake()
        {
            BindUI();
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

            sb.AppendLine("增益：");
            foreach (var buff in set.Buffs)
            {
                sb.AppendFormat("{0}: {1}/{2}层, 剩余{3:F2}/{4:F2}秒\n",
                    buff.name, buff.stackCount, buff.stackLimit,
                    buff.GetMinRemainingTime(), buff.duration);
            }

            Text.SetText(sb);
        }

        private void Update()
        {
            UpdateEffectText();
        }
    }
}