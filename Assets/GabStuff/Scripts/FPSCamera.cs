using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GabStuff.Scripts
{
    public class FPSCamera : MonoBehaviour
    {
        #region Variables
        [SerializeField] private GameObject cameraGameObject;
        [SerializeField] private GameObject cameraVertical;
        [SerializeField] private float cameraSensitivityX;
        [SerializeField] private float cameraSensitivityY;
        [SerializeField] private float fieldOfView = 90;
        [SerializeField] private float smoothing;
        [HideInInspector] public float playerDirection;
        [HideInInspector] public Vector3 playerDirectionVector;
        private Vector2 _mouseInput;
        private Vector2 _smoothMouseInput;
        private Camera _camera;
        private float _xRotation;
        private float _yRotation;
        private Camera[] _portalCameras = new Camera[2];
        #endregion
        
        private void Start()
        {
            _camera = cameraGameObject.GetComponent<Camera>();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            for (int i = 0; i < _portalCameras.Length; i++)
            {
                _portalCameras[i] = cameraGameObject.AddComponent<Camera>();
            }
        }
        
        public void OnCameraMove(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                 _mouseInput = context.ReadValue<Vector2>();
            }
        }

        private void Update()
        {
            SmoothCamera();
            RotateX();
            RotateY();
            UpdateFov();
        }

        private void SmoothCamera()
        {
            _smoothMouseInput = Vector2.Lerp(_smoothMouseInput, _mouseInput, smoothing);
        }

        private void UpdateFov()
        {
            if (!Mathf.Approximately(_camera.fieldOfView, fieldOfView))
            {
                _camera.fieldOfView = fieldOfView;
                foreach (var portalCamera in _portalCameras)
                {
                    portalCamera.fieldOfView = fieldOfView;
                }
            }
        }
        
        private void RotateX()
        {
            _xRotation += _smoothMouseInput.x * cameraSensitivityX;
            playerDirection = _xRotation;
            cameraGameObject.transform.rotation = Quaternion.Euler
            (
                cameraGameObject.transform.rotation.eulerAngles.x,
                _xRotation,
                cameraGameObject.transform.rotation.eulerAngles.z
            );

            playerDirectionVector = Quaternion.AngleAxis(playerDirection, Vector3.up) * Vector3.forward;
            Debug.DrawLine(transform.position, transform.position + playerDirectionVector, Color.black);
        }

        private void RotateY()
        {
            _yRotation += -_smoothMouseInput.y * cameraSensitivityY;
            _yRotation = Mathf.Clamp(_yRotation, -90f, 90f);
            cameraGameObject.transform.rotation = Quaternion.Euler
            (
                _yRotation, 
                cameraGameObject.transform.rotation.eulerAngles.y, 
                cameraGameObject.transform.rotation.eulerAngles.z
            );
        }
    }
}
