using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.TestScroll
{
    public class TestScroll : MonoBehaviour
    {
        #region View
        private bool _isBound;
        private Kirara.UI.LinearScroller Scroller;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var c    = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            Scroller = c.Q<Kirara.UI.LinearScroller>(0, "Scroller");
        }
        #endregion

        public GameObject Prefab;

        private void Awake()
        {
            BindUI();

            var pool = new LoopScrollGOPool(Prefab, transform);

            Scroller.SetPoolFunc(pool.GetObject, pool.ReturnObject);
            Scroller.provideData = ProvideData;

            Scroller.totalCount = 100;
        }

        private void ProvideData(GameObject go, int index)
        {
            var layout = go.GetComponent<LayoutElement>();
            var text = go.GetComponentInChildren<TextMeshProUGUI>();
            layout.preferredHeight = Random.Range(50, 150);
            text.text = $"index: {index}\nheight: {layout.preferredHeight}";
        }
    }
}