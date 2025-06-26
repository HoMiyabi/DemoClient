using UnityEngine;

namespace System
{
    public static class LayerConfig
    {
        public static readonly int MonsterMask = LayerMask.GetMask("Monster");
        public static readonly int CharacterMask = LayerMask.GetMask("Character");
    }
}