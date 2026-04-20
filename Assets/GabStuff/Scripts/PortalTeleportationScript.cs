using System.Linq;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace GabStuff.Scripts
{
    public class PortalTeleportationScript : MonoBehaviour
    {
        private Player _player;
        private Portal _thisPortal;
        private Portal _otherPortal;
        private Collider _portalCollider;
        private GameObject _playerMirror;
        [SerializeField] private float distanceToTeleport;
    
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
            
            Quaternion portalRotationDifference = _otherPortal.Object.transform.rotation * Quaternion.Inverse(gameObject.transform.rotation);
                
            var sphereColliders = Physics.OverlapSphere(collision.transform.position, 0f);
            print(sphereColliders.Length);
            
            Vector3 portalToPlayer = _player.Position - _thisPortal.Position;
            Vector3 otherPortalToPlayer = Quaternion.AngleAxis(180f+portalRotationDifference.eulerAngles.y, _thisPortal.Object.transform.up) * portalToPlayer;

            _playerMirror.transform.position = _otherPortal.Position + otherPortalToPlayer;
            print(portalRotationDifference.eulerAngles.z);
            
            if (sphereColliders.Contains(_portalCollider))
            {
                print("Player in portal");
                _player.MovementScript.RotateMomentum(Quaternion.AngleAxis(180f, Vector3.up) * portalRotationDifference);
                
                _player.CameraScript.AddXRotation(180f + portalRotationDifference.eulerAngles.y);

                float verticalRotation = -portalRotationDifference.eulerAngles.z;
                if(verticalRotation > 180)
                    _player.CameraScript.AddYRotation(360-verticalRotation);
                else
                    _player.CameraScript.AddYRotation(verticalRotation);
                
                _player.Object.transform.position = _otherPortal.Position + otherPortalToPlayer;
            }
        }
    }
}
