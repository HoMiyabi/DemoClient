using cfg;
using UnityEngine;

namespace Kirara
{
    public static class LubanExtensions
    {
        public static Vector3 Unity(this vector3 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }
    }
}