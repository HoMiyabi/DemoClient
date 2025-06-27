using UnityEngine;

namespace Kirara.Model
{
    public class SimRole
    {
        public string Id { get; private set; }
        public int Cid { get; private set; }
        public Vector3 Pos { get; set; }
        public Quaternion Rot { get; set; }

        public SimRole(NSimRole simRole)
        {
            Id = simRole.Id;
            Cid = simRole.Cid;
            Pos = simRole.PosRot.Pos();
            Rot = simRole.PosRot.Rot();
        }

        public void Update(NSyncRole syncRole)
        {
            Pos = syncRole.PosRot.Pos();
            Rot = syncRole.PosRot.Rot();
        }
    }
}