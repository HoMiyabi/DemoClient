using UnityEngine;

namespace Kirara
{
    public static class ProtoExtensions
    {
        public static Vector3 Unity(this NVector3 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public static Quaternion Unity(this NQuaternion q)
        {
            return new Quaternion(q.X, q.Y, q.Z, q.W);
        }

        public static Quaternion Quat(this NVector3 v)
        {
            return Quaternion.Euler(v.X, v.Y, v.Z);
        }

        public static Vector3 Pos(this NMovement movement)
        {
            return movement.Pos.Unity();
        }

        public static Quaternion Rot(this NMovement movement)
        {
            return movement.Rot.Quat();
        }

        public static NVector3 Set(this NVector3 self, Vector3 v)
        {
            self.X = v.x;
            self.Y = v.y;
            self.Z = v.z;
            return self;
        }

        public static NVector3 Net(this Vector3 v)
        {
            return new NVector3().Set(v);
        }

        //
        // public static Modifier GetModifier(this NWeaponAttr weaponAttr)
        // {
        //     return new Modifier((EAttrType)weaponAttr.AttrTypeId, weaponAttr.Value);
        // }
        //
        // public static Modifier GetModifier(this NDiscAttr discAttr)
        // {
        //     return new Modifier((EAttrType)discAttr.AttrTypeId, discAttr.Value);
        // }
    }
}