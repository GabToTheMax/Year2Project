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
        [SerializeField] private float distanceToTeleport;
    
        private void Start()
        {
            _thisPortal = GetComponent<PortalScript>().ThisPortal;
            _otherPortal = PortalManager.Instance.GetPortal(_thisPortal);
            _player = PlayerManager.Instance.GetPlayer(); 
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.CompareTag($"PlayerTeleportCollider"))
            {
                print("Player Entered portal");
                
                _player.MovementScript.Halt();
                _player.CameraScript.AddXRotation(180f);
                Vector3 portalToPlayer = _player.Object.transform.position - _thisPortal.Object.transform.position;
                Vector3 otherPortalToPlayer = Quaternion.AngleAxis(180f, _thisPortal.Object.transform.up) * portalToPlayer;
                _player.Object.transform.position = _otherPortal.Object.transform.position + otherPortalToPlayer + _otherPortal.Object.transform.forward * distanceToTeleport;
            }
        }
    }
}
