using System;
using UnityEngine;

namespace GabStuff.Scripts
{
    public class PortalScript : MonoBehaviour
    {
        #region Variables
        
        [SerializeField] private GameObject player;
        public int index;
        public Vector3 vectorToPlayerCamera;
        private FPSCamera _fpsCam;
        private Portal _thisPortal;
        private Portal _otherPortal;
        private Camera _playerCamera;
        
        #endregion
    
        void Awake()
        {
            _thisPortal = new Portal(gameObject);
            PortalManager.Instance.SetPortal(_thisPortal);
            
            _fpsCam = player.GetComponent<FPSCamera>();
            _playerCamera = player.GetComponentInChildren<Camera>();
        }

        private void Start()
        {
            _otherPortal = PortalManager.Instance.Portals[1 - index];
        }

        void Update()
        {
            MoveCamera();
            RotateCamera();
        }
        
        private void MoveCamera()
        {
            vectorToPlayerCamera = _playerCamera.transform.position - transform.position;
            // Quaternion black magic to account for rotated portals
            // vectorToPlayerCamera = transform.rotation * Quaternion.Inverse(_otherPortal.Object.transform.rotation) * vectorToPlayerCamera;
            vectorToPlayerCamera = Quaternion.Inverse(_otherPortal.Object.transform.rotation) * transform.rotation * vectorToPlayerCamera;
            
            Vector3 otherPortalPos = _otherPortal.Object.transform.position;
            _thisPortal.Camera.transform.position = otherPortalPos - vectorToPlayerCamera;
            
            Debug.DrawLine(transform.position, transform.position + vectorToPlayerCamera, Color.green);
            //Debug.DrawLine(transform.position, transform.position + transform.rotation * Quaternion.Inverse(_otherPortal.Object.transform.rotation) * vectorToPlayerCamera, Color.orange);
            Debug.DrawLine(otherPortalPos, otherPortalPos + -vectorToPlayerCamera, Color.orange);
        }
        
        private void RotateCamera()
        {
            Vector3 vectorToOtherPortalFromCamera = _thisPortal.Camera.transform.position - _otherPortal.Object.transform.position;
            _thisPortal.Camera.transform.rotation = Quaternion.LookRotation(-vectorToOtherPortalFromCamera, Vector3.up);

        }


        /*
         *  To make the camera work
         *
         *  vectorToPlayer = current portal to the player
         *  camera position = other portal position - vectorToPlayer
         *
         *  Make camera look at the other portal.
         *
         *
         */
    }
}
