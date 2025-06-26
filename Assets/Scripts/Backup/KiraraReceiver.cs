// using UnityEngine;
// using UnityEngine.Playables;
//
// namespace Kirara.Timeline
// {
//     public class KiraraReceiver : MonoBehaviour, INotificationReceiver
//     {
//         public void OnNotify(Playable origin, INotification notification, object context)
//         {
//             if (notification is KiraraSignal signal)
//             {
//                 Debug.Log("Signal Event Name " + signal.eventName);
//             }
//             else if (notification is PlaySFXSignalEmitter sfxSignal)
//             {
//                 Debug.Log("PlaySFX " + sfxSignal.sfxName);
//             }
//         }
//     }
// }