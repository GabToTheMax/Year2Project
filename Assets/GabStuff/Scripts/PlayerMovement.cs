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
        private Vector3 _moveDirection;
        private Vector3 _smoothMove;
        private readonly Dictionary<string, float> _speedModifiers = new();
        private Player _player;
        #endregion

        private void Start()
        {
            _player = PlayerManager.Instance.GetPlayer();
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
            var forwardRotation = Quaternion.AngleAxis(_player.CameraScript.playerDirection, Vector3.up);
            
            var forwardVector = forwardRotation * _moveDirection * _speedModifiers.Values.Sum();
            
            _smoothMove = Vector3.Lerp(_smoothMove, forwardVector, 0.1f);
            
            Debug.DrawLine(transform.position, transform.position + forwardVector, Color.red);
            _player.Rigidbody.AddForce(_smoothMove, ForceMode.VelocityChange);
        }

        public void Halt()
        {
            _smoothMove = Vector3.zero;
            _moveDirection = Vector3.zero;
            _player.Rigidbody.linearVelocity = Vector3.zero;
        }
    }
}
