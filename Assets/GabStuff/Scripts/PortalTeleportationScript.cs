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
            Vector3 otherPortalToPlayer = portalRotationDifference*_thisPortal.Script.Flip180 * portalToPlayer;
            float distanceToTeleport = 0f;
                
            Debug.DrawLine(_thisPortal.Position, _thisPortal.Position+portalToPlayer, Color.red);
            Debug.DrawLine(_otherPortal.Position, _otherPortal.Position+otherPortalToPlayer, Color.red);
            
            SetMirrorPosition(otherPortalToPlayer);
            
            var sphereColliders = Physics.OverlapSphere(collision.transform.position, distanceToTeleport);
            if (sphereColliders.Contains(_portalCollider))
            {
                TeleportPlayer(portalRotationDifference, otherPortalToPlayer, distanceToTeleport);
            }
        }

        private void SetMirrorPosition(Vector3 otherPortalToPlayer)
        {
            _playerMirror.transform.position = _otherPortal.Position + otherPortalToPlayer;
        }
        
        private void TeleportPlayer(Quaternion portalRotationDifference, Vector3 otherPortalToPlayer, float distanceToTeleport)
        {
            print("Player in portal");
            _player.MovementScript.RotateMomentum(Quaternion.AngleAxis(180f, Vector3.up) * portalRotationDifference);
            _player.CameraScript.AddXRotation(180f + portalRotationDifference.eulerAngles.y);

            float verticalRotation = -portalRotationDifference.eulerAngles.z;
            _player.Object.transform.position = _otherPortal.Position + otherPortalToPlayer + -_otherPortal.Object.transform.forward * 2 * distanceToTeleport;
        }
    }
}
