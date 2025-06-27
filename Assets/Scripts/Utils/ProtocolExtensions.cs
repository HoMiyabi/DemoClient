using cfg.main;

using Kirara.AttrEffect;
using Manager;
using UnityEngine;

namespace Kirara
{
    public static class ProtoExtensions
    {
        public static Vector3 Unity(this NFloat3 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public static Quaternion Quat(this NFloat3 v)
        {
            return Quaternion.Euler(v.X, v.Y, v.Z);
        }

        public static Vector3 Pos(this NPosRot posRot)
        {
            return posRot.Pos.Unity();
        }

        public static Quaternion Rot(this NPosRot posRot)
        {
            return posRot.Rot.Quat();
        }

        public static NFloat3 Set(this NFloat3 self, Vector3 v)
        {
            self.X = v.x;
            self.Y = v.y;
            self.Z = v.z;
            return self;
        }

        public static Modifier GetModifier(this NWeaponAttr weaponAttr)
        {
            return new Modifier((EAttrType)weaponAttr.AttrTypeId, weaponAttr.Value);
        }

        public static Modifier GetModifier(this NDiscAttr discAttr)
        {
            return new Modifier((EAttrType)discAttr.AttrTypeId, discAttr.Value);
        }
    }
}