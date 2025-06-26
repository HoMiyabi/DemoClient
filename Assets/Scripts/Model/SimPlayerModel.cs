using System.Collections.Generic;

using Manager;
using UnityEngine;

namespace Kirara.Model
{
    public class SimPlayerModel
    {
        public readonly int uid;
        public Vector3 pos;
        public Quaternion rot;
        public List<SimChCtrl> simChCtrls = new();
        public int frontChIdx;
        public SimChCtrl FrontCh => simChCtrls[frontChIdx];

        public SimPlayerModel(NRoomSimPlayer playerInfo)
        {
            uid = playerInfo.UId;
            pos = playerInfo.PosRot.Pos.Unity();
            rot = playerInfo.PosRot.Rot.Quat();
            foreach (int chCid in playerInfo.GroupChCids)
            {
                string loc = ConfigMgr.tb.TbCharacterConfig[chCid].SimPrefabLoc;
                var go = AssetMgr.Instance.InstantiateGO(loc, SimPlayerSystem.Instance.simulateCharacterParent);

                var simCh = go.GetComponent<SimChCtrl>();
                simCh.Set(new SimChModel(chCid));
                simCh.SetImmediate(pos, rot);
                simChCtrls.Add(simCh);
            }
            frontChIdx = playerInfo.FrontChIdx;

            for (int i = 0; i < simChCtrls.Count; i++)
            {
                var ch = simChCtrls[i];
                if (i == frontChIdx)
                {
                    ch.gameObject.SetActive(true);
                }
                else
                {
                    ch.gameObject.SetActive(false);
                }
            }
        }

        public void SwitchCh(int idx, bool next)
        {
            FrontCh.AIControl();
            frontChIdx = idx;
            FrontCh.SimControl();
        }
    }
}