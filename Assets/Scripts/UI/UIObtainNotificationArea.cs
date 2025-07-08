using System.Collections.Generic;
using cfg.main;
using DG.Tweening;
using Kirara.Model;
using Kirara.Service;
using Manager;
using TMPro;
using UnityEngine;

namespace Kirara.UI
{
    public class UIObtainNotificationArea : MonoBehaviour
    {
        #region View
        private RectTransform   NotificationsParent;
        private TextMeshProUGUI TitleText;
        private void InitUI()
        {
            var c               = GetComponent<KiraraRuntimeComponents>();
            NotificationsParent = c.Q<RectTransform>(0, "NotificationsParent");
            TitleText           = c.Q<TextMeshProUGUI>(1, "TitleText");
        }
        #endregion

        [SerializeField] private GameObject UIObtainNotificationPrefab;
        [SerializeField] private float spacing = 5;
        [SerializeField] private float height = 45;
        [SerializeField] private float titleFadeTime = 0.2f;
        [SerializeField] private float notifFloatingSpeed = 100f;

        private SimpleGOPool pool;
        private readonly Queue<UIObtainNotification> notifQue = new();
        private UIObtainNotification lastNotif;

        private void Awake()
        {
            InitUI();

            pool = new SimpleGOPool(UIObtainNotificationPrefab, transform);
            pool.ReleaseChildren(NotificationsParent);

            UpdateTitleImmed();

            AddCallbacks();
        }

        private void OnDestroy()
        {
            RemoveCallbacks();
        }

        private void AddCallbacks()
        {
            InventoryService.OnObtainItem += OnObtainItem;
        }

        private void RemoveCallbacks()
        {
            InventoryService.OnObtainItem -= OnObtainItem;
        }

        private void OnObtainItem(BaseItem item, int count)
        {
            Add(item.IconLoc, item.Name, count);
        }

        private void UpdateTitleImmed()
        {
            TitleText.alpha = notifQue.Count == 0f ? 0f : 1f;
        }

        private void UpdateTitle()
        {
            if (notifQue.Count == 0)
            {
                TitleText.DOKill();
                TitleText.DOFade(0f, titleFadeTime);
            }
            else if (notifQue.Count == 1)
            {
                TitleText.DOKill();
                TitleText.DOFade(1f, titleFadeTime);
            }
        }

        public void Add(string iconLoc, string name_, int count)
        {
            var notif = pool.Get<UIObtainNotification>(NotificationsParent, false);
            var rect = (RectTransform)notif.transform;
            rect.anchoredPosition = new Vector2(0, GetLastFollowPosY());

            notifQue.Enqueue(notif);
            lastNotif = notif;

            notif.Set(iconLoc, name_, count);
            notif.Play(OnNotifPlayFinished);

            UpdateTitle();
        }

        private void OnNotifPlayFinished(UIObtainNotification notif)
        {
            if (notifQue.TryPeek(out var notifInQue) || notifInQue  == notif)
            {
                notifQue.Dequeue();
                if (notifQue.Count == 0)
                {
                    lastNotif = null;
                }
            }
            else
            {
                Debug.LogWarning("Notif动画结束，但不在队列头部");
            }
            pool.Release(notif.gameObject);

            UpdateTitle();
        }

        private float GetTargetPosY(int idx)
        {
            return -(height + spacing) * idx;
        }

        private float GetLastFollowPosY()
        {
            if (lastNotif != null)
            {
                var rect = (RectTransform)lastNotif.transform;
                return rect.anchoredPosition.y - (height + spacing);
            }
            return GetTargetPosY(0);
        }

        private void UpdateNotificationsPos()
        {
            int i = 0;
            foreach (var notif in notifQue)
            {
                var rect = (RectTransform)notif.transform;
                float currY = rect.anchoredPosition.y;
                float targetY = GetTargetPosY(i);
                if (currY < targetY)
                {
                    float y = Mathf.Min(currY + Time.deltaTime * notifFloatingSpeed, targetY);
                    rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, y);
                }

                i++;
            }
        }

        private void Update()
        {
            UpdateNotificationsPos();
        }
    }
}