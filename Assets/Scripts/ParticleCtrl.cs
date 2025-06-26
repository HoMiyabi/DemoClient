using System.Collections.Generic;
using UnityEngine;

namespace Kirara
{
    public class ParticleCtrl : MonoBehaviour
    {
        public List<ParticleSystem> rotParticleSystems = new();

        public void Set(Vector3 forward, float rotMinDeg, float rotMaxDeg)
        {
            transform.forward = forward;
            foreach (var ps in rotParticleSystems)
            {
                var main = ps.main;
                main.startRotation = new ParticleSystem.MinMaxCurve(rotMinDeg * Mathf.Deg2Rad, rotMaxDeg * Mathf.Deg2Rad);
            }
        }
    }
}