// using System;
// using System.Collections.Generic;
// using UnityEngine;
//
// namespace Kirara.Manager
// {
//     public class KiraraEventManager : Singleton<KiraraEventManager>
//     {
//         private Dictionary<string, Delegate> dict = new();
//
//         #region Register
//
//         private void Register(string name, Delegate action)
//         {
//             if (dict.TryGetValue(name, out var callbacks))
//             {
//                 dict[name] = Delegate.Combine(callbacks, action);
//             }
//             else
//             {
//                 dict.Add(name, action);
//             }
//         }
//
//         public void Register(string name, Action action)
//         {
//             Register(name, action as Delegate);
//         }
//
//         public void Register<T>(string name, Action<T> action)
//         {
//             Register(name, action as Delegate);
//         }
//
//         public void Register<T1, T2>(string name, Action<T1, T2> action)
//         {
//             Register(name, action as Delegate);
//         }
//
//         public void Register<T1, T2, T3>(string name, Action<T1, T2, T3> action)
//         {
//             Register(name, action as Delegate);
//         }
//
//         public void Register<T1, T2, T3, T4>(string name, Action<T1, T2, T3, T4> action)
//         {
//             Register(name, action as Delegate);
//         }
//
//         public void Register<T1, T2, T3, T4, T5>(string name, Action<T1, T2, T3, T4, T5> action)
//         {
//             Register(name, action as Delegate);
//         }
//
//         #endregion
//
//         #region Unregister
//
//         private void Unregister(string name, Delegate action)
//         {
//             if (dict.TryGetValue(name, out var callbacks))
//             {
//                 dict[name] = Delegate.Remove(callbacks, action);
//             }
//         }
//
//         public void Unregister(string name, Action action)
//         {
//             Unregister(name, action as Delegate);
//         }
//
//         public void Unregister<T>(string name, Action<T> action)
//         {
//             Unregister(name, action as Delegate);
//         }
//
//         public void Unregister<T1, T2>(string name, Action<T1, T2> action)
//         {
//             Unregister(name, action as Delegate);
//         }
//
//         public void Unregister<T1, T2, T3>(string name, Action<T1, T2, T3> action)
//         {
//             Unregister(name, action as Delegate);
//         }
//
//         public void Unregister<T1, T2, T3, T4>(string name, Action<T1, T2, T3, T4> action)
//         {
//             Unregister(name, action as Delegate);
//         }
//
//         public void Unregister<T1, T2, T3, T4, T5>(string name, Action<T1, T2, T3, T4, T5> action)
//         {
//             Unregister(name, action as Delegate);
//         }
//
//         #endregion
//
//         #region CallEvent
//
//         public void CallEvent(string name)
//         {
//             if (!dict.TryGetValue(name, out var callbacks)) return;
//             if (callbacks is Action action)
//             {
//                 action();
//             }
//             else
//             {
//                 Debug.LogWarning($"调用事件{name}参数不匹配");
//             }
//         }
//
//         public void CallEvent<T>(string name, T value)
//         {
//             if (!dict.TryGetValue(name, out var callbacks)) return;
//             if (callbacks is Action<T> action)
//             {
//                 action(value);
//             }
//             else
//             {
//                 Debug.LogWarning($"调用事件{name}参数不匹配");
//             }
//         }
//
//         public void CallEvent<T1, T2>(string name, T1 t1, T2 t2)
//         {
//             if (!dict.TryGetValue(name, out var callbacks)) return;
//             if (callbacks is Action<T1, T2> action)
//             {
//                 action(t1, t2);
//             }
//             else
//             {
//                 Debug.LogWarning($"调用事件{name}参数不匹配");
//             }
//         }
//
//         public void CallEvent<T1, T2, T3>(string name, T1 t1, T2 t2, T3 t3)
//         {
//             if (!dict.TryGetValue(name, out var callbacks)) return;
//             if (callbacks is Action<T1, T2, T3> action)
//             {
//                 action(t1, t2, t3);
//             }
//             else
//             {
//                 Debug.LogWarning($"调用事件{name}参数不匹配");
//             }
//         }
//
//         public void CallEvent<T1, T2, T3, T4>(string name, T1 t1, T2 t2, T3 t3, T4 t4)
//         {
//             if (!dict.TryGetValue(name, out var callbacks)) return;
//             if (callbacks is Action<T1, T2, T3, T4> action)
//             {
//                 action(t1, t2, t3, t4);
//             }
//             else
//             {
//                 Debug.LogWarning($"调用事件{name}参数不匹配");
//             }
//         }
//
//         public void CallEvent<T1, T2, T3, T4, T5>(string name, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
//         {
//             if (!dict.TryGetValue(name, out var callbacks)) return;
//             if (callbacks is Action<T1, T2, T3, T4, T5> action)
//             {
//                 action(t1, t2, t3, t4, t5);
//             }
//             else
//             {
//                 Debug.LogWarning($"调用事件{name}参数不匹配");
//             }
//         }
//
//         #endregion
//     }
// }