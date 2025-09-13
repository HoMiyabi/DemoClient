using UnityEngine;
using YooAsset;

namespace Kirara.UI.Panel
{
    public class AttackTipPanel : BasePanel
    {
        public GameObject AttackLightPrefab;

        private void OnEnable()
        {
            MonsterSystem.Instance.OnMonsterAttackTip += OnMonsterAttackTip;
        }

        private void OnDisable()
        {
            MonsterSystem.Instance.OnMonsterAttackTip -= OnMonsterAttackTip;
        }

        private void OnMonsterAttackTip(MonsterCtrl monsterCtrl, bool canParry)
        {
            Debug.Log("攻击提示");
            var handle = YooAssets.LoadAssetSync<AudioClip>("Attack_Tip");
            AudioMgr.Instance.PlaySFX(handle.AssetObject as AudioClip, transform.position);
            handle.Release();

            Instantiate(AttackLightPrefab, transform).GetComponent<UIAttackLight>()
                .Set(canParry, monsterCtrl.attackLightFollow);
        }
    }
}