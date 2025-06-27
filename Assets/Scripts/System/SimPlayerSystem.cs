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

        public void RemoveSimPlayer(string uid)
        {
            if (simPlayers.Remove(uid, out var simPlayer))
            {
                foreach (var simRole in simPlayer.simChCtrls)
                {
                    Destroy(simRole.gameObject);
                }
            }
            else
            {
                Debug.LogWarning($"Remove SimPlayer {uid} is not found");
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