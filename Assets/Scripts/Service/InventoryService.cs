using System;
using Kirara.Model;
using Kirara.NetHandler;
using UnityEngine;

namespace Kirara.Service
{
    public static class InventoryService
    {
        public static event Action<BaseItem, int> OnObtainItem;

        private static bool bInit;

        public static void Init()
        {
            if (bInit)
            {
                Debug.LogWarning("InventoryService has already been initialized.");
                return;
            }
            bInit = true;

            NotifyObtainItems_Handler.OnNotifyObtainItems += OnNotifyObtainItems;
        }

        private static void OnNotifyObtainItems(NotifyObtainItems msg)
        {
            // 货币
            foreach (var cur in msg.CurItems)
            {
                ObtainCurrency(cur.Cid, cur.Count);
            }
            // 材料
            foreach (var mat in msg.MatItems)
            {
                ObtainMaterial(mat.Cid, mat.Count);
            }
            // 驱动盘
            foreach (var disc in msg.DiscItems)
            {
                ObtainDisc(disc);
            }
            // 武器
            foreach (var weapon in msg.WeaponItems)
            {
                ObtainWeapon(weapon);
            }
        }

        public static void GatherMaterial(int MaterialCid, int Count)
        {
            NetFn.Send(new MsgGatherMaterial
            {
                MaterialCid = MaterialCid,
                Count = Count
            });
        }

        #region Material

        private static void ObtainMaterial(int cid, int count)
        {
            var item = AddMaterialCount(cid, count);
            OnObtainItem?.Invoke(item, count);
        }

        private static MaterialItem AddMaterialCount(int cid, int count)
        {
            var item = PlayerService.player.materials.Find(it => it.Cid == cid);
            if (item != null)
            {
                item.Count += count;
            }
            else
            {
                item = new MaterialItem(new NMaterialItem
                {
                    Cid = cid,
                    Count = count
                });
                PlayerService.player.materials.Add(item);
            }
            return item;
        }

        #endregion

        #region Currency

        private static void ObtainCurrency(int cid, int count)
        {
            var item = AddCurrencyCount(cid, count);
            OnObtainItem?.Invoke(item, count);
        }

        private static CurrencyItem AddCurrencyCount(int cid, int count)
        {
            var item = PlayerService.player.currencies.Find(it => it.Cid == cid);
            if (item != null)
            {
                item.Count += count;
            }
            else
            {
                item = new CurrencyItem(new NCurrencyItem
                {
                    Cid = cid,
                    Count = count
                });
                PlayerService.player.currencies.Add(item);
            }
            return item;
        }

        #endregion

        private static void ObtainDisc(NDiscItem disc)
        {
            var item = new DiscItem(disc);
            PlayerService.player.discs.Add(item);
            OnObtainItem?.Invoke(item, 1);
        }

        private static void ObtainWeapon(NWeaponItem weapon)
        {
            var item = new WeaponItem(weapon);
            PlayerService.player.weapons.Add(item);
            OnObtainItem?.Invoke(item, 1);
        }
    }
}