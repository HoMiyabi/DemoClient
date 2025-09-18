using System.Collections.Generic;
using Manager;
using UnityEngine;
using YooAsset;

namespace Kirara.Model
{
    public class SimPlayer
    {
        public string Uid { get; private set; }
        private Dictionary<string, SimRole> SimRoles { get; set; } = new();
        private Dictionary<string, SimRoleCtrl> SimRoleCtrls { get; set; } = new();

        public SimPlayer(NSyncPlayer simPlayer)
        {
            Uid = simPlayer.Uid;

            foreach (var nSimRole in simPlayer.Roles)
            {
                var simRole = new SimRole(nSimRole);
                SimRoles.Add(simRole.Id, simRole);

                string loc = ConfigMgr.tb.TbRoleConfig[simRole.Cid].SimPrefabLoc;
                var handle = YooAssets.LoadAssetSync<GameObject>(loc);
                var go = handle.InstantiateSync(SimPlayerSystem.Instance.simRoleParent);

                var simRoleCtrl = go.GetComponent<SimRoleCtrl>();
                simRoleCtrl.Set(simRole, nSimRole.ActionName);
                SimRoleCtrls.Add(simRole.Id, simRoleCtrl);
            }
        }

        public void RemoveAllRoles()
        {
            foreach (var simRoleCtrl in SimRoleCtrls.Values)
            {
                Object.Destroy(simRoleCtrl.gameObject);
            }
            SimRoleCtrls.Clear();
            SimRoles.Clear();
        }

        public void Update(NSyncPlayer syncPlayer)
        {
            foreach (var syncRole in syncPlayer.Roles)
            {
                if (SimRoles.TryGetValue(syncRole.Id, out var simRole))
                {
                    simRole.Update(syncRole);
                }
                else
                {
                    Debug.LogWarning($"SimRole: {syncRole.Id} not found");
                }
            }
        }

        public void RolePlayAction(string roleId, string actionName)
        {
            if (SimRoleCtrls.TryGetValue(roleId, out var simRoleCtrl))
            {
                simRoleCtrl.PlayAction(actionName, 0.15f);
            }
        }
    }
}