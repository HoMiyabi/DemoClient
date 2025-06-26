using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Kirara.Model;
using Manager;
using UnityEngine;

namespace Kirara.Service
{
    public static class DiscService
    {
        public static event Action<DiscItem> OnDiscUpgradedLevel;

        public static void CalcUpgraded(DiscItem disc, int addExp, out int level, out int exp)
        {
            exp = disc.Exp + addExp;
            level = disc.Level;
            while (level < DiscItem.MaxLevel)
            {
                var config = ConfigMgr.tb.TbDiscUpgradeExpConfig[level + 1];
                if (exp >= config.Exp)
                {
                    level++;
                    exp -= config.Exp;
                }
                else
                {
                    break;
                }
            }
            if (level >= DiscItem.MaxLevel)
            {
                exp = 0;
            }
        }

        public static async UniTask Upgrade(DiscItem disc, int matCid, int matExp, int useCount)
        {
            var rsp = await NetFn.ReqUpgradeDisc(new ReqUpgradeDisc
            {
                DiscId = disc.Id,
                MaterialCid = matCid,
                Count = useCount
            });
            if (rsp.Code != 0)
            {
                Debug.LogWarning($"失败: {rsp.Msg}");
                return;
            }
            CalcUpgraded(disc, useCount * matExp, out int level, out int exp);
            int prevLevel = disc.Level;
            disc.Level = level;
            disc.Exp = exp;

            disc.SubAttrs = rsp.SubAttrs.ToList();

            if (disc.Level > prevLevel)
            {
                OnDiscUpgradedLevel?.Invoke(disc);
            }
        }

        public static void GachaDisc()
        {
            var msg = new MsgGachaDisc();
            NetFn.Send(msg);
        }
    }
}