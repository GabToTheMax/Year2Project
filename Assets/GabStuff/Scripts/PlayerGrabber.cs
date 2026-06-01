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
            
            if (!Physics.Raycast(_thisPlayer.Camera.transform.position, _facingVector, out RaycastHit hit, reach)) return;
            GameObject hitObject = hit.transform.gameObject;
            
            if (hitObject.GetComponent<PlayerGrabbable>() != null)
            {
                _currentlyGrabbing = true;
                _grabbedObject = hitObject;
            }
        }
        
        private void Update()
        {
            _facingVector = _thisPlayer.Camera.transform.forward;
            Debug.DrawLine(_thisPlayer.Camera.transform.position, _thisPlayer.Position + _facingVector*reach, Color.red);

            MoveGrabbedObject();
        }

        private void MoveGrabbedObject()
        {
            if (!_currentlyGrabbing) return; 

            RaycastHit[] hits = Physics.RaycastAll(_thisPlayer.Camera.transform.position, _facingVector, reach);
            
            var objectInTheWay = false;
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.gameObject == _grabbedObject) continue;
                objectInTheWay = true;
                _grabbedObject.transform.position = hit.point;
                break;
            }

            print("It reached this code");
            if (!objectInTheWay)
            {
                print("It is trying to move the grabbed object to the floating position");
                _grabbedObject.transform.position = _thisPlayer.Camera.transform.position + _facingVector * reach;
            }

            /*
            if (_currentlyGrabbing)
            {
                RaycastHit hit;
                if(!Physics.Raycast(_thisPlayer.Camera.transform.position, _facingVector, out hit, reach)) return;
                if (hit.transform.gameObject == _grabbedObject)
                {
                    _grabbedObject.transform.position = _thisPlayer.Camera.transform.position + _facingVector * reach;
                }
                else
                {
                    _grabbedObject.transform.position = hit.point;
                }
            }*/
        }
    }
}
