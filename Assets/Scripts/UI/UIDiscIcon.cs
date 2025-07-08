using Manager;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

namespace Kirara.UI
{
    public class UIDiscIcon : MonoBehaviour
    {
        #region View
        private Image Img;
        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            Img   = c.Q<Image>(0, "Img");
        }
        #endregion

        private AssetHandle handle;

        private void Awake()
        {
            InitUI();
        }

        private void OnDestroy()
        {
            Clear();
        }

        private void Clear()
        {
            handle?.Release();
            handle = null;
        }

        public void Set(string loc)
        {
            Clear();
            handle = AssetMgr.Instance.package.LoadAssetSync<Sprite>(loc);
            Img.sprite = handle.AssetObject as Sprite;
        }
    }
}