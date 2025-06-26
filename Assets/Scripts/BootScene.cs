using Kirara.UI.Panel;
using UnityEngine;
namespace Kirara
{
    public class BootScene : MonoBehaviour
    {
        public GameObject BootPanelPrefab;

        private void Start()
        {
            UIMgr.Instance.PushPanel<BootPanel>(BootPanelPrefab);
        }
    }
}