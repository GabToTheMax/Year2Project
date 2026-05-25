using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GabStuff.Scripts
{
    public class PlayerGrabber : MonoBehaviour
    {
        [SerializeField] private float reach;
        private Player _thisPlayer;
        private bool _currentlyGrabbing;
        private GameObject _grabbedObject;
        private Vector3 _facingVector;

        private void Start()
        {
            _thisPlayer = PlayerManager.Instance.GetPlayer();
        }

        public void OnGrab(InputAction.CallbackContext ctx)
        {
            if(!ctx.performed) return;
         
            _currentlyGrabbing = false;
            _grabbedObject = null;
            
            RaycastHit hit;
            if (!Physics.Raycast(_thisPlayer.Camera.transform.position, _facingVector, out hit, reach)) return;
            GameObject hitObject = hit.transform.gameObject;
            if (hitObject.GetComponent<PlayerGrabbable>() == null) return;
            _currentlyGrabbing = true;
            _grabbedObject = hitObject;
        }
        
        private void Update()
        {
            _facingVector = _thisPlayer.Camera.transform.forward;
            Debug.DrawLine(_thisPlayer.Camera.transform.position, _thisPlayer.Position + _facingVector*reach, Color.red);
            
            if (_currentlyGrabbing)
            {
                _grabbedObject.transform.position = _thisPlayer.Position + _facingVector * reach;
            }
        }
    }
}
