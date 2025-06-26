using System;
using UnityEngine;

public class SelectController : MonoBehaviour
{
    private ISelectItem item;
    public ISelectItem Item
    {
        get => item;
        set
        {
            if (value == item) return;

            item?.OnDeselect();
            item = value;
            item?.OnSelect();
            OnItemChanged?.Invoke();
        }
    }
    public event Action OnItemChanged;
}