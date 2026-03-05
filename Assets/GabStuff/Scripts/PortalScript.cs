using System;
using Assets.GabStuff.Scripts;
using UnityEngine;

namespace GabStuff.Scripts
{
    public class PortalScript : MonoBehaviour
    {
        #region Variables
        
        [SerializeField] private GameObject player;
        [SerializeField] private int debugRotationValue;
        [SerializeField] private Material portalMaterial;
        public int index;
        public Vector3 vectorToPlayerCamera;
        private FPSCamera _fpsCam;
        private Portal _thisPortal;
        private Portal _otherPortal;
        private Camera _playerCamera;
        private Quaternion _portalRotationDifference;
        
        #endregion
    
        private void Awake()
        {
            _thisPortal = new Portal(gameObject, portalMaterial);
            PortalManager.Instance.SetPortal(_thisPortal);
            
            _fpsCam = player.GetComponent<FPSCamera>();
            _playerCamera = player.GetComponentInChildren<Camera>();
        }

        private void Start()
        {
            _otherPortal = PortalManager.Instance.Portals[1 - index];
        }

        private void Update()
        {
            MoveCamera();
            RotateCamera();
        }
        
        private void MoveCamera()
        {
            _portalRotationDifference = _otherPortal.Object.transform.rotation * Quaternion.Inverse(gameObject.transform.rotation);
            
            vectorToPlayerCamera = _playerCamera.transform.position - transform.position;
            Debug.DrawLine(transform.position, transform.position + vectorToPlayerCamera, Color.green);
            
            vectorToPlayerCamera = Quaternion.AngleAxis(180, _thisPortal.Object.transform.forward) * vectorToPlayerCamera;
            Vector3 otherPortalPos = _otherPortal.Object.transform.position;
            
            Debug.DrawLine(otherPortalPos, otherPortalPos + _portalRotationDifference*vectorToPlayerCamera, Color.orange);
            
            // Quaternion black magic to account for rotated portals
            vectorToPlayerCamera = Quaternion.Inverse(gameObject.transform.rotation) * vectorToPlayerCamera;
            
            _thisPortal.Camera.transform.position = otherPortalPos + _otherPortal.Object.transform.rotation * vectorToPlayerCamera;
        }
        
        private void RotateCamera()
        {
            Vector3 vectorToOtherPortalFromCamera = _thisPortal.Camera.transform.position - _otherPortal.Object.transform.position;
            _thisPortal.Camera.transform.rotation = Quaternion.LookRotation(-vectorToOtherPortalFromCamera, _portalRotationDifference*Vector3.up);
        }

        private void ZoomInCamera()
        {
            _thisPortal.Camera.WorldToScreenPoint(_otherPortal.Object.transform.position);
            Texture2D portalTexture = _thisPortal.PortalMaterial.mainTexture as Texture2D;
            if (portalTexture != null)
            {
                print("Texture is not null");
                portalTexture.SetPixel(0, 0, Color.red);
                _thisPortal.PortalMaterial.mainTexture = portalTexture;
            }
        }
    }
}
