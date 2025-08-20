using Manager;
using UnityEngine;

namespace Kirara.UI.Panel
{
    public class SettingsPanel : BasePanel
    {
        #region View
        private bool _isBound;
        private UnityEngine.UI.Button     UIBackBtn;
        private UnityEngine.RectTransform Content;
        public override void BindUI()
        {
            if (_isBound) return;
            _isBound = true;
            var b     = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            UIBackBtn = b.Q<UnityEngine.UI.Button>(0, "UIBackBtn");
            Content   = b.Q<UnityEngine.RectTransform>(1, "Content");
        }
        #endregion

        public GameObject SettingItemPrefab;

        private void Start()
        {
            UIBackBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));

            Content.DestroyChildren();

            Instantiate(SettingItemPrefab, Content).GetComponent<UISettingItem>()
                .Set("主音量",0, 10,
                    SettingsMgr.settings.MainVolume, value =>
                {
                    SettingsMgr.settings.MainVolume = value;
                    SettingsMgr.Save();
                    // AudioManger.Instance.masterVolume = value / 10f;
                });

            Instantiate(SettingItemPrefab, Content).GetComponent<UISettingItem>()
                .Set("音乐音量", 0, 10,
                    SettingsMgr.settings.MusicVolume, value =>
                {
                    SettingsMgr.settings.MusicVolume = value;
                    SettingsMgr.Save();
                    // AudioManger.Instance.sfxVolume = value / 10f;
                });

            Instantiate(SettingItemPrefab, Content).GetComponent<UISettingItem>()
                .Set("语音音量", 0, 10,
                    SettingsMgr.settings.DialogVolume, value =>
                {
                    SettingsMgr.settings.DialogVolume = value;
                    SettingsMgr.Save();
                    // AudioManger.Instance.sfxVolume = value / 10f;
                });

            Instantiate(SettingItemPrefab, Content).GetComponent<UISettingItem>()
                .Set("音效音量", 0, 10,
                    SettingsMgr.settings.SFXVolume, value =>
                {
                    SettingsMgr.settings.SFXVolume = value;
                    SettingsMgr.Save();
                    // AudioManger.Instance.sfxVolume = value / 10f;
                });
        }
    }
}