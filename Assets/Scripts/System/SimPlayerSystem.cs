using System.Collections.Generic;
using Kirara.Model;
using UnityEngine;

namespace Kirara
{
    public class SimPlayerSystem : UnitySingleton<SimPlayerSystem>
    {
        [SerializeField] public Transform simulateCharacterParent;
        private readonly Dictionary<int, SimPlayerModel> simPlayers = new();

        public void AddSimPlayer(NRoomSimPlayerInfo playerInfo)
        {
            var player = new SimPlayerModel(playerInfo);
            simPlayers.Add(playerInfo.UId, player);
        }

        public void RemoveSimPlayer(int playerUId)
        {
            if (simPlayers.Remove(playerUId, out var simPlayer))
            {
                foreach (var simCh in simPlayer.simChCtrls)
                {
                    Destroy(simCh.gameObject);
                }
            }
            else
            {
                Debug.LogWarning($"Remove SimPlayer {playerUId} is not found");
            }
        }

        public bool TryGetSimPlayer(int uid, out SimPlayerModel simPlayer)
        {
            if (simPlayers.TryGetValue(uid, out simPlayer)) return true;
            Debug.LogWarning($"Get SimPlayer {uid} is not found");
            return false;
        }
    }
}