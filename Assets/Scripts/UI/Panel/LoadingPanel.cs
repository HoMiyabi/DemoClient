using System;
using System.Collections;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

namespace Kirara.UI.Panel
{
    public class LoadingPanel : BasePanel
    {
        #region View
        private Image           ProgressBarImg;
        private TextMeshProUGUI ProgressText;
        private void InitUI()
        {
            var c          = GetComponent<KiraraRuntimeComponents>();
            ProgressBarImg = c.Q<Image>(0, "ProgressBarImg");
            ProgressText   = c.Q<TextMeshProUGUI>(1, "ProgressText");
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

        private void Awake()
        {
            InitUI();

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