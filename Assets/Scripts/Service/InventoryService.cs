using System;
using Kirara.Model;

namespace Kirara.Service
{
    public static class InventoryService
    {
        public static event Action<BaseItem, int> OnObtainItem;

        public static void NotifyObtainItems(NotifyObtainItems msg)
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
            var item = PlayerService.Player.Materials.Find(it => it.Cid == cid);
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
                PlayerService.Player.Materials.Add(item);
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
            var item = PlayerService.Player.Currencies.Find(it => it.Cid == cid);
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
                PlayerService.Player.Currencies.Add(item);
            }
            return item;
        }

        #endregion

        private static void ObtainDisc(NDiscItem disc)
        {
            var item = new DiscItem(disc);
            PlayerService.Player.Discs.Add(item);
            OnObtainItem?.Invoke(item, 1);
        }

        private static void ObtainWeapon(NWeaponItem weapon)
        {
            var item = new WeaponItem(weapon);
            PlayerService.Player.Weapons.Add(item);
            OnObtainItem?.Invoke(item, 1);
        }
    }
}