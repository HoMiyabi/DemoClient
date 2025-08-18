using TMPro;
using UnityEngine;

namespace Kirara.TestScroll
{
    public class TestGridScrollView : MonoBehaviour
    {
        #region View
        private bool _isBound;
        private KiraraLoopScroll.GridScrollView GridScrollView;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var c          = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            GridScrollView = c.Q<KiraraLoopScroll.GridScrollView>(0, "GridScrollView");
        }
        #endregion

        [SerializeField] private GameObject ItemPrefab;

        private void Awake()
        {
            BindUI();

            GridScrollView.SetSource(new LoopScrollGOPool(ItemPrefab, transform));
            GridScrollView.provideData = ProvideData;
            GridScrollView._totalCount = 100;
        }

        private void ProvideData(GameObject go, int index)
        {
            var text = go.GetComponentInChildren<TextMeshProUGUI>();
            text.text = $"index: {index}";
            go.name = $"index: {index}";
        }
    }
}