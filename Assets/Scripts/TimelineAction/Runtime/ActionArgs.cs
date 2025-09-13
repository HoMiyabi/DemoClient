using System;
using UnityEngine.Serialization;

namespace Kirara.TimelineAction
{
    [Serializable]
    public class ActionArgs
    {
        public bool enableRotation = false;
        public bool enableRecenter = false;
        [FormerlySerializedAs("lookAtMonster")] public bool enableLookAtMonster = false;
        public ERoleShowState roleShowState = ERoleShowState.Front;

        // public static ActionParams GetStateDefault(EActionState state)
        // {
        //     var ret = new ActionParams();
        //     switch (state)
        //     {
        //         case EActionState.Idle:
        //         {
        //             ret.enableRotation = true;
        //             break;
        //         }
        //         case EActionState.Dodge_Front:
        //         {
        //             ret.enableRotation = true;
        //             break;
        //         }
        //         case EActionState.Walk:
        //         {
        //             ret.enableRotation = true;
        //             ret.enableRecenter = true;
        //             break;
        //         }
        //         case EActionState.Run:
        //         {
        //             ret.enableRotation = true;
        //             ret.enableRecenter = true;
        //             break;
        //         }
        //         case EActionState.Attack_Normal:
        //         {
        //             ret.lookAtMonster = true;
        //             break;
        //         }
        //         case EActionState.Attack_Special:
        //         {
        //             ret.lookAtMonster = true;
        //             break;
        //         }
        //         case EActionState.Attack_Ex_Special:
        //         {
        //             ret.lookAtMonster = true;
        //             break;
        //         }
        //     }
        //     return ret;
        // }
    }
}