using System.Collections;
using UnityEngine;
using YooAsset;

namespace Kirara.UI.Panel
{
    public class LoadingPanel : BasePanel
    {
        #region View
        private bool _isBound;
        private Kirara.UI.UIProgressBar UIProgressBar;
        public override void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var b         = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            UIProgressBar = b.Q<Kirara.UI.UIProgressBar>(0, "UIProgressBar");
        }
        #endregion

        public void Load(string sceneName, string loadingSceneName)
        {
            YooAssets.LoadSceneSync(loadingSceneName);

            var handle = YooAssets.LoadSceneAsync(sceneName);
            // handle.Completed += (h) =>
            // {
            //     Debug.Log("场景加载完成");
            //     UIManager.Instance.PopPanel(UILayer.Top);
            // };
            UIProgressBar.Progress = 0f;
            StartCoroutine(UpdateProgress(handle));
        }

        private IEnumerator UpdateProgress(SceneHandle handle)
        {
            while (!handle.IsDone)
            {
                if (UIProgressBar.Progress < 0.9f)
                {
                    UIProgressBar.Progress += Time.deltaTime;
                }
                yield return null;
            }

            while (UIProgressBar.Progress < 1f)
            {
                UIProgressBar.Progress += Time.deltaTime;
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}