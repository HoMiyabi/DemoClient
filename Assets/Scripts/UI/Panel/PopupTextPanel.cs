using UnityEngine;
using Random = UnityEngine.Random;

namespace Kirara.UI.Panel
{
    public class PopupTextPanel : BasePanel
    {
        public GameObject PopupTextPrefab;

        private void OnEnable()
        {
            MonsterSystem.Instance.OnMonsterTakeDamage += OnMonsterTakeDamage;
        }

        private void OnDisable()
        {
            MonsterSystem.Instance.OnMonsterTakeDamage -= OnMonsterTakeDamage;
        }

        private void OnMonsterTakeDamage(MonsterCtrl monsterCtrl, double damage, bool isCrit)
        {
            // 伤害跳字
            var popupTextLocalPos = new Vector3(0, 1.5f, 0) + Random.insideUnitSphere * 0.3f;

            Instantiate(PopupTextPrefab, transform).GetComponent<UIPopupText>()
                .SetInfo(monsterCtrl.transform, popupTextLocalPos, damage, isCrit)
                .Play();
        }
    }
}