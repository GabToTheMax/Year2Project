using System;
using System.Linq;
using GabStuff.Scripts.Singletons;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace GabStuff.Scripts
{
    
    public class PlayerGrabber : MonoBehaviour
    {
        #region variables
        [Header("Grabber settings")]
        [SerializeField] private float reach;
        [SerializeField] private float forceStrength;
        [SerializeField] private float moveCompensation;
        [Range(0,5)] [SerializeField] private int forceMode;
        //[SerializeField] private float rotationStrengthProportion;
        //[SerializeField] private float rotationStrengthDerivative;
        
        [Header("Grabbed object settings")]
        [SerializeField] private float linearDampingStrength;
        //[SerializeField] private float angularDampingStrength; 
        [SerializeField] private float waitTimeBeforeDropOutOfRange;
        
        [Header("Layers")]
        [SerializeField] private LayerMask playerLayerMask;

        public GrabbedObject GrabbedObject { get; private set; }
        public bool CurrentlyGrabbing { get; private set; }
        private Player _player;
        private Vector3 _facingVector;
        private Vector3 _grabbedTarget;
        private bool _isGrabbedObjectInFront;
        private bool _isFirstTime;
        private float _startTime;
        
        #endregion

        private void Start()
        {
            _player = PlayerManager.Instance.GetPlayer();
        }

        public void OnGrab(InputAction.CallbackContext ctx)
        {
            if(!ctx.performed) return;

            if (CurrentlyGrabbing)
            {
                Ungrab();
                return;
            }
            
            if (!Physics.Raycast(
                    _player.Camera.transform.position,
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
            CurrentlyGrabbing = true;
            GrabbedObject = new(hitObject);

            // TODO what if the player has mulitple colliders
            if (PortalManager.Instance.Portalled.Keys.Contains(GrabbedObject.GameObject))
            {
                foreach (Collider c in GrabbedObject.Colliders)
                {
                    Physics.IgnoreCollision(c, _player.Collider, true);
                }
            }
            else
            {
                Physics.IgnoreCollision(GrabbedObject.Colliders[0], _player.Collider, true);
            }

            GrabbedObject.Rigidbody.linearDamping = linearDampingStrength;
            //_grabbedObject.Rigidbody.angularDamping = angularDampingStrength;
            GrabbedObject.Rigidbody.useGravity = false;
        }

        private void Ungrab()
        {
            GrabbedObject.Rigidbody.useGravity = true;
            GrabbedObject.Rigidbody.linearDamping = 0f;
            GrabbedObject.Rigidbody.angularVelocity = Vector3.zero;

            if (PortalManager.Instance.Portalled.Keys.Contains(GrabbedObject.GameObject))
            {
                foreach (Collider c in GrabbedObject.Colliders)
                foreach (PortalledObject p in PortalManager.Instance.Portalled.Values)
                {
                    bool ignore = p.SpiritSideCollider == c || p.OriginalCollider == c;
                    Physics.IgnoreCollision(_player.Collider, c, ignore);
                }
            }
            else
            {
                Physics.IgnoreCollision(GrabbedObject.Colliders[0], _player.Collider, false);
            }
            
            CurrentlyGrabbing = false;
            GrabbedObject = null;
        }

        private void Update()
        {
            _facingVector = _player.Camera.transform.forward;
            Debug.DrawLine (
                _player.Camera.transform.position,
                _player.Position + _facingVector*reach, 
                Color.red
            );

            AffectGrabbedObject();
        }

        private void AffectGrabbedObject()
        {
            if (!CurrentlyGrabbing) return;

            SetGrabbedPosition();
            SetGrabbedRotation();
            CheckIfGrabbedIsTooFar();
        }
        
        private void SetGrabbedPosition()
        {
            RaycastHit[] hits = Physics.RaycastAll(
                _player.Camera.transform.position, 
                _facingVector,
                reach,
                ~playerLayerMask
            );
            
            bool containsWall = false;
            float closestDistance = reach + GrabbedObject.Script.GetRadius();
            RaycastHit closestHit = default;
            foreach (RaycastHit hit in hits)
            {
                GameObject hitObject = hit.transform.gameObject;
                if (hitObject != GrabbedObject.GameObject)
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
                _grabbedTarget = closestHit.point + closestHit.normal.normalized * GrabbedObject.Script.GetRadius();
            else
                _grabbedTarget = _player.Camera.transform.position + _facingVector * reach;

            Vector3 force = -GrabbedObject.Position +
                            _grabbedTarget +
                            _player.Rigidbody.linearVelocity * moveCompensation;
            
            GrabbedObject.Rigidbody.AddForce(
                force*forceStrength,
                (ForceMode)forceMode
            );
        }
        
        private void SetGrabbedRotation()
        {
            Vector3 grabbedToPlayer = _player.GameObject.transform.forward;
            grabbedToPlayer.y = 0;
            GrabbedObject.GameObject.transform.rotation = Quaternion.LookRotation(grabbedToPlayer,Vector3.up);

            /*Quaternion targetRotation = Quaternion.LookRotation(
                grabbedToPlayer,
                _player.Object.transform.up
            );
            Quaternion currentRotation = _grabbedObject.Object.transform.rotation;
            Quaternion torque = targetRotation * Quaternion.Inverse(currentRotation);
            _grabbedObject.Rigidbody.AddTorque(torque.x * rotationStrengthProportion, torque.y * rotationStrengthProportion, torque.z * rotationStrengthProportion, ForceMode.VelocityChange);
            _grabbedObject.Rigidbody.AddTorque(-_grabbedObject.Rigidbody.angularVelocity * rotationStrengthDerivative);*/
        }
        
        private void CheckIfGrabbedIsTooFar()
        {
            /* This checks if a raycast from the target to the grabbed object has nothing in the way of it.
             * If the target is within the object then the raycast will return false
             * If the target is outside of the object, but has line of sight, it will return true, then true
             * If the target is outside of the object, but doesn't have line of sight, it will return true then false
             */
            if (Physics.Linecast(_grabbedTarget, GrabbedObject.Position, out var hit))
            {
                if (hit.transform.gameObject == GrabbedObject.GameObject)
                {
                    _isGrabbedObjectInFront = true;
                    _isFirstTime = true;
                }
                else
                {
                    _isGrabbedObjectInFront = false;
                }
            }
            else
            {
                _isGrabbedObjectInFront = true;
                _isFirstTime = true;
            }
            
            CheckIfGrabbedTooFarForTime();
            
            /*
            print($"Current time: {Time.time}");
            print($"Start time: {_startTime}");
            */
        }

        private void CheckIfGrabbedTooFarForTime()
        {
            if (_isFirstTime)
            {
                _isFirstTime = false;
                _startTime = Time.time;
            }

            if (!_isGrabbedObjectInFront)
            {
                if (Time.time - _startTime > waitTimeBeforeDropOutOfRange)
                {
                    Ungrab();
                }
            }
        }
    }
}
