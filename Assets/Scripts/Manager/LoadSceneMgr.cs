using Kirara;
using Kirara.UI.Panel;
using UnityEngine;

namespace Manager
{
    public class LoadSceneMgr : UnitySingleton<LoadSceneMgr>
    {
        [SerializeField]
        private string LoadingSceneName = "LoadingScene";

        public void LoadScene(string sceneName)
        {
            UIMgr.Instance.PopAllPanel();
            UIMgr.Instance.AddTop<LoadingPanel>().Load(sceneName, LoadingSceneName);
        }
    }
}