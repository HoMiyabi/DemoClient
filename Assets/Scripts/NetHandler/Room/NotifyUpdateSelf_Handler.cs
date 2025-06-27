// using Kirara.Network;
// using UnityEngine;
//
// namespace Kirara.NetHandler
// {
//     public class NotifyUpdateSelf_Handler : MsgHandler<NotifyUpdateSelf>
//     {
//         protected override void Run(Session session, NotifyUpdateSelf message)
//         {
//             var ch = PlayerSystem.Instance.FrontRoleCtrl;
//
//             ch.CharacterController.enabled = false;
//             ch.transform.position = message.PosRot.Pos.Unity();
//             ch.transform.eulerAngles = message.PosRot.Rot.Unity();
//             ch.CharacterController.enabled = true;
//
//             Debug.Log($"SetPosRotMessageHandler: {ch.transform.position}, {ch.transform.eulerAngles}");
//         }
//     }
// }