
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Newtonsoft.Json.Linq;
using Luban;



namespace cfg.main
{

public partial class TbCharacterConfig
{
    private readonly System.Collections.Generic.Dictionary<int, main.CharacterConfig> _dataMap;
    private readonly System.Collections.Generic.List<main.CharacterConfig> _dataList;
    
    public TbCharacterConfig(JArray _buf)
    {
        _dataMap = new System.Collections.Generic.Dictionary<int, main.CharacterConfig>();
        _dataList = new System.Collections.Generic.List<main.CharacterConfig>();
        
        foreach(JObject _ele in _buf)
        {
            main.CharacterConfig _v;
            _v = main.CharacterConfig.DeserializeCharacterConfig(_ele);
            _dataList.Add(_v);
            _dataMap.Add(_v.Id, _v);
         }
    }


    public System.Collections.Generic.Dictionary<int, main.CharacterConfig> DataMap => _dataMap;
    public System.Collections.Generic.List<main.CharacterConfig> DataList => _dataList;

    public main.CharacterConfig GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public main.CharacterConfig Get(int key) => _dataMap[key];
    public main.CharacterConfig this[int key] => _dataMap[key];

    public void ResolveRef(Tables tables)
    {
        foreach(var _v in _dataList)
        {
            _v.ResolveRef(tables);
        }
    }

}
}

