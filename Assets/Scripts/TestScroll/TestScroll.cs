using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.TestScroll
{
    public class TestScroll : MonoBehaviour
    {
        #region View
        private bool _isBound;
        private Kirara.UI.LinearScroller  Scroller;
        private UnityEngine.UI.RectMask2D Mask;
        private UnityEngine.UI.Toggle     EnableMask;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var c      = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            Scroller   = c.Q<Kirara.UI.LinearScroller>(0, "Scroller");
            Mask       = c.Q<UnityEngine.UI.RectMask2D>(1, "Mask");
            EnableMask = c.Q<UnityEngine.UI.Toggle>(2, "EnableMask");
        }
        #endregion

        public GameObject Prefab;

        private void Awake()
        {
            BindUI();

            var pool = new LoopScrollGOPool(Prefab, transform);

            Scroller.SetPoolFunc(pool.GetObject, pool.ReturnObject);
            Scroller.provideData = ProvideData;

            Scroller.totalCount = 3;

            EnableMask.onValueChanged.AddListener((value) =>
            {
                Mask.enabled = value;
            });
            Mask.enabled = EnableMask.isOn;
        }

        private void ProvideData(GameObject go, int index)
        {
            var layout = go.GetComponent<LayoutElement>();
            var text = go.GetComponentInChildren<TextMeshProUGUI>();
            layout.preferredHeight = Random.Range(50, 150);
            text.text = $"index: {index}\nheight: {layout.preferredHeight}";
            go.name = $"index: {index} height: {layout.preferredHeight}";
        }
    }
}