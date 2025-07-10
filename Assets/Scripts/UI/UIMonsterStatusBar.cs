using System;
using cfg.main;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI
{
    public class UIMonsterStatusBar : MonoBehaviour
    {
        public Image delayHPBar;
        public Image HPBar;
        public float delayHPBarSpeed = 0.2f;

        public Image dazeBar;
        public TextMeshProUGUI dazeText;

        private RectTransform rectTransform;

        private Transform wsFollow;
        private Monster Monster { get; set; }

        private void Awake()
        {
            rectTransform = transform as RectTransform;
        }

        public void Set(Monster monster)
        {
            Monster = monster;

            wsFollow = monster.statusBarFollow;

            monster.onDie += () => Destroy(gameObject);

            SetHpRatioImmediate();
            UpdateDazeRatio();
        }

        private void SetHpRatioImmediate()
        {
            double currHp = Monster.Model.AttrSet[EAttrType.CurrHp];
            double maxHp = Monster.Model.AttrSet[EAttrType.Hp];
            HPBar.fillAmount = (float)(currHp / maxHp);
            delayHPBar.fillAmount = (float)(currHp / maxHp);
        }

        public void UpdateHpRatio()
        {
            double currHp = Monster.Model.AttrSet[EAttrType.CurrHp];
            double maxHp = Monster.Model.AttrSet[EAttrType.Hp];
            HPBar.fillAmount = (float)(currHp / maxHp);
        }

        public void UpdateDazeRatio()
        {
            double currDaze = Monster.Model.AttrSet[EAttrType.CurrDaze];
            double maxDaze = Monster.Model.AttrSet[EAttrType.MaxDaze];

            double ratio = currDaze / maxDaze;
            dazeBar.fillAmount = (float)ratio;

            dazeText.text = (ratio * 100).ToString("F0");
        }

        public void Update()
        {
            if (!wsFollow)
            {
                Destroy(gameObject);
                return;
            }
            RectUtils.SetRectWorldPos(rectTransform, wsFollow.position);

            UpdateHpRatio();
            UpdateDazeRatio();

            // 缓降
            if (HPBar.fillAmount < delayHPBar.fillAmount)
            {
                delayHPBar.fillAmount = Math.Max(HPBar.fillAmount,
                    delayHPBar.fillAmount - delayHPBarSpeed * Time.deltaTime);
            }
        }
    }
}