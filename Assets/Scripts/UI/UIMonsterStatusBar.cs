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

            var currHpAttr = monster.ae.GetAttr(EAttrType.CurrHp);
            var hpAttr = monster.ae.GetAttr(EAttrType.Hp);
            SetHpRatioImmediate(currHpAttr.Evaluate() / hpAttr.Evaluate());

            var dazeAttr = monster.ae.GetAttr(EAttrType.CurrDaze);
            var maxDazeAttr = monster.ae.GetAttr(EAttrType.MaxDaze);
            SetDazeRatio(dazeAttr.Evaluate() / maxDazeAttr.Evaluate());

            currHpAttr.OnBaseValueChanged += value =>
            {
                SetHpRatio(value / hpAttr.Evaluate());
            };
            dazeAttr.OnBaseValueChanged += value =>
            {
                SetDazeRatio(value / maxDazeAttr.Evaluate());
            };
        }

        private void SetHpRatioImmediate(float ratio)
        {
            HPBar.fillAmount = ratio;
            delayHPBar.fillAmount = ratio;
        }

        public void SetHpRatio(float ratio)
        {
            HPBar.fillAmount = ratio;
        }

        public void SetDazeRatio(float ratio)
        {
            dazeBar.fillAmount = ratio;

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