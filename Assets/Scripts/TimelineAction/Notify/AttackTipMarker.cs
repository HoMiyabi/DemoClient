using System.ComponentModel;
using Kirara.UI;
using Manager;
using UnityEngine;

namespace Kirara.TimelineAction
{
    [DisplayName("攻击提示标记")]
    public class AttackTipMarker : ActionNotify
    {
        public bool canParry;

        public override void Notify(ActionPlayer player)
        {
            var monster = player.GetComponent<Monster>();
            if (monster == null)
            {
                Debug.LogWarning("Monster is null");
                return;
            }
            Debug.Log("攻击提示");

            var handle = AssetMgr.Instance.package.LoadAssetSync<AudioClip>("AttackTip");
            AudioMgr.Instance.PlaySFX(handle.AssetObject as AudioClip, monster.transform.position);
            handle.Release();

            UIMgr.Instance.AddHUD<UIAttackLight>().Set(canParry, monster.attackLightFollow);
        }
    }
}