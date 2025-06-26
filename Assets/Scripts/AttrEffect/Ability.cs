using System.Collections.Generic;
using UnityEngine;

namespace Kirara.AttrEffect
{
    public class Ability
    {
        public AttrEffect ae;

        public readonly string name;
        public readonly Effect effect;

        // 触发
        public readonly bool triggerOnAdd;
        public readonly List<string> eventNames;
        public readonly float triggerInterval;
        public float TriggerTimer { get; private set; }

        public Ability(string name, Effect effect,
            bool triggerOnAdd = true, List<string> eventNames = null, float triggerInterval = 0f)
        {
            this.name = name;
            this.effect = effect;

            this.triggerOnAdd = triggerOnAdd;
            this.eventNames = eventNames;
            this.triggerInterval = triggerInterval;
        }

        public virtual void OnAdded()
        {
            if (triggerOnAdd)
            {
                Trigger();
            }
        }

        private void Trigger()
        {
            ae.ApplyEffect(effect);
            TriggerTimer = triggerInterval;
        }

        public void WakeUp()
        {
            if (TriggerTimer <= 0f)
            {
                Trigger();
            }
        }

        public void Update()
        {
            if (TriggerTimer > 0)
            {
                TriggerTimer = Mathf.Max(0f, TriggerTimer - Time.deltaTime);
            }
        }
    }
}