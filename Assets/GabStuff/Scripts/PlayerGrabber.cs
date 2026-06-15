using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace GabStuff.Scripts
{
    public class GrabbedObject
    {
        public GameObject Object;
        public PlayerGrabbable Script;
        public Rigidbody Rigidbody;
        public Vector3 Position => Object.transform.position;
        
        public GrabbedObject(GameObject g, PlayerGrabbable s, Rigidbody r)
        {
            Object = g;
            Script = s;
            Rigidbody = r;
        }

    }
    
    public class PlayerGrabber : MonoBehaviour
    {
        [SerializeField] private float reach;
        [SerializeField] private LayerMask playerLayerMask;
        [SerializeField] private LayerMask grabbedObjectLayerMask;
        private Player _thisPlayer;
        private bool _currentlyGrabbing;
        private GrabbedObject _grabbedObject;
        private Vector3 _facingVector;

        private void Start()
        {
            _thisPlayer = PlayerManager.Instance.GetPlayer();
            Physics.IgnoreLayerCollision(7, 8);
        }

        public void OnGrab(InputAction.CallbackContext ctx)
        {
            if(!ctx.performed) return;

            if (_currentlyGrabbing)
            {
                _grabbedObject.Rigidbody.linearVelocity = Vector3.zero;
                _grabbedObject.Object.layer = 0;
                _currentlyGrabbing = false;
                _grabbedObject = null;
                return;
            }
            
            if (!Physics.Raycast(_thisPlayer.Camera.transform.position, _facingVector, out RaycastHit hit, reach)) return;
            
            GameObject hitObject = hit.transform.gameObject;
            
            if (hitObject.GetComponent<PlayerGrabbable>() != null)
            {
                _currentlyGrabbing = true;
                _grabbedObject = new(hitObject, hitObject.GetComponent<PlayerGrabbable>(), hitObject.GetComponent<Rigidbody>());
                _grabbedObject.Object.layer = 7;
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

            SetGrabbedObjectPosition();
            SetGrabbedObjectRotation();
        }

        private void SetGrabbedObjectRotation()
        {
            _grabbedObject.Object.transform.rotation = _thisPlayer.Object.transform.rotation;
        }
        
        private void SetGrabbedObjectPosition()
        {
            RaycastHit[] hits = Physics.SphereCastAll(_thisPlayer.Camera.transform.position, _grabbedObject.Script.GetRadius(), _facingVector, reach, ~playerLayerMask);
            
            bool containsWall = false;
            float closestDistance = reach + _grabbedObject.Script.GetRadius();
            RaycastHit closestHit = default;
            foreach (RaycastHit hit in hits)
            {
                GameObject hitObject = hit.transform.gameObject;
                if (hitObject != _grabbedObject.Object)
                {
                    containsWall = true;
                    if(hit.distance <= closestDistance)
                    {
                        closestDistance = hit.distance;
                        closestHit = hit;
                    }
                }
            }
            
            print(hits.Length);
            
            if (containsWall)
            {
                _grabbedObject.Object.transform.position = closestHit.point + closestHit.normal.normalized*_grabbedObject.Script.GetRadius();
            }
            else
            {
                _grabbedObject.Object.transform.position = _thisPlayer.Camera.transform.position + _facingVector*reach;
            }
        }
    }
}
