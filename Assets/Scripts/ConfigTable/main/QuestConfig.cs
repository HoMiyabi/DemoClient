
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;
using Newtonsoft.Json.Linq;



namespace cfg.main
{

public abstract partial class QuestConfig : Luban.BeanBase
{
    public QuestConfig(JToken _buf) 
    {
        JObject _obj = _buf as JObject;
        QuestCid = (int)_obj.GetValue("quest_cid");
        NextQuestCid = (int)_obj.GetValue("next_quest_cid");
        Name = (string)_obj.GetValue("name");
        Desc = (string)_obj.GetValue("desc");
        { var __json0 = _obj.GetValue("npc_overrides"); NpcOverrides = new System.Collections.Generic.List<main.NPCOverrideConfig>((__json0 as JArray).Count); foreach(JToken __e0 in __json0) { main.NPCOverrideConfig __v0;  __v0 = main.NPCOverrideConfig.DeserializeNPCOverrideConfig(__e0);  NpcOverrides.Add(__v0); }   }
        CompleteGetQuestChainCid = (int)_obj.GetValue("complete_get_quest_chain_cid");
        { var __json0 = _obj.GetValue("trunk_npc_cids"); TrunkNpcCids = new System.Collections.Generic.List<int>((__json0 as JArray).Count); foreach(JToken __e0 in __json0) { int __v0;  __v0 = (int)__e0;  TrunkNpcCids.Add(__v0); }   }
    }

    public static QuestConfig DeserializeQuestConfig(JToken _buf)
    {
        var _obj=_buf as JObject;
        switch (_obj.GetValue("$type").ToString())
        {
            case "DialogueQuestConfig": return new main.DialogueQuestConfig(_buf);
            case "GotoQuestConfig": return new main.GotoQuestConfig(_buf);
            case "DefeatQuestConfig": return new main.DefeatQuestConfig(_buf);
            case "GatherQuestConfig": return new main.GatherQuestConfig(_buf);
            case "UpgradeQuestConfig": return new main.UpgradeQuestConfig(_buf);
            default: throw new SerializationException();
        }
    }

    public readonly int QuestCid;
    public readonly int NextQuestCid;
    public readonly string Name;
    public readonly string Desc;
    public readonly System.Collections.Generic.List<main.NPCOverrideConfig> NpcOverrides;
    public readonly int CompleteGetQuestChainCid;
    public readonly System.Collections.Generic.List<int> TrunkNpcCids;



    public virtual void ResolveRef(Tables tables)
    {
        foreach (var _e in NpcOverrides) { _e?.ResolveRef(tables); }
    }

    public override string ToString()
    {
        return "{ "
        + "questCid:" + QuestCid + ","
        + "nextQuestCid:" + NextQuestCid + ","
        + "name:" + Name + ","
        + "desc:" + Desc + ","
        + "npcOverrides:" + Luban.StringUtil.CollectionToString(NpcOverrides) + ","
        + "completeGetQuestChainCid:" + CompleteGetQuestChainCid + ","
        + "trunkNpcCids:" + Luban.StringUtil.CollectionToString(TrunkNpcCids) + ","
        + "}";
    }
}
}

