using System;
using System.Collections;
using UnityEngine;

namespace Game.Monster
{
    public class FirstPersonCameraMovement : MonoBehaviour
    {
        [SerializeField] private bool editor;
        [SerializeField] private new Transform camera;
        [SerializeField] private Transform  pivot, thirdPersonBody;
        [SerializeField] private float rotateSpeed;
        [SerializeField] private FirstPersonCameraMovementEditor editorMovement;
        private Vector3 _touchStartPos;
        
        private bool _isTouching;
        private bool _working;
        

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.1f);
            _working = true;
            editorMovement.enabled = editor;
        }

        private void Update()
        {
            if(!_working)return;
            CameraRotation();
        }

        private void LateUpdate()
        {
            PlayerRotation();
        }

        private void PlayerRotation()
        {
            pivot.rotation = camera.rotation;
            var targetPoint = camera.forward + camera.position;
            targetPoint.y = thirdPersonBody.position.y;
            var dir = -thirdPersonBody.position + targetPoint;
            thirdPersonBody.rotation = Quaternion.LookRotation(dir);
        }

        private void CameraRotation()
        {
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                for (int i = 0; i < Input.touchCount; i++)
                {
                    if (Input.GetTouch(i).position.x > Screen.width / 2)
                    {
                        touch = Input.GetTouch(i);
                        break;
                    }
                }

                if (touch.phase == TouchPhase.Began)
                {
                    if (touch.position.x > Screen.width / 2)
                    {
                        _isTouching = true;
                    }
                }
                else if (touch.phase == TouchPhase.Moved && _isTouching)
                {
                    var deltaX = touch.deltaPosition.x * rotateSpeed;
                    var deltaZ = touch.deltaPosition.y * rotateSpeed;
                    camera.Rotate(Vector3.up, deltaX);
                    camera.Rotate(Vector3.left, deltaZ);

                    // Limit pitch to prevent looking too far up or down
                    var xRotation = ClampAngle(camera.eulerAngles.x); // Adjust the pitch limit as needed
                    camera.eulerAngles = new Vector3(xRotation, camera.eulerAngles.y, 0);
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    _isTouching = false;
                }
            }
        }
        
        float ClampAngle(float angle)
        {
            return angle < 200 ? Mathf.Clamp(angle, 0, 60) : Mathf.Clamp(angle, 300, 360);
        }



        public void Stop(bool stop)
        {
            enabled = !stop;
            editorMovement.enabled = !stop && editor;
        }

        public Vector3 GetCameraForwardDirection()
        {
            var cameraForward = camera.forward;
            cameraForward.y = 0;
            return cameraForward;
        }
        public Vector3 GetCameraRight() => camera.right;
        
    }
}
