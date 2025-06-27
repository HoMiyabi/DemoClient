using UnityEngine;

namespace Kirara.UI
{
    public class UICharactersStatusBar : MonoBehaviour
    {
        #region View
        private UISingleChStatusBar UIBigSingleChStatusBar;
        private UISingleChStatusBar UISmallSingleChStatusBar;
        private UISingleChStatusBar UISmallSingleChStatusBar1;
        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            UIBigSingleChStatusBar = c.Q<UISingleChStatusBar>("UIBigSingleChStatusBar");
            UISmallSingleChStatusBar = c.Q<UISingleChStatusBar>("UISmallSingleChStatusBar");
            UISmallSingleChStatusBar1 = c.Q<UISingleChStatusBar>("UISmallSingleChStatusBar1");
        }
        #endregion

        private UISingleChStatusBar[] bars;

        private void Awake()
        {
            InitUI();
            bars = new[] { UIBigSingleChStatusBar, UISmallSingleChStatusBar, UISmallSingleChStatusBar1 };
        }

        private void OnEnable()
        {
            PlayerSystem.Instance.OnFrontRoleChanged += UpdateView;
        }

        private void OnDisable()
        {
            PlayerSystem.Instance.OnFrontRoleChanged -= UpdateView;
        }

        private void UpdateView()
        {
            int chIdx = PlayerSystem.Instance.FrontRoleIdx;
            for (int i = 0; i < bars.Length; i++)
            {
                if (i < PlayerSystem.Instance.RoleCtrls.Count)
                {
                    bars[i].gameObject.SetActive(true);
                    bars[i].Set(PlayerSystem.Instance.RoleCtrls[chIdx].RoleModel);
                    chIdx = PlayerSystem.Instance.GetNext(chIdx);
                }
                else
                {
                    bars[i].gameObject.SetActive(false);
                }
            }
        }
    }
}