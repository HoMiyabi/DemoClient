using UnityEngine;
using XLua;

namespace Kirara
{
    [LuaCallCSharp]
    public class UnitySingleton<T> : MonoBehaviour where T : UnitySingleton<T>
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Debug.LogWarning($"UnitySingleton<{typeof(T).Name}>已经存在");
                Destroy(gameObject);
            }
        }
    }
}