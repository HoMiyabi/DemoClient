using System.Collections.Generic;
using System.Linq;
using Kirara;

namespace System
{
    public class NPCSystem : UnitySingleton<NPCSystem>
    {
        public List<NPCInteractable> npcList = new();
        public Dictionary<int, NPCInteractable> npcDict;

        protected override void Awake()
        {
            base.Awake();
            npcDict = npcList.ToDictionary(it => it.npcCid);
        }
    }
}