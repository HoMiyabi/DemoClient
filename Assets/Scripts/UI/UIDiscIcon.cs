using cfg.main;
using Kirara.Model;
using Manager;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

namespace Kirara.UI
{
    public class UIDiscIcon : MonoBehaviour
    {
        private Image Img;

        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            Img = c.Q<Image>("Img");
        }

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