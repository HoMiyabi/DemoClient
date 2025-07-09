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

        private void Awake()
        {
            rectTransform = transform as RectTransform;
        }

        public void Set(Monster monster)
        {
            wsFollow = monster.statusBarFollow;

            monster.onDie += () => Destroy(gameObject);

            var currHp = monster.Model.AttrSet[EAttrType.CurrHp];
            var hp = monster.Model.AttrSet[EAttrType.Hp];
            SetHpRatioImmediate(currHp / hp);

            var daze = monster.Model.AttrSet[EAttrType.CurrDaze];
            var maxDaze = monster.Model.AttrSet[EAttrType.MaxDaze];
            SetDazeRatio(daze / maxDaze);

            monster.Model.AttrSet.GetAttr(EAttrType.CurrHp).OnBaseValueChanged += value =>
            {
                SetHpRatio(value / monster.Model.AttrSet[EAttrType.Hp]);
            };
            monster.Model.AttrSet.GetAttr(EAttrType.CurrDaze).OnBaseValueChanged += value =>
            {
                SetDazeRatio(value / monster.Model.AttrSet[EAttrType.MaxDaze]);
            };
        }

        private void SetHpRatioImmediate(double ratio)
        {
            HPBar.fillAmount = (float)ratio;
            delayHPBar.fillAmount = (float)ratio;
        }

        public void SetHpRatio(double ratio)
        {
            HPBar.fillAmount = (float)ratio;
        }

        public void SetDazeRatio(double ratio)
        {
            dazeBar.fillAmount = (float)ratio;

            dazeText.text = (ratio * 100).ToString("F0");
        }

        public void Update()
        {
            if (wsFollow == null)
            {
                Destroy(gameObject);
                return;
            }
            RectUtils.SetRectWorldPos(rectTransform, wsFollow.position);

            // 缓降
            if (HPBar.fillAmount < delayHPBar.fillAmount)
            {
                delayHPBar.fillAmount = Math.Max(HPBar.fillAmount,
                    delayHPBar.fillAmount - delayHPBarSpeed * Time.deltaTime);
            }
        }
    }
}