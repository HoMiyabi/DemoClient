using System;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI
{
    public class UITabController : MonoBehaviour
    {
        public int index;
        public event Action<int> onIndexChanged;

        public Transform TabBar;
        public Transform TabView;

        private void Awake()
        {
            if (TabBar == null || TabView == null)
            {
                Debug.LogWarning($"{nameof(TabBar)} and {nameof(TabView)} must be assigned.");
                return;
            }
            if (TabBar.childCount != TabView.childCount)
            {
                Debug.LogWarning($"{nameof(TabBar)} and {nameof(TabView)} must have the same number of children.");
            }
        }

        private void Start()
        {
            int n = Mathf.Min(TabBar.childCount, TabView.childCount);
            for (int i = 0; i < n; i++)
            {
                int idx = i;
                TabBar.GetChild(i).GetComponent<Button>().onClick.AddListener(() =>
                {
                    To(idx);
                });
                TabView.GetChild(i).gameObject.SetActive(i == index);
            }
        }


        public void To(int idx)
        {
            if (idx < 0 || idx >= TabView.childCount)
            {
                Debug.LogError("Invalid index.");
                return;
            }
            if (idx != index)
            {
                TabView.GetChild(idx).gameObject.SetActive(true);
                TabView.GetChild(index).gameObject.SetActive(false);
                index = idx;
                onIndexChanged?.Invoke(index);
            }
        }
    }
}