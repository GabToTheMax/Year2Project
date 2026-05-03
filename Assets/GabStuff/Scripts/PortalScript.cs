using System;
using UnityEngine;

namespace GabStuff.Scripts
{
    
    public class PortalScript : MonoBehaviour
    {
        #region Variables
        
        // TODO FIX PERMISSIONS
        [SerializeField] private Material portalMaterial;
        public int index;
        public Quaternion portalRotationDifference;
        private Quaternion _portalCameraRotation;
        public Vector3 vectorToPlayerCamera;
        private Portal _thisPortal;
        private Portal _otherPortal;
        private Player _player;
        public Quaternion flip180;
        
        public Portal ThisPortal => _thisPortal;
        
        #endregion
    
        private void Awake()
        {
            _thisPortal = new Portal(gameObject, portalMaterial);
            PortalManager.Instance.SetPortal(_thisPortal);
        }

        private void Start()
        {
            _player = PlayerManager.Instance.GetPlayer();
            _otherPortal = PortalManager.Instance.GetPortal(_thisPortal);
        }

        private void Update()
        {
            flip180 = Quaternion.AngleAxis(180, _thisPortal.Object.transform.up);
            MoveCamera();
            RotateCamera();
        }
        
        private void MoveCamera()
        {
            vectorToPlayerCamera = _player.Camera.transform.position - transform.position;
            Debug.DrawLine(transform.position, transform.position + vectorToPlayerCamera, Color.green);
            
            vectorToPlayerCamera = flip180 * vectorToPlayerCamera;
            Vector3 otherPortalPos = _otherPortal.Position;
            
            // Quaternion black magic to account for rotated portals
            vectorToPlayerCamera = _otherPortal.Object.transform.rotation * Quaternion.Inverse(gameObject.transform.rotation) * vectorToPlayerCamera;
            
            _thisPortal.Camera.transform.position = otherPortalPos + vectorToPlayerCamera;
        }

        private void RotateCamera()
        {
            portalRotationDifference = _otherPortal.Object.transform.rotation * Quaternion.Inverse(gameObject.transform.rotation);
            _portalCameraRotation = portalRotationDifference * (flip180 * _player.Camera.transform.rotation);
            _thisPortal.Camera.transform.rotation = _portalCameraRotation;
            
            #region debug lines
            //Debug.DrawLine(_thisPortal.Camera.transform.position, _thisPortal.Camera.transform.position + upVector, Color.red);
            //Debug.DrawLine(_thisPortal.Camera.transform.position, _thisPortal.Camera.transform.position + vectorToOtherPortal, Color.orange);
            Debug.DrawLine(_thisPortal.Camera.transform.position, _thisPortal.Camera.transform.position + _portalCameraRotation * Vector3.forward, Color.red);
            Debug.DrawLine(_thisPortal.Camera.transform.position, _thisPortal.Camera.transform.position + _portalCameraRotation * Vector3.up, Color.limeGreen);
            #endregion
        }
    }
}
