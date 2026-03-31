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
        private void OnTriggerStay(Collider collision)
        {
            if (!collision.CompareTag($"Player")) return;
                
            var sphereColliders = Physics.OverlapSphere(collision.transform.position, 0f);
            print(sphereColliders.Length);
            
            if (sphereColliders.Contains(_portalCollider))
            {
                print("Player in portal");
                _player.MovementScript.InvertMomentum();
                _player.CameraScript.AddXRotation(180f);
                
                Vector3 portalToPlayer = _player.Position - _thisPortal.Position;
                Vector3 otherPortalToPlayer = Quaternion.AngleAxis(180f, _thisPortal.Object.transform.up) * portalToPlayer;
                _player.Object.transform.position = _otherPortal.Position + otherPortalToPlayer;
            }
                
                
            // _player.MovementScript.Halt();
            // _player.CameraScript.AddXRotation(180f);
            // Vector3 portalToPlayer = _player.Object.transform.position - _thisPortal.Object.transform.position;
            // Vector3 otherPortalToPlayer = Quaternion.AngleAxis(180f, _thisPortal.Object.transform.up) * portalToPlayer;
        }
    }
}
