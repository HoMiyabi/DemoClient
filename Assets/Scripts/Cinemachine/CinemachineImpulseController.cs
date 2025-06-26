using Cinemachine;
using UnityEngine;

namespace Kirara
{
    public class CinemachineImpulseController : UnitySingleton<CinemachineImpulseController>
    {
        [SerializeField] private CinemachineImpulseSource impulseSource;

        public void GenerateImpulse(Vector3 velocity)
        {
            if (velocity == Vector3.zero) return;

            impulseSource.GenerateImpulseWithVelocity(velocity);
        }

        public void GenerateImpulse(float angle, float speed)
        {
            if (speed == 0f) return;

            // 速度反向，因为Explosion的曲线开始是反的

            float rad = angle * Mathf.Deg2Rad;
            var velocity = -speed * new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            Instance.GenerateImpulse(velocity);
        }
    }
}