using UnityEngine;

namespace Kirara
{
    public class UnitySceneSingleton<T> : MonoBehaviour where T : UnitySceneSingleton<T>
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
            }
            else
            {
                Debug.LogWarning($"UnitySingleton<{typeof(T).Name}>已经存在");
                Destroy(gameObject);
            }
        }
    }
}