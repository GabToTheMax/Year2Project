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
        private Vector2 _mouseAim;
        
        
        void Start()
        {
            
        }

        public void OnMouseMove(InputAction.CallbackContext context)
        {
            _mouseAim = context.ReadValue<Vector2>();
            print(_mouseAim);
            playerCamera.transform.RotateAround(playerCamera.transform.position, Vector3.up, _mouseAim.x*xSensitivity);
        }

        void Update()
        {
        }
    }
}
