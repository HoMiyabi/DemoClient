using System;
using UnityEngine;

namespace Kirara
{
    public class ChGravity : MonoBehaviour
    {
        public float gravity = -15;
        public float verticalVelocity;
        public float minVerticalVelocity = -8;

        private CharacterController characterController;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        public void Update()
        {
            if (characterController.isGrounded)
            {
                verticalVelocity = 0;
                characterController.Move(new Vector3(0, -1, 0) * Time.deltaTime);
            }
            else
            {
                verticalVelocity += gravity * Time.deltaTime;
                verticalVelocity = Math.Max(verticalVelocity, minVerticalVelocity);
                characterController.Move(verticalVelocity * Time.deltaTime * Vector3.up);
            }
        }
    }
}