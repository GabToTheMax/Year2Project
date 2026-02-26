using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GabStuff.Scripts
{
    public class PlayerMovement : MonoBehaviour
    {
        // TODO Deal with linear drag making gravity slow
        
        #region Variables
        [SerializeField] private float moveRate;
        [SerializeField] private float sprintRate;
        private Rigidbody _rb;
        private Vector3 _moveDirection;
        private Vector3 _smoothMove;
        private FPSCamera _fpsCamera;
        private readonly Dictionary<string, float> _speedModifiers = new();
        #endregion

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _fpsCamera = GetComponent<FPSCamera>();
            _speedModifiers.Add("Base", moveRate);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _moveDirection = context.ReadValue<Vector2>();
            _moveDirection.z = _moveDirection.y;
            _moveDirection.y = 0;
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _speedModifiers.Add("Sprint", sprintRate);
            }

            if (context.canceled)
            {
                _speedModifiers.Remove("Sprint");
            }
        }
    
        private void FixedUpdate()
        {
            var forwardRotation = Quaternion.AngleAxis(_fpsCamera.playerDirection, Vector3.up);
            
            var forwardVector = forwardRotation * _moveDirection * _speedModifiers.Values.Sum();
            
            Debug.DrawLine(transform.position, transform.position + forwardVector, Color.red);
            _smoothMove = Vector3.Lerp(_smoothMove, forwardVector, 0.1f);
            
            _rb.AddForce(_smoothMove, ForceMode.VelocityChange);
        }
    }
}
