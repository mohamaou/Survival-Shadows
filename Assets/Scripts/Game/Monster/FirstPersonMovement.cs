using System;
using Game.UI;
using UnityEngine;

namespace Game.Monster
{
    public class FirstPersonMovement : MonoBehaviour
    {
        [SerializeField] private Rigidbody rb;
        [SerializeField] private FirstPersonCameraMovement firstPersonCamera;
        [SerializeField] private MonsterAnimationManager anim;
        [SerializeField] [Range(0,6)] private float speed = 3f;
        
        
        
        private void FixedUpdate()
        {
            var dir = ControlDirection();
            var movementDirection = (firstPersonCamera.GetCameraForwardDirection() * dir.y + firstPersonCamera.GetCameraRight() * dir.x).normalized;
            rb.MovePosition(rb.position + movementDirection * (speed * Time.fixedDeltaTime));
        }

        private void Update()
        {
            anim.SetWalk(ControlDirection() != Vector2.zero);
            anim.SetMoveDirection((ControlDirection()*15).normalized);
        }

        private Vector2 ControlDirection()
        {
            if (DynamicJoystick.Instance == null) return Vector2.zero;
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                float horizontalInput = 0;
                float verticalInput = 0;

                if (Input.GetKey(KeyCode.A))
                {
                    horizontalInput = -1;
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    horizontalInput = 1;
                }

                if (Input.GetKey(KeyCode.W))
                {
                    verticalInput = 1;
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    verticalInput = -1;
                }
                return  new Vector2(horizontalInput, verticalInput);
            }
            return DynamicJoystick.Instance.Direction;
        }
        
    }
}

