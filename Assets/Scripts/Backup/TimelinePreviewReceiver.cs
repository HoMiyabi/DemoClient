// using Timeline.Marker;
// using UnityEngine;
// using UnityEngine.Playables;
//
// namespace Kirara
// {
//     public class TimelinePreviewReceiver : MonoBehaviour, INotificationReceiver
//     {
//         public void OnNotify(Playable origin, INotification notification, object context)
//         {
//             if (notification is IActionMarker actionMarker)
//             {
//                 actionMarker.PlayInTimelineEditor(origin);
//             }
//         }
//     }
// }