using UnityEngine;

namespace GabStuff.Scripts
{
    public class PortalScript : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        public int portalIndex;
        public Vector3 vectorToPlayer;
        private FPSCamera _fpsCam;
        private Camera _cam;
    
        void Awake()
        {
            PortalManager.Instance.SetPortal(gameObject);
            _cam = GetComponentInChildren<Camera>();
            _fpsCam = player.GetComponent<FPSCamera>();
        }

        void Update()
        {
            SetVectorToPlayer();
            MoveCamera();
            RotateCamera();
        }

        private void SetVectorToPlayer()
        {
            vectorToPlayer = player.transform.position - transform.position;
            vectorToPlayer.y = -vectorToPlayer.y;
        }
        
        private void MoveCamera()
        {
            Vector3 otherVectorToPlayer = PortalManager.Instance.PortalScripts[1-portalIndex].vectorToPlayer;
            _cam.transform.position = transform.position - otherVectorToPlayer;
            Debug.DrawLine(transform.position, transform.position - otherVectorToPlayer, Color.yellow);
        }

        private void RotateCamera()
        {
            _cam.transform.rotation = Quaternion.AngleAxis(180+_fpsCam.playerDirection, Vector3.up);
        }
    }
}
