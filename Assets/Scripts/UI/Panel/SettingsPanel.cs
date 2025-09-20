using Manager;
using UnityEngine;

namespace Kirara.UI.Panel
{
    public class SettingsPanel : BasePanel
    {
        #region View
        private bool _isBound;
        private UnityEngine.UI.Button             UIBackBtn;
        private KiraraLoopScroll.LinearScrollView ScrollView;
        public override void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var b      = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            UIBackBtn  = b.Q<UnityEngine.UI.Button>(0, "UIBackBtn");
            ScrollView = b.Q<KiraraLoopScroll.LinearScrollView>(1, "ScrollView");
        }
        #endregion

        public GameObject SettingItemPrefab;

        protected override void Awake()
        {
            base.Awake();
            UIBackBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));

            ScrollView._totalCount = 4;
            ScrollView.SetSource(new LoopScrollGOPool(SettingItemPrefab, transform));
            ScrollView.provideData = ProvideData;
        }

        private void ProvideData(GameObject go, int index)
        {
            var item = go.GetComponent<UISettingItem>();
            switch (index)
            {
                case 0:
                {
                    item.Set("主音量", 0, 10,
                        SettingsMgr.settings.MainVolume, value =>
                        {
                            SettingsMgr.settings.MainVolume = value;
                            SettingsMgr.Save();
                            // AudioManger.Instance.masterVolume = value / 10f;
                        });
                    break;
                }
                case 1:
                {
                    item                .Set("音乐音量", 0, 10,
                        SettingsMgr.settings.MusicVolume, value =>
                        {
                            SettingsMgr.settings.MusicVolume = value;
                            SettingsMgr.Save();
                            // AudioManger.Instance.sfxVolume = value / 10f;
                        });
                    break;
                }
                case 2:
                {
                    item.Set("语音音量", 0, 10,
                        SettingsMgr.settings.DialogVolume, value =>
                        {
                            SettingsMgr.settings.DialogVolume = value;
                            SettingsMgr.Save();
                            // AudioManger.Instance.sfxVolume = value / 10f;
                        });
                    break;
                }
                case 3:
                {
                    item.Set("音效音量", 0, 10,
                        SettingsMgr.settings.SFXVolume, value =>
                        {
                            SettingsMgr.settings.SFXVolume = value;
                            SettingsMgr.Save();
                            // AudioManger.Instance.sfxVolume = value / 10f;
                        });
                    break;
                }
                default:
                    Debug.LogWarning("index out of range");
                    break;
            }
        }
    }
}