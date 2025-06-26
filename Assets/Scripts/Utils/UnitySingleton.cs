using UnityEngine;

namespace Kirara
{
    public class UnitySingleton<T> : MonoBehaviour where T : UnitySingleton<T>
    {
        private static T instance;

        public static T Instance {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance != null)
                    {
                        DontDestroyOnLoad(instance.gameObject);
                        Debug.LogWarning($"{typeof(T).Name}.Instance未设置，使用FindObjectOfType查找到");
                    }
                    else
                    {
                        var go = new GameObject(typeof(T).Name);
                        instance = go.AddComponent<T>();
                        DontDestroyOnLoad(go);
                        Debug.LogWarning($"{typeof(T).Name}.Instance不存在，创建GameObject");
                    }
                }
                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
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