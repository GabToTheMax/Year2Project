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
        private PlayerGrabbable _grabbedObjectScript;
        private Vector3 _facingVector;

        private void Start()
        {
            _thisPlayer = PlayerManager.Instance.GetPlayer();
        }

        public void OnGrab(InputAction.CallbackContext ctx)
        {
            if(!ctx.performed) return;

            if (_currentlyGrabbing)
            {
                _currentlyGrabbing = false;
                _grabbedObject = null;
                return;
            }
            
            if (!Physics.Raycast(_thisPlayer.Camera.transform.position, _facingVector, out RaycastHit hit, reach)) return;
            
            GameObject hitObject = hit.transform.gameObject;
            
            if (hitObject.GetComponent<PlayerGrabbable>() != null)
            {
                _currentlyGrabbing = true;
                _grabbedObject = hitObject;
                _grabbedObjectScript = hitObject.GetComponent<PlayerGrabbable>();
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

            RaycastHit[] hits = Physics.SphereCastAll(_thisPlayer.Camera.transform.position, _grabbedObjectScript.GetRadius(), _facingVector, reach);
            
            if(hits.Length == 0)
            {
                print("It is trying to move the grabbed object to the floating position");
                _grabbedObject.transform.position = _thisPlayer.Camera.transform.position + _facingVector * reach;
            }

            int closestIndex = 0;
            float closestDistance = 0;
            for (var i = 0; i < hits.Length; i++)
            {
                var hit = hits[i];
                if (hit.transform.gameObject == _grabbedObject) continue;
                if (hit.distance < closestDistance)
                {
                    closestDistance = hit.distance;
                    closestIndex = i;
                }
            }
            
            _grabbedObject.transform.position = hits[closestIndex].point + hits[closestIndex].normal * _grabbedObjectScript.GetRadius();
        }
    }
}
