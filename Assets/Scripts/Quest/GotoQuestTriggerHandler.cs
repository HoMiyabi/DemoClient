using System;
using Kirara.Quest;
using UnityEngine;

namespace Quest
{
    public class GotoQuestTriggerHandler : MonoBehaviour
    {
        private GotoQuest _quest;

        public void Set(GotoQuest quest)
        {
            _quest = quest;
        }

        private void OnTriggerEnter(Collider other)
        {
            _quest.OnTriggerEnter();
        }
    }
}