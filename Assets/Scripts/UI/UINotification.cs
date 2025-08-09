using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Kirara.UI
{
    public class UINotification : MonoBehaviour
    {
        #region View
        private TextMeshProUGUI Text;
        private CanvasGroup     Group;
        private void InitUI()
        {
            var c = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            Text  = c.Q<TextMeshProUGUI>(0, "Text");
            Group = c.Q<CanvasGroup>(1, "Group");
        }
        #endregion

        public void Awake()
        {
            InitUI();
        }

        private float enterDuration = 0.3f;
        private float duration = 1.5f;
        private float exitDuration = 0.5f;

        private IEnumerator Start()
        {
            Group.alpha = 0.5f;
            yield return Group.DOFade(1f, enterDuration).WaitForCompletion();

            yield return new WaitForSeconds(duration);

            transform.DOMoveY(transform.position.y + 50, exitDuration);
            yield return Group.DOFade(0f, exitDuration);
            Destroy(gameObject);
        }

        public void Set(string text)
        {
            Text.text = text;
        }
    }
}