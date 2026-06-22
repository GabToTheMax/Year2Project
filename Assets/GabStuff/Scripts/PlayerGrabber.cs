using System;
using System.Linq;
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
        [SerializeField] private float forceStrength;
        [SerializeField] private float linearDampingStrength;
        [SerializeField] private int playerLayer;
        [SerializeField] private int grabbedObjectLayer;
        private Player _thisPlayer;
        private bool _currentlyGrabbing;
        private GrabbedObject _grabbedObject;
        private Vector3 _facingVector;
        private Vector3 _grabbedTarget;
        
        private bool isGrabbedObjectInFront;
        private bool isFirstTime;
        private float startTime;
            

        private void Start()
        {
            _thisPlayer = PlayerManager.Instance.GetPlayer();
            Physics.IgnoreLayerCollision(grabbedObjectLayer, playerLayer);
        }

        public void OnGrab(InputAction.CallbackContext ctx)
        {
            if(!ctx.performed) return;

            if (_currentlyGrabbing)
            {
                Ungrab();
                return;
            }
            
            if (!Physics.Raycast(
                    _thisPlayer.Camera.transform.position,
                    _facingVector,
                    out RaycastHit hit,
                    reach
                )
            ) return;
            
            GameObject hitObject = hit.transform.gameObject;
            
            if (hitObject.GetComponent<PlayerGrabbable>() != null)
            {
                Grab(hitObject);
            }
        }

        private void Grab(GameObject hitObject)
        {
            _currentlyGrabbing = true;
            _grabbedObject = new(
                hitObject,
                hitObject.GetComponent<PlayerGrabbable>(),
                hitObject.GetComponent<Rigidbody>()
            );
            _grabbedObject.Object.layer = grabbedObjectLayer;
            _grabbedObject.Rigidbody.linearDamping = linearDampingStrength;
            _grabbedObject.Rigidbody.useGravity = false;
        }

        private void Ungrab()
        {
            _grabbedObject.Rigidbody.useGravity = true;
            _grabbedObject.Rigidbody.linearDamping = 0;
            _grabbedObject.Object.layer = 0;
            _currentlyGrabbing = false;
            _grabbedObject = null;
        }

        private void Update()
        {
            _facingVector = _thisPlayer.Camera.transform.forward;
            Debug.DrawLine (
                _thisPlayer.Camera.transform.position,
                _thisPlayer.Position + _facingVector*reach, 
                Color.red
            );

            MoveGrabbedObject();
        }

        private void MoveGrabbedObject()
        {
            if (!_currentlyGrabbing) return;

            SetGrabbedPosition();
            SetGrabbedRotation();
            CheckIfGrabbedIsTooFar();
        }

        private void SetGrabbedRotation()
        {
            Vector3 grabbedToPlayer = _thisPlayer.Position - _grabbedObject.Position;
            grabbedToPlayer.y = 0;
            _grabbedObject.Object.transform.rotation = Quaternion.LookRotation(
                grabbedToPlayer,
                _thisPlayer.Object.transform.up
            );
        }
        
        private void SetGrabbedPosition()
        {
            RaycastHit[] hits = Physics.RaycastAll(
                _thisPlayer.Camera.transform.position, 
                _facingVector, reach, ~playerLayerMask
            );
            
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
            
            if (containsWall)
                _grabbedTarget = closestHit.point + closestHit.normal.normalized*_grabbedObject.Script.GetRadius();
            else
                _grabbedTarget = _thisPlayer.Camera.transform.position + _facingVector*reach;

            Vector3 force = -_grabbedObject.Position+_grabbedTarget;
            _grabbedObject.Rigidbody.AddForce(
                force*forceStrength + _thisPlayer.Rigidbody.linearVelocity,
                ForceMode.Impulse
            );
        }

        private void CheckIfGrabbedIsTooFar()
        {
            // Check all objects in front of it in its reach
            // If the gameobject is in that:
                // Set isGrabbedObjectInFront to true
            // if the gameobject is not in that:
                // Set isGrabbedObjectInFront to false
                
            // every tick:
            // If firstTick
            //      startTime = ...
            // If isGrabbedObjectInFront is true
            //      if 0.5 seconds have passed since startTime
            //      let go
            
            
            Ray camToObject = new
            (
                _thisPlayer.Camera.transform.position, 
                _grabbedObject.Position
                    -_thisPlayer.Camera.transform.position
            );
            
            RaycastHit[] hits = Physics.RaycastAll(
                camToObject,
                reach
            );

            isGrabbedObjectInFront = false;
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.gameObject == _grabbedObject.Object)
                    isGrabbedObjectInFront = true;
            }

            if (isGrabbedObjectInFront)
            {
                isFirstTime = true;
                isGrabbedObjectInFront = true;
            }
            
            CheckIfGrabbedTooFarForTime();
            
            print($"Current time: {Time.time}");
            print($"Start time: {startTime}");
        }

        private void CheckIfGrabbedTooFarForTime()
        {
            if (isFirstTime)
            {
                isFirstTime = false;
                startTime = Time.time;
            }

            if (!isGrabbedObjectInFront)
            {
                if (Time.time - startTime > 0.5f)
                {
                    Ungrab();
                }
            }
        }
    }
}
