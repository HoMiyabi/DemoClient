using System;
using System.IO;
using Manager;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI.Panel
{
    public class SettingsPanel : BasePanel
    {
        private Button UIBackBtn;
        private RectTransform Content;

        private void InitUI()
        {
            var c = GetComponent<KiraraRuntimeComponents>();
            c.Init();
            UIBackBtn = c.dict["UIBackBtn"] as Button;
            Content = c.dict["Content"] as RectTransform;
        }

        private const string settingItemName = "UISettingItem";

        private void Awake()
        {
            InitUI();

            UIBackBtn.onClick.AddListener(() => UIMgr.Instance.PopPanel(this));
        }

        private void Start()
        {
            Content.DestroyChildren();
            var handle = AssetMgr.Instance.package.LoadAssetSync<GameObject>("UISettingItem");

            handle.InstantiateSync(Content).GetComponent<UISettingItem>()
                .Set("主音量",0, 10,
                    SettingsMgr.settings.MainVolume, value =>
                {
                    SettingsMgr.settings.MainVolume = value;
                    SettingsMgr.Save();
                    // AudioManger.Instance.masterVolume = value / 10f;
                });

            handle.InstantiateSync(Content).GetComponent<UISettingItem>()
                .Set("音乐音量", 0, 10,
                    SettingsMgr.settings.MusicVolume, value =>
                {
                    SettingsMgr.settings.MusicVolume = value;
                    SettingsMgr.Save();
                    // AudioManger.Instance.sfxVolume = value / 10f;
                });

            handle.InstantiateSync(Content).GetComponent<UISettingItem>()
                .Set("语音音量", 0, 10,
                    SettingsMgr.settings.DialogVolume, value =>
                {
                    SettingsMgr.settings.DialogVolume = value;
                    SettingsMgr.Save();
                    // AudioManger.Instance.sfxVolume = value / 10f;
                });

            handle.InstantiateSync(Content).GetComponent<UISettingItem>()
                .Set("音效音量", 0, 10,
                    SettingsMgr.settings.SFXVolume, value =>
                {
                    SettingsMgr.settings.SFXVolume = value;
                    SettingsMgr.Save();
                    // AudioManger.Instance.sfxVolume = value / 10f;
                });

            handle.Release();
        }
    }
}