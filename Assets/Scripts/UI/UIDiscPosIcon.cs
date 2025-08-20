using Manager;
using UnityEngine;
using YooAsset;

namespace Kirara.UI
{
    public class UIDiscPosIcon : MonoBehaviour
    {
        #region View
        private bool _isBound;
        private UnityEngine.UI.Image Img;
        public void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var c = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            Img   = c.Q<UnityEngine.UI.Image>(0, "Img");
        }
        #endregion

        private AssetHandle handle;

        private void OnDestroy()
        {
            Clear();
        }

        private void Clear()
        {
            handle?.Release();
            handle = null;
        }

        public void Set(int pos)
        {
            BindUI();
            if (pos is < 1 or > 6)
            {
                Debug.LogWarning($"pos {pos} is invalid");
                return;
            }
            Clear();
            handle = YooAssets.LoadAssetSync<Sprite>($"Pos{pos}Icon");
            Img.sprite = handle.AssetObject as Sprite;
        }
    }
}