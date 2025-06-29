using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kirara.TimelineAction
{
    // 动作列表
    // 一般动作列表属于某个角色
    // 一个角色一般有许多动作，我们希望能在一个地方编辑
    [CreateAssetMenu(fileName = "TimelineActionListSO", menuName = "Kirara/TimelineActionListSO")]
    public class KiraraActionListSO : ScriptableObject
    {
        // 动作名称包含角色名前缀用于区分文件，但播放的时候一般不写前缀，所以要能去掉
        public string namePrefix;
        public List<KiraraActionSO> actions;

        public string RemoveNamePrefix(string actionFullName)
        {
            if (!string.IsNullOrEmpty(namePrefix))
            {
                if (actionFullName.StartsWith(namePrefix))
                {
                    return actionFullName[namePrefix.Length..];
                }
                Debug.LogError($"无法移除前缀 全名：{actionFullName}，前缀：{namePrefix}");
            }
            else
            {
                Debug.Log($"{name} 前缀为空");
            }
            return actionFullName;
        }

        public Dictionary<string, KiraraActionSO> ActionDict
        {
            get
            {
                if (actions == null) return new Dictionary<string, KiraraActionSO>();
                var dict = actions
                    .ToDictionary(x => RemoveNamePrefix(x.name));
                return dict;
            }
        }
    }
}