using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Kirara
{
    public static class Extensions
    {
        public static void DestroyChildren(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                Object.Destroy(child.gameObject);
            }
        }

        public static T RandomItem<T>(this T[] array)
        {
            if (array.Length == 0)
            {
                return default;
            }
            return array[Random.Range(0, array.Length)];
        }

        public static T RandomItem<T>(this T[] array, float defaultProbability)
        {
            if (array.Length == 0 || Random.Range(0f, 1f) < defaultProbability)
            {
                return default;
            }
            return array[Random.Range(0, array.Length)];
        }
    }
}