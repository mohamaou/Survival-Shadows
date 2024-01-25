using Cinemachine;
using Game.UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Survivors
{
 
    public class ThirdPersonMovement : NetworkBehaviour
    {
        [SerializeField] private NetworkSurvivor networkSurvivor;
        [SerializeField] private new SurvivorAnimation animation;
        private float freezeAnimation;

        [Header("Player Movement")]
        [SerializeField] private Rigidbody rb;
        [SerializeField] private float speed = 1f;
    
        [Header("Camera")]
        [SerializeField] private CinemachineFreeLook freeLook;
        [SerializeField] private float ySpeed, xSpeed;

        [HideInInspector] public bool active;
        


        private void Start()
        {
            active = true;
            if (!IsOwner)
            {
                Destroy(freeLook.gameObject);
                enabled = false;
            }
        }

        public void FixedUpdate()
        {
            AvatarMovement();
        }
        public void LateUpdate()
        {
            CameraMovement();
        }
        private void CameraMovement()
        {

            if (!GameManager.Instance.waitRoom && GameManager.State != GameState.Play) return;
            if (TouchControle.Instance == null) return;
            freeLook.m_XAxis.Value += TouchControle.Instance.TouchDist.x * Time.deltaTime * 180 * ySpeed/10;
            freeLook.m_YAxis.Value -= TouchControle.Instance.TouchDist.y * Time.deltaTime * xSpeed/10;
        }
        private void AvatarMovement()
        {
            if(!active || DynamicJoystick.Instance == null)return;
            var movementDirection = new Vector3(DynamicJoystick.Instance.Direction.x,0,DynamicJoystick.Instance.Direction.y);
            if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
            {
                movementDirection.z = Input.GetAxis("Vertical");
                movementDirection.x = Input.GetAxis("Horizontal");
            }
            var dir =movementDirection;
            animation.Movement(dir != Vector3.zero, Running());
            var localSpeed = dir == Vector3.zero ? 0 :speed;
            if (dir != Vector3.zero)
            {
                if (networkSurvivor.IsReviving) networkSurvivor.CancelRevive();
                var targetRotation = Quaternion.LookRotation(dir) * Quaternion.AngleAxis(freeLook.m_XAxis.Value, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 6* Time.deltaTime);
                localSpeed *= Running() ? 1f : 0.3f;
            }
            var velocity = new Vector3(transform.forward.x * localSpeed, rb.velocity.y, transform.forward.z * localSpeed);
            rb.velocity = Vector3.Lerp(rb.velocity, velocity, 6* Time.deltaTime);
        }

        public bool IsMoving()
        {
            if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0) return true;
            return DynamicJoystick.Instance.Direction.magnitude > 0;
        }
        private bool Running()
        {
            if (networkSurvivor.SurvivorState != SurvivorState.Normal) return false;
            var movementDirection = DynamicJoystick.Instance.Direction;
            if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
            {
                movementDirection.y = Input.GetAxis("Vertical");
                movementDirection.x = Input.GetAxis("Horizontal");
            }
            var dir =movementDirection;
            return dir.magnitude  > 0.5f;
        }
        
    }
}
