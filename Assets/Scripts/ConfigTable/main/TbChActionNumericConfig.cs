
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

public partial class TbChActionNumericConfig
{
    private readonly System.Collections.Generic.Dictionary<int, main.ChActionNumericConfig> _dataMap;
    private readonly System.Collections.Generic.List<main.ChActionNumericConfig> _dataList;
    
    public TbChActionNumericConfig(JArray _buf)
    {
        _dataMap = new System.Collections.Generic.Dictionary<int, main.ChActionNumericConfig>();
        _dataList = new System.Collections.Generic.List<main.ChActionNumericConfig>();
        
        foreach(JObject _ele in _buf)
        {
            main.ChActionNumericConfig _v;
            _v = main.ChActionNumericConfig.DeserializeChActionNumericConfig(_ele);
            _dataList.Add(_v);
            _dataMap.Add(_v.ActionId, _v);
         }
    }


    public System.Collections.Generic.Dictionary<int, main.ChActionNumericConfig> DataMap => _dataMap;
    public System.Collections.Generic.List<main.ChActionNumericConfig> DataList => _dataList;

    public main.ChActionNumericConfig GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public main.ChActionNumericConfig Get(int key) => _dataMap[key];
    public main.ChActionNumericConfig this[int key] => _dataMap[key];

    public void ResolveRef(Tables tables)
    {
        foreach(var _v in _dataList)
        {
            _v.ResolveRef(tables);
        }
    }

}
}

