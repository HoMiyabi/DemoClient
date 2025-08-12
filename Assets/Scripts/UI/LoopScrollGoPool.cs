using System.Collections.Generic;
using KiraraLoopScroll;
using UnityEngine;

public class LoopScrollGOPool : IGOSource
{
    private readonly GameObject prefab;
    private readonly Transform poolPlace;
    private readonly Stack<GameObject> pool = new();
    private readonly int max;

    public LoopScrollGOPool(GameObject prefab, Transform poolPlace, int max = 128)
    {
        this.prefab = prefab;
        this.poolPlace = poolPlace;
        this.max = max;
    }

    public GameObject GetObject(int index)
    {
        if (pool.Count > 0)
        {
            var go = pool.Pop();
            go.SetActive(true);
            return go;
        }
        return Object.Instantiate(prefab);
    }

    public void ReturnObject(GameObject go)
    {
        if (pool.Count < max)
        {
            go.transform.SetParent(poolPlace, false);
            go.SetActive(false);
            pool.Push(go);
        }
        else
        {
            Object.Destroy(go);
        }
    }
}