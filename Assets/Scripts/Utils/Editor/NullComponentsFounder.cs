using UnityEditor;
using UnityEngine;

namespace Kirara
{
    public class NullComponentsFounder
    {
        [MenuItem("Kirara/查找空组件")]
        public static void FindNullComponents()
        {
            var allGameObjects = Resources.FindObjectsOfTypeAll<GameObject>();

            int count = 0;
            foreach (var go in allGameObjects)
            {
                if (PrefabUtility.IsPartOfPrefabInstance(go) && PrefabUtility.IsAnyPrefabInstanceRoot(go))
                {
                    continue; // 此处跳过了预制体实例，如果不想跳过自行注释。
                }

                var components = go.GetComponents<Component>();

                foreach (var component in components)
                {
                    if (component == null)
                    {
                        Debug.Log("GameObject with missing script found: " + go.name, go);
                        count++;
                        break;
                    }
                }
            }
            Debug.Log("Total null components found: " + count);
        }
    }
}