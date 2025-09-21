using System.Collections.Generic;
using cfg.main;
using Manager;
using UnityEngine;
using XLua;

namespace Kirara.AttrBuff
{
    [LuaCallCSharp]
    public class AttrBuffSet
    {
        #region Attr

        private Dictionary<int, Attr> AttrDict { get; set; } = new();

        public double this[EAttrType type]
        {
            get => this[(int)type];
            set => this[(int)type] = value;
        }

        public double this[int type]
        {
            get
            {
                if (AttrDict.TryGetValue(type, out var attr))
                {
                    return attr.Evaluate(this);
                }
                return 0;
            }
            set
            {
                if (AttrDict.TryGetValue(type, out var attr))
                {
                    attr.baseValue = value;
                }
                else
                {
                    AttrDict[type] = new Attr((EAttrType)type, value);
                }
            }
        }

        public Attr GetAttr(EAttrType type)
        {
            return AttrDict[(int)type];
        }

        public Attr GetAttr(int type)
        {
            return AttrDict[type];
        }

        #endregion

        #region Buff

        public List<IBuffComponent> Buffs { get; private set; } = new();
        private List<(string handle, float time)> Timers { get; set; } = new();

        [CSharpCallLua]
        private delegate void InitBuffDel(IBuffComponent buff);

        private static readonly Dictionary<string, InitBuffDel> configBuffs =
            LuaMgr.Instance.LuaEnv.Global.Get<Dictionary<string, InitBuffDel>>("configBuffs");

        [CSharpCallLua]
        private delegate IBuffComponent NewBuffDel(AttrBuffSet set, string name);

        private static readonly NewBuffDel NewBuff =
            LuaMgr.Instance.LuaEnv.Global.GetInPath<NewBuffDel>("BuffComponent.New");

        public void Update(float dt)
        {
            UpdateTimers(dt);
            UpdateBuffs(dt);
            RemoveZeroStackBuffs();
        }

        private void UpdateTimers(float dt)
        {
            for (int i = 0; i < Timers.Count;)
            {
                (string handle, float time) = Timers[i];
                time -= dt;
                if (time > 0)
                {
                    Timers[i] = (handle, time);
                    i++;
                }
                else
                {
                    Timers.RemoveAt(i);
                }
            }
        }

        private void UpdateBuffs(float dt)
        {
            // Debug.Log($"UpdateBuffs, Buffs.Count: {Buffs.Count}");
            foreach (var ability in Buffs)
            {
                ability.Update(dt);
            }
        }

        private void RemoveZeroStackBuffs()
        {
            for (int i = 0; i < Buffs.Count;)
            {
                if (Buffs[i].stackCount <= 0)
                {
                    if (Buffs[i].stackCount < 0)
                    {
                        Debug.LogWarning($"Buff stack count < 0 name:{Buffs[i].name}");
                    }
                    Buffs.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }

        public void RemoveBuff(string buffName)
        {
            Buffs.RemoveAll(x => x.name == buffName);
        }

        public void SetTimer(string handle, float time)
        {
            int i = Timers.FindIndex(x => x.handle == handle);
            if (i >= 0)
            {
                Timers[i] = (handle, time);
            }
            else
            {
                Timers.Add((handle, time));
            }
        }

        public bool HasTimer(string handle)
        {
            return Timers.FindIndex(x => x.handle == handle) >= 0;
        }

        public void AttachBuff(string name, Dictionary<EAttrType, double> attrs)
        {
            if (Buffs.Find(x => x.name == name) != null)
            {
                Debug.LogWarning($"Buff已存在 name: {name}");
                return;
            }
            var buff = NewBuff(this, name);
            foreach (var kv in attrs)
            {
                buff.attrs.Set(kv.Key, kv.Value);
            }
            Buffs.Add(buff);
            buff.Attached();
        }

        public void AttachBuff(string name)
        {
            var buff = Buffs.Find(x => x.name == name);
            if (buff == null)
            {
                if (configBuffs.TryGetValue(name, out var initBuff))
                {
                    buff = NewBuff(this, name);
                    initBuff(buff);
                    Buffs.Add(buff);
                }
                else
                {
                    Debug.LogWarning($"Buff不存在 name: {name}");
                    return;
                }
            }
            buff.Attached();
        }

        public double Inject(string buffName, string varName)
        {
            return 0;
        }

        public void OnActionStart(OnActionStartContext ctx)
        {
            // 可能在OnActionStart中添加新的Buff，不能foreach
            for (int i = 0; i < Buffs.Count; i++)
            {
                Buffs[i].OnActionStart(ctx);
            }
        }

        public void OnAttackHit(OnAttackHitContext ctx)
        {
            // 可能在OnAttackHit中添加新的Buff，不能foreach
            for (int i = 0; i < Buffs.Count; i++)
            {
                Buffs[i].OnAttackHit(ctx);
            }
        }

        #endregion
    }
}