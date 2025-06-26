using System;
using UnityEngine;

namespace Kirara
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}