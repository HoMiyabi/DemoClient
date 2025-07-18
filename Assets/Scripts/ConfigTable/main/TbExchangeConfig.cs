
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

public partial class TbExchangeConfig
{
    private readonly System.Collections.Generic.Dictionary<int, main.ExchangeConfig> _dataMap;
    private readonly System.Collections.Generic.List<main.ExchangeConfig> _dataList;
    
    public TbExchangeConfig(JArray _buf)
    {
        _dataMap = new System.Collections.Generic.Dictionary<int, main.ExchangeConfig>();
        _dataList = new System.Collections.Generic.List<main.ExchangeConfig>();
        
        foreach(JObject _ele in _buf)
        {
            main.ExchangeConfig _v;
            _v = main.ExchangeConfig.DeserializeExchangeConfig(_ele);
            _dataList.Add(_v);
            _dataMap.Add(_v.ExchangeId, _v);
         }
    }


    public System.Collections.Generic.Dictionary<int, main.ExchangeConfig> DataMap => _dataMap;
    public System.Collections.Generic.List<main.ExchangeConfig> DataList => _dataList;

    public main.ExchangeConfig GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public main.ExchangeConfig Get(int key) => _dataMap[key];
    public main.ExchangeConfig this[int key] => _dataMap[key];

    public void ResolveRef(Tables tables)
    {
        foreach(var _v in _dataList)
        {
            _v.ResolveRef(tables);
        }
    }

}
}

