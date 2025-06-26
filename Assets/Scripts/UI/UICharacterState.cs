using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Kirara.UI
{
    public class UICharacterState : MonoBehaviour
    {
        public TextMeshProUGUI[] tmps;
        public int count = 10;

        private Queue<string>[] ques = new Queue<string>[3];

        public UICharacterState Set(List<ChCtrl> characters)
        {
            // for (int i = 0; i < PlayerSystem.Instance.ChCtrls.Count; i++)
            // {
            //     var ch = PlayerSystem.Instance.ChCtrls[i];
            //     var tmp = tmps[i];
            //     ques[i] = new Queue<string>();
            //     var que = ques[i];
            //     ch.combatStateMachine.onStateChanged += state =>
            //     {
            //         if (que.Count == count)
            //         {
            //             que.Dequeue();
            //         }
            //         que.Enqueue($"{ch.characterName}: {state.GetType().Name}");
            //         tmp.text = string.Join('\n', que);
            //     };
            // }
            // for (int i = PlayerSystem.Instance.ChCtrls.Count; i < tmps.Length; i++)
            // {
            //     tmps[i].gameObject.SetActive(false);
            // }
            return this;
        }
    }
}