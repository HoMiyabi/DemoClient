using System;
using System.Collections;
using Manager;
using UnityEngine;
using YooAsset;

namespace Kirara.UI.Panel
{
    public class LoadingPanel : BasePanel
    {
        #region View
        private bool _isBound;
        private UnityEngine.UI.Image  ProgressBarImg;
        private TMPro.TextMeshProUGUI ProgressText;
        public override void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var b          = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            ProgressBarImg = b.Q<UnityEngine.UI.Image>(0, "ProgressBarImg");
            ProgressText   = b.Q<TMPro.TextMeshProUGUI>(1, "ProgressText");
        }
        #endregion

        private float progress;

        public float Progress
        {
            get => progress;
            set
            {
                progress = value;
                ProgressBarImg.fillAmount = progress;
                ProgressText.text = progress.ToString("P0");
            }
        }

        protected override void Awake()
        {
            base.Awake();

            Progress = 0f;
        }

        public void Load(string sceneName, string loadingSceneName)
        {
            AssetMgr.Instance.package.LoadSceneSync(loadingSceneName);

            var handle = AssetMgr.Instance.package.LoadSceneAsync(sceneName);
            // handle.Completed += (h) =>
            // {
            //     Debug.Log("场景加载完成");
            //     UIManager.Instance.PopPanel(UILayer.Top);
            // };
            StartCoroutine(UpdateProgress(handle));
        }

        private IEnumerator UpdateProgress(SceneHandle handle)
        {
            while (!handle.IsDone)
            {
                if (Progress < 0.9f)
                {
                    Progress += Time.deltaTime;
                }
                yield return null;
            }

            while (Progress < 1f)
            {
                Progress += Time.deltaTime * 3f;
                yield return null;
            }
            Destroy(gameObject);
        }
        //
        // private IEnumerator GetProgress(SceneHandle handle)
        // {
        //     while (!handle.IsDone)
        //     {
        //         Debug.Log($"加载进度：{handle.Progress}");
        //         SetProgress(handle.Progress);
        //         yield return null;
        //     }
        // }
    }
}