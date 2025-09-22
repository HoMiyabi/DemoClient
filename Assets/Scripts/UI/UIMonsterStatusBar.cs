using System.Linq;
using cfg.main;
using Kirara.Model;
using Manager;
using UnityEngine;
using YooAsset;

namespace Kirara.UI
{
    public class UIMonsterStatusBar : MonoBehaviour
    {
        #region View
        private bool _isBound;
        private UnityEngine.UI.Image  HpBar;
        private UnityEngine.UI.Image  DazeBar;
        private UnityEngine.UI.Image  MonsterIcon;
        private TMPro.TextMeshProUGUI DazeText;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var c       = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            HpBar       = c.Q<UnityEngine.UI.Image>(0, "HpBar");
            DazeBar     = c.Q<UnityEngine.UI.Image>(1, "DazeBar");
            MonsterIcon = c.Q<UnityEngine.UI.Image>(2, "MonsterIcon");
            DazeText    = c.Q<TMPro.TextMeshProUGUI>(3, "DazeText");
        }
        #endregion

        private static readonly int Value = Shader.PropertyToID("_Value");

        private void Awake()
        {
            BindUI();
            HpBar.material = new Material(HpBar.material);
            DazeBar.material = new Material(DazeBar.material);
        }

        private MonsterModel Monster { get; set; }
        private int prevMonsterCid = -1;

        // public void Set(MonsterCtrl monster)
        // {
        //     Monster = monster;
        //
        //     UpdateHp();
        //     UpdateDaze();
        // }

        private void UpdateHp()
        {
            double currHp = Monster.Set[EAttrType.CurrHp];
            double maxHp = Monster.Set[EAttrType.Hp];
            HpBar.material.SetFloat(Value, (float)(currHp / maxHp));
        }

        private void UpdateDaze()
        {
            double currDaze = Monster.Set[EAttrType.CurrDaze];
            double maxDaze = Monster.Set[EAttrType.MaxDaze];

            double ratio = currDaze / maxDaze;
            DazeBar.material.SetFloat(Value, (float)ratio);

            DazeText.text = (ratio * 100).ToString("F0");
        }

        private void UpdateIcon()
        {
            if (Monster.Config.Id == prevMonsterCid) return;

            prevMonsterCid = Monster.Config.Id;
            var handle = YooAssets.LoadAssetSync<Sprite>(Monster.Config.IconLoc);
            MonsterIcon.sprite = handle.AssetObject as Sprite;
        }

        public void Update()
        {
            if (MonsterSystem.Instance.IdToMonsterCtrl.Count > 0)
            {
                var monsterCtrl = MonsterSystem.Instance.IdToMonsterCtrl.Values.First();
                Monster = monsterCtrl.Model;
            }
            else
            {
                Monster = null;
            }

            if (Monster != null)
            {
                transform.localScale = Vector3.one;
                UpdateHp();
                UpdateDaze();
                UpdateIcon();
            }
            else
            {
                transform.localScale = Vector3.zero;
            }


            // // 缓降
            // if (HPBar.fillAmount < delayHPBar.fillAmount)
            // {
            //     delayHPBar.fillAmount = Math.Max(HPBar.fillAmount,
            //         delayHPBar.fillAmount - delayHPBarSpeed * Time.deltaTime);
            // }
        }
    }
}