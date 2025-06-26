using Manager;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

namespace Kirara.UI
{
    public class UIDiscPosIcon : MonoBehaviour
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

        public void Set(int pos)
        {
            if (pos is < 1 or > 6)
            {
                Debug.LogWarning($"pos {pos} is invalid");
                return;
            }
            Clear();
            handle = AssetMgr.Instance.package.LoadAssetSync<Sprite>($"Pos{pos}Icon");
            Img.sprite = handle.AssetObject as Sprite;
        }
    }
}