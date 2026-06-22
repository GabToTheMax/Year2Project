using System.Linq;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace GabStuff.Scripts
{
    public class PortalTeleportationScript : MonoBehaviour
    {
        //TODO Figure out why the player copy looks wierd
        
        #region variables
        private Player _player;
        private Portal _thisPortal;
        private Portal _otherPortal;
        private Collider _portalCollider;
        private GameObject _playerMirror;
        #endregion
        
        private void Start()
        {
            _thisPortal = GetComponent<PortalScript>().ThisPortal;
            _otherPortal = PortalManager.Instance.GetPortal(_thisPortal);
            _player = PlayerManager.Instance.GetPlayer(); 
            _portalCollider = _thisPortal.Object.GetComponent<Collider>();
        }
        
        /*
         *  I need to check if the player's center is across the portal, only then teleport them, to stop a teleport
         *  loop. So, for each frame where there is something in the collision, check if one of the colliders is the player. If so, halt and teleport.
         */

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playerMirror = new("PlayerMirror");
                _playerMirror.AddComponent<Rigidbody>();
                _playerMirror.AddComponent<MeshFilter>();
                _playerMirror.AddComponent<MeshRenderer>();
                _playerMirror.GetComponent<MeshFilter>().mesh = _player.Mesh;
                _playerMirror.GetComponent<MeshRenderer>().material = _player.Material;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Destroy(_playerMirror);
            }
        }
        
        private void OnTriggerStay(Collider collision)
        {
            if (!collision.CompareTag($"Player")) return;
            
            Quaternion portalRotationDifference = _thisPortal.Script.PortalRotationDifference;
            Vector3 portalToPlayer = _player.Position - _thisPortal.Position;
            Vector3 otherPortalToPlayerMirror = portalRotationDifference*_thisPortal.Script.Flip180 * portalToPlayer;
            
            SetMirrorPosition(otherPortalToPlayerMirror);
            
            var sphereColliders = Physics.OverlapSphere(collision.transform.position, 0);
            if (sphereColliders.Contains(_portalCollider))
            {
                TeleportPlayer(portalRotationDifference, otherPortalToPlayerMirror);
            }
        }

        private void SetMirrorPosition(Vector3 otherPortalToPlayer) 
        {
            _playerMirror.transform.position = _otherPortal.Position + otherPortalToPlayer;
            _playerMirror.transform.rotation = _player.Object.transform.rotation * Quaternion.Euler(0f, 180f, 0f);
        }
        
        private void TeleportPlayer(Quaternion portalRotationDifference, Vector3 otherPortalToPlayer)
        {
            //print("Player in portal");
            _player.MovementScript.RotateMomentum(Quaternion.AngleAxis(180f, Vector3.up) * portalRotationDifference);
            _player.CameraScript.AddXRotation(180f + portalRotationDifference.eulerAngles.y);

            float verticalRotation = -portalRotationDifference.eulerAngles.z;
            _player.Object.transform.position = _otherPortal.Position + otherPortalToPlayer;
        }
    }
}
