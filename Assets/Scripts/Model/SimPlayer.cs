using System.Collections.Generic;
using Manager;

namespace Kirara.Model
{
    public class SimPlayer
    {
        public string Uid { get; private set; }
        public List<SimRole> simRoles = new();

        public List<SimChCtrl> simChCtrls = new();
        public int frontChIdx;
        public SimChCtrl FrontCh => simChCtrls[frontChIdx];

        public SimPlayer(NSimPlayer simPlayer)
        {
            Uid = simPlayer.Uid;

            foreach (var simRole in simPlayer.Roles)
            {
                string loc = ConfigMgr.tb.TbCharacterConfig[simRole.Cid].SimPrefabLoc;
                var go = AssetMgr.Instance.InstantiateGO(loc, SimPlayerSystem.Instance.simulateCharacterParent);

                var simCh = go.GetComponent<SimChCtrl>();
                simCh.Set(new SimRole(simRole));
                simCh.SetImmediate(pos, rot);
                simChCtrls.Add(simCh);
            }
            frontChIdx = simPlayer.FrontChIdx;

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