using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace GabStuff.Scripts
{
    public class PortalTeleportationScript : MonoBehaviour
    {
        #region variables
        private Portal _thisPortal;
        private Portal _otherPortal;
        private Collider _portalCollider;
        private Player _player;
        private Quaternion _portalRotationDifference;
        private Vector3 _portalToObject;
        private Vector3 _otherPortalToMirror;
        private Dictionary<GameObject, MirrorObject> _mirrors = PortalManager.Instance.GetMirrors();
        #endregion
        
        private void Start()
        {
            _mirrors = PortalManager.Instance.GetMirrors();
            _thisPortal = GetComponent<PortalScript>().ThisPortal;
            _otherPortal = PortalManager.Instance.GetPortal(_thisPortal);
            _portalCollider = _thisPortal.Object.GetComponent<Collider>();
            _player = PlayerManager.Instance.GetPlayer();
        }
        
        /*
         *  I need to check if the player's center is across the portal, only then teleport them, to stop a teleport
         *  loop. So, for each frame where there is something in the collision, check if one of the colliders is the player. If so, teleport.
         */

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Teleportable>() && !_mirrors.ContainsKey(other.gameObject))
            {
                print("Something entered my collision. Sincerely portal #" + _thisPortal.Index);
                Mesh otherMesh = other.gameObject.GetComponent<MeshFilter>().mesh;
                Material otherMaterial = other.gameObject.GetComponent<MeshRenderer>().material;

                var component = other.GetComponent<Collider>();
                var colliderType = component.GetType();
                
                _mirrors.Add(other.gameObject, new MirrorObject(otherMesh, otherMaterial, colliderType, component));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_mirrors.Keys.Contains(other.gameObject) && !_mirrors[other.gameObject].IsTeleporting)
            {
                print("Something exited my collision. Sincerely portal #" + _thisPortal.Index);
                Destroy(_mirrors[other.gameObject].MirrorGameObject);
                _mirrors.Remove(other.gameObject);
            }
            else if (_mirrors.Keys.Contains(other.gameObject) && _mirrors[other.gameObject].IsTeleporting)
            {
                _mirrors[other.gameObject].IsTeleporting = false;
            }
        }
        
        private void OnTriggerStay(Collider collision)
        {
            if (!_mirrors.Keys.Contains(collision.gameObject)) return;

            MoveTheMirror(collision.gameObject);
            
            var sphereColliders = Physics.OverlapSphere(collision.transform.position, 0);
            if (sphereColliders.Contains(_portalCollider))
            {
                Teleport(collision.gameObject);
            }
        }

        private void MoveTheMirror(GameObject objectToMove)
        {
            MirrorObject mirror = _mirrors[objectToMove];
            
            _portalRotationDifference = _thisPortal.Script.PortalRotationDifference;
            _portalToObject = objectToMove.transform.position - _thisPortal.Position;
            _otherPortalToMirror = _portalRotationDifference * _thisPortal.Script.Flip180 * _portalToObject;
            
            mirror.SetMirrorPosition(_otherPortalToMirror, _otherPortal);
            mirror.SetMirrorRotation(_portalRotationDifference, objectToMove.transform.rotation, _otherPortal.Object);
        }
        
        public void Teleport(GameObject objectToTeleport)
        {
            _mirrors[objectToTeleport].IsTeleporting = true;
            if (objectToTeleport == _player.Object)
            {
                _player.CameraScript.AddXRotation(180f + _portalRotationDifference.eulerAngles.y);
                _player.MovementScript.RotateMomentum(Quaternion.AngleAxis(180f, Vector3.up) * _portalRotationDifference);
            }
            else
            {
                objectToTeleport.transform.rotation = Quaternion.AngleAxis(180f, _otherPortal.Object.transform.up) * _portalRotationDifference * objectToTeleport.transform.rotation;
                objectToTeleport.GetComponent<Teleportable>().RotateMomentum(Quaternion.AngleAxis(180f, _thisPortal.Object.transform.up) * _portalRotationDifference);
            }
            
            objectToTeleport.transform.position = _otherPortal.Position + _otherPortalToMirror;
            MoveTheMirror(objectToTeleport);
        }
        
        
    }
}
