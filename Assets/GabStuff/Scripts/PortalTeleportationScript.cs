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
        private Dictionary<GameObject, MirrorObject> _mirrors;
        private Player _player;
        #endregion
        
        private void Start()
        {
            _mirrors = new Dictionary<GameObject, MirrorObject>();
            _thisPortal = GetComponent<PortalScript>().ThisPortal;
            _otherPortal = PortalManager.Instance.GetPortal(_thisPortal);
            _portalCollider = _thisPortal.Object.GetComponent<Collider>();
            _player = PlayerManager.Instance.GetPlayer();
        }
        
        /*
         *  I need to check if the player's center is across the portal, only then teleport them, to stop a teleport
         *  loop. So, for each frame where there is something in the collision, check if one of the colliders is the player. If so, halt and teleport.
         */

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Teleportable>())
            {
                Mesh otherMesh = other.gameObject.GetComponent<MeshFilter>().mesh;
                Material otherMaterial = other.gameObject.GetComponent<MeshRenderer>().material;

                var collider = other.GetComponent<Collider>();
                var colliderType = collider.GetType();
                
                _mirrors.Add(other.gameObject, new MirrorObject(otherMesh, otherMaterial, colliderType, collider));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_mirrors.Keys.Contains(other.gameObject))
            {
                Destroy(_mirrors[other.gameObject].MirrorGameObject);
                _mirrors.Remove(other.gameObject);
            }
        }
        
        private void OnTriggerStay(Collider collision)
        {
            if (!_mirrors.Keys.Contains(collision.gameObject)) return;

            MirrorObject mirror = _mirrors[collision.gameObject];
            
            Quaternion portalRotationDifference = _thisPortal.Script.PortalRotationDifference;
            Vector3 portalToObject = collision.gameObject.transform.position - _thisPortal.Position;
            Vector3 otherPortalToMirror = portalRotationDifference * _thisPortal.Script.Flip180 * portalToObject;
            
            mirror.SetMirrorPosition(otherPortalToMirror, _otherPortal);
            mirror.SetMirrorRotation(portalRotationDifference, collision.gameObject.transform.rotation, _otherPortal.Object);
            
            var sphereColliders = Physics.OverlapSphere(collision.transform.position, 0);
            if (sphereColliders.Contains(_portalCollider))
            {
                Teleport(portalRotationDifference, otherPortalToMirror, collision.gameObject);
            }
        }
        
        public void Teleport(Quaternion portalRotationDifference, Vector3 otherPortalToMirror, GameObject objectToTeleport)
        {
            if (objectToTeleport == _player.Object)
            {
                _player.CameraScript.AddXRotation(180f + portalRotationDifference.eulerAngles.y);
                _player.MovementScript.RotateMomentum(Quaternion.AngleAxis(180f, Vector3.up) * portalRotationDifference);
            }
            else
            {
                objectToTeleport.transform.rotation = Quaternion.AngleAxis(180f, _otherPortal.Object.transform.up) * portalRotationDifference * objectToTeleport.transform.rotation;
                objectToTeleport.GetComponent<Teleportable>().RotateMomentum(Quaternion.AngleAxis(180f, Vector3.up) * portalRotationDifference);
            }
            
            objectToTeleport.transform.position = _otherPortal.Position + otherPortalToMirror;
        }
    }
}
