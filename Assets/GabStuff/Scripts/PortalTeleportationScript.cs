using System;
using System.Collections.Generic;
using System.Linq;
using GabStuff.Scripts.Singletons;
using Unity.Mathematics;
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
        private GameSettings _gameSettings;
        #endregion
        
        private void Start()
        {
            _mirrors = PortalManager.Instance.GetMirrors();
            _thisPortal = GetComponent<PortalScript>().ThisPortal;
            _otherPortal = PortalManager.Instance.GetPortal(_thisPortal);
            _portalCollider = _thisPortal.Object.GetComponent<Collider>();
            _player = PlayerManager.Instance.GetPlayer();
            _gameSettings = GameManager.Instance.GetGameSettings();
        }
        
        /*
         *  I need to check if the player's center is across the portal, only then teleport them, to stop a teleport
         *  loop. So, for each frame where there is something in the collision, check if one of the colliders is the player. If so, teleport.
         */

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Teleportable>() && !_mirrors.ContainsKey(other.gameObject))
            {
                Material originalMaterialCopy = other.gameObject.GetComponent<MeshRenderer>().material;
                Material otherMaterial = null;
                print("Something entered my collision. Sincerely portal #" + _thisPortal.Index);
                
                switch (_thisPortal.Index)
                {
                    case 0:
                        otherMaterial = new Material(_gameSettings.BehindPortal2Shader);
                        other.gameObject.GetComponent<MeshRenderer>().material = new Material(_gameSettings.BehindPortal1Shader);
                        break;                
                    case 1:
                        otherMaterial = new Material(_gameSettings.BehindPortal1Shader);
                        other.gameObject.GetComponent<MeshRenderer>().material = new Material(_gameSettings.BehindPortal2Shader);
                        break;
                    default:
                        throw new Exception("Portal with incorrect index");
                }
                
                otherMaterial.CopyPropertiesFromMaterial(originalMaterialCopy);
                other.gameObject.GetComponent<MeshRenderer>().material.CopyPropertiesFromMaterial(originalMaterialCopy);
                    
                var otherMesh = other.gameObject.GetComponent<MeshFilter>().mesh;
                var component = other.GetComponent<Collider>();
                var colliderType = component.GetType();
                
                _mirrors.Add(other.gameObject, new MirrorObject(otherMesh, otherMaterial, colliderType, component));
                _mirrors[other.gameObject].OriginalMaterial = originalMaterialCopy;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_mirrors.Keys.Contains(other.gameObject) && !_mirrors[other.gameObject].IsTeleporting)
            {
                print("Something exited my collision. Sincerely portal #" + _thisPortal.Index);
                Destroy(_mirrors[other.gameObject].MirrorGameObject);
                Material originalMaterial = _mirrors[other.gameObject].OriginalMaterial;
                _mirrors.Remove(other.gameObject);
                other.gameObject.GetComponent<MeshRenderer>().material = originalMaterial;
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
            
            Material buffer = objectToTeleport.GetComponent<MeshRenderer>().material;
            objectToTeleport.GetComponent<MeshRenderer>().material =
                _mirrors[objectToTeleport].MirrorGameObject.GetComponent<MeshRenderer>().material;
            _mirrors[objectToTeleport].MirrorGameObject.GetComponent<MeshRenderer>().material = buffer;
            
        }
        
        
    }
}
