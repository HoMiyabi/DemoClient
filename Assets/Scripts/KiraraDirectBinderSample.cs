using System;
using UnityEngine;

namespace Kirara
{
    public class KiraraDirectBinderSample : MonoBehaviour
    {
        #region View
        private bool _isBound;
        private UnityEngine.UI.Button IncBtn;
        private UnityEngine.UI.Button DecBtn;
        private UnityEngine.UI.Button SignUpBtn;
        private UnityEngine.UI.Button SignInBtn;
        private UnityEngine.UI.Button QuitBtn;
        private TMPro.TextMeshProUGUI TitleText;
        private TMPro.TextMeshProUGUI ContentText;
        private TMPro.TextMeshProUGUI NameText;
        private TMPro.TextMeshProUGUI SignatureText;
        private TMPro.TextMeshProUGUI KiraraText;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var b         = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            IncBtn        = b.Q<UnityEngine.UI.Button>(0, "IncBtn");
            DecBtn        = b.Q<UnityEngine.UI.Button>(1, "DecBtn");
            SignUpBtn     = b.Q<UnityEngine.UI.Button>(2, "SignUpBtn");
            SignInBtn     = b.Q<UnityEngine.UI.Button>(3, "SignInBtn");
            QuitBtn       = b.Q<UnityEngine.UI.Button>(4, "QuitBtn");
            TitleText     = b.Q<TMPro.TextMeshProUGUI>(5, "TitleText");
            ContentText   = b.Q<TMPro.TextMeshProUGUI>(6, "ContentText");
            NameText      = b.Q<TMPro.TextMeshProUGUI>(7, "NameText");
            SignatureText = b.Q<TMPro.TextMeshProUGUI>(8, "SignatureText");
            KiraraText    = b.Q<TMPro.TextMeshProUGUI>(9, "KiraraText");
        }
        #endregion

        private void Awake()
        {
            TitleText.text = "Kirara Direct Binder Sample";
        }
    }
}