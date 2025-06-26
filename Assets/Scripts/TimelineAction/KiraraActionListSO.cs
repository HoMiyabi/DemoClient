using System.Collections.Generic;
using UnityEngine;

namespace Kirara.TimelineAction
{
    // 动作列表
    // 一般动作列表属于某个角色
    // 一个角色一般有许多动作，我们希望能在一个地方编辑
    [CreateAssetMenu(fileName = "TimelineActionListSO", menuName = "Kirara/TimelineActionListSO")]
    public class KiraraActionListSO : ScriptableObject
    {
        public string namePrefix;
        public List<KiraraActionSO> actions;
    }
}