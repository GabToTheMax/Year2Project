using System;
using System.Collections;
using UnityEngine;

namespace GabStuff.Scripts
{
    
    public class PortalScript : MonoBehaviour
    {
        #region Variables
        
        [SerializeField] private Material portalMaterial;
        public int index;
        private Quaternion _portalRotationDifference;
        public Quaternion PortalRotationDifference => _portalRotationDifference;
        private Vector3 _vectorToPlayerCamera;
        private Quaternion _flip180;
        public Quaternion Flip180 => _flip180;
        private Quaternion _portalCameraRotation;
        private Portal _thisPortal;
        private Portal _otherPortal;
        private Player _player;
        
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
            _flip180 = Quaternion.AngleAxis(180, _thisPortal.Object.transform.up);
            MoveCamera();
            RotateCamera();
            SetClippingPlane();
        }
        
        private void MoveCamera()
        {
            _vectorToPlayerCamera = _player.Camera.transform.position - transform.position;
            Debug.DrawLine(transform.position, transform.position + _vectorToPlayerCamera, Color.green);
            
            _vectorToPlayerCamera = _flip180 * _vectorToPlayerCamera;
            Vector3 otherPortalPos = _otherPortal.Position;
            
            // Quaternion black magic to account for rotated portals
            _vectorToPlayerCamera = _otherPortal.Object.transform.rotation * Quaternion.Inverse(gameObject.transform.rotation) * _vectorToPlayerCamera;
            
            _thisPortal.Camera.transform.position = otherPortalPos + _vectorToPlayerCamera;
        }

        private void RotateCamera()
        {
            _portalRotationDifference = _otherPortal.Object.transform.rotation * Quaternion.Inverse(gameObject.transform.rotation);
            _portalCameraRotation = _portalRotationDifference * (_flip180 * _player.Camera.transform.rotation);
            _thisPortal.Camera.transform.rotation = _portalCameraRotation;
            
            #region debug lines
            //Debug.DrawLine(_thisPortal.Camera.transform.position, _thisPortal.Camera.transform.position + upVector, Color.red);
            //Debug.DrawLine(_thisPortal.Camera.transform.position, _thisPortal.Camera.transform.position + vectorToOtherPortal, Color.orange);
            Debug.DrawLine(_thisPortal.Camera.transform.position, _thisPortal.Camera.transform.position + _portalCameraRotation * Vector3.forward, Color.red);
            Debug.DrawLine(_thisPortal.Camera.transform.position, _thisPortal.Camera.transform.position + _portalCameraRotation * Vector3.up, Color.limeGreen);
            #endregion
        }

        private void SetClippingPlane()
        {
            var normalToPlane = _otherPortal.Object.transform.forward;
            var pointOnPlane = _otherPortal.Object.transform.position;
            Shader.SetGlobalVector($"_Portal{_thisPortal.Index+1}PlaneNormal", new Vector4(normalToPlane.x, normalToPlane.y, normalToPlane.z, 0));
            Shader.SetGlobalVector($"_Portal{_thisPortal.Index+1}PlanePoint", new Vector4(pointOnPlane.x, pointOnPlane.y, pointOnPlane.z, 0));
        }
    }
}
