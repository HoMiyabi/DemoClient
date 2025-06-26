using System;
using System.Collections.Generic;
using cfg.main;
using UnityEngine;

namespace Kirara.UI
{
    public class UIQuestRewordBar : MonoBehaviour
    {
        [SerializeField] private GameObject UIQuestRewordCellPrefab;

        private SimpleGOPool pool;
        private List<RewordConfig> _rewords;

        private void Awake()
        {
            pool = new SimpleGOPool(UIQuestRewordCellPrefab, transform);
            pool.ReleaseChildren(transform);
        }

        public void Set(List<RewordConfig> rewords)
        {
            _rewords = rewords;
            pool.ReleaseChildren(transform);
            foreach (var reword in rewords)
            {
                var cell = pool.Get<UIQuestRewordCell>(transform, false);
                cell.Set(reword);
            }
        }
    }
}