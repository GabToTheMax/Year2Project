using UnityEngine;

namespace GabStuff.Scripts
{
    public class PortalScript : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        public int portalIndex;
        public Vector3 vectorToPlayer;
        private Camera _cam;
    
        void Awake()
        {
            PortalManager.Instance.SetPortal(gameObject);
            _cam = GetComponentInChildren<Camera>();
        }

        void Update()
        {
            // TODO CLEAN UP INTO FUNCTIONS
            
            vectorToPlayer = player.transform.position - transform.position;
            vectorToPlayer.y = -vectorToPlayer.z;
            Vector3 otherVectorToPlayer = PortalManager.Instance.PortalScripts[1-portalIndex].vectorToPlayer;
            _cam.transform.position = transform.position + otherVectorToPlayer;
            _cam.transform.rotation = player.transform.rotation;
            Debug.DrawLine(transform.position, transform.position - otherVectorToPlayer, Color.yellow);
        }
    }
}
