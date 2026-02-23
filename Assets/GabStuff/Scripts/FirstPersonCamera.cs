using UnityEngine;
using UnityEngine.InputSystem;

namespace GabStuff.Scripts
{
    public class FirstPersonCamera : MonoBehaviour
    {
        [SerializeField] private GameObject cameraCentre;
        [SerializeField] private GameObject playerCamera;
        [SerializeField] private float xSensitivity;
        [SerializeField] private float ySensitivity;
        public float facingDirection;
        public Vector3 forwardVector3;
        private Vector2 _mouseAim;
        
        
        void Start()
        {
            
        }

        public void OnMouseMove(InputAction.CallbackContext context)
        {
            _mouseAim = context.ReadValue<Vector2>();
            facingDirection = cameraCentre.transform.rotation.eulerAngles.y;
            transform.RotateAround(transform.position, Vector3.up, _mouseAim.x * xSensitivity);
            
            // UpdateFacingVector();
            // cameraCentre.transform.RotateAround(transform.position, Quaternion.AngleAxis(90, Vector3.up)*forwardVector3, _mouseAim.y * -ySensitivity);
            // var cameraRotation = cameraCentre.transform.rotation;
            Quaternion cameraRotation = new();
            cameraRotation.eulerAngles = new Vector3(cameraCentre.transform.rotation.x + _mouseAim.y * ySensitivity, 0, 0);
            cameraCentre.transform.rotation = cameraRotation;

        }

        void FixedUpdate()
        {
            UpdateFacingVector();
        }

        private void UpdateFacingVector()
        {
            forwardVector3 = Quaternion.AngleAxis(facingDirection, Vector3.up)*Vector3.forward;
            Debug.DrawLine(transform.position + Vector3.zero, transform.position + forwardVector3, Color.yellow);
        }
    }
}
