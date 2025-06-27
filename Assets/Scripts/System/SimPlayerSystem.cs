using System.Collections.Generic;
using Kirara.Model;
using UnityEngine;

namespace Kirara
{
    public class SimPlayerSystem : UnitySingleton<SimPlayerSystem>
    {
        [SerializeField] public Transform simulateCharacterParent;
        private readonly Dictionary<string, SimPlayer> simPlayers = new();

        public void AddSimPlayer(NSimPlayer simPlayer)
        {
            var player = new SimPlayer(simPlayer);
            simPlayers.Add(simPlayer.Uid, player);
        }

        public void RemoveSimPlayer(IList<string> uids)
        {
            foreach (string uid in uids)
            {
                if (simPlayers.Remove(uid, out var simPlayer))
                {
                    simPlayer.RemoveAllRoles();
                }
                else
                {
                    Debug.LogWarning($"Remove SimPlayer {uid} is not found");
                }
            }
        }

        public bool TryGetSimPlayer(string uid, out SimPlayer simPlayer)
        {
            if (simPlayers.TryGetValue(uid, out simPlayer)) return true;
            Debug.LogWarning($"Get SimPlayer {uid} is not found");
            return false;
        }
    }
}