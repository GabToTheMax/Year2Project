using System;
using System.Linq;
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
        [SerializeField] private int playerLayer;
        [SerializeField] private int grabbedObjectLayer;
        
        private Player _player;
        private bool _currentlyGrabbing;
        private GrabbedObject _grabbedObject;
        private Vector3 _facingVector;
        private Vector3 _grabbedTarget;
        private bool _isGrabbedObjectInFront;
        private bool _isFirstTime;
        private float _startTime;
        
        #endregion

        private void Start()
        {
            _player = PlayerManager.Instance.GetPlayer();
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
            _currentlyGrabbing = true;
            _grabbedObject = new(
                hitObject,
                hitObject.GetComponent<PlayerGrabbable>(),
                hitObject.GetComponent<Rigidbody>()
            );
            _grabbedObject.Object.layer = grabbedObjectLayer;
            _grabbedObject.Rigidbody.linearDamping = linearDampingStrength;
            //_grabbedObject.Rigidbody.angularDamping = angularDampingStrength;
            _grabbedObject.Rigidbody.useGravity = false;
        }

        private void Ungrab()
        {
            _grabbedObject.Rigidbody.useGravity = true;
            _grabbedObject.Rigidbody.linearDamping = 0f;
            //_grabbedObject.Rigidbody.angularDamping = 0.05f;
            _grabbedObject.Object.layer = 0;
            _currentlyGrabbing = false;
            _grabbedObject = null;
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
            if (!_currentlyGrabbing) return;

            SetGrabbedPosition();
            SetGrabbedRotation();
            CheckIfGrabbedIsTooFar();
        }
        
        private void SetGrabbedPosition()
        {
            RaycastHit[] hits = Physics.RaycastAll(
                _player.Camera.transform.position, 
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
                _grabbedTarget = closestHit.point + closestHit.normal.normalized * _grabbedObject.Script.GetRadius();
            else
                _grabbedTarget = _player.Camera.transform.position + _facingVector * reach;

            Vector3 force = -_grabbedObject.Position +
                            _grabbedTarget +
                            _player.Rigidbody.linearVelocity * moveCompensation;
            
            _grabbedObject.Rigidbody.AddForce(
                force*forceStrength,
                (ForceMode)forceMode
            );
        }
        
        private void SetGrabbedRotation()
        {
            Vector3 grabbedToPlayer = _player.Object.transform.forward;
            grabbedToPlayer.y = 0;
            _grabbedObject.Object.transform.rotation = Quaternion.LookRotation(grabbedToPlayer,Vector3.up);

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
            if (Physics.Linecast(_grabbedTarget, _grabbedObject.Position, out var hit))
            {
                if (hit.transform.gameObject == _grabbedObject.Object)
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
