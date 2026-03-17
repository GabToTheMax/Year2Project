using System;
using System.Collections.Generic;
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
        private Portal _thisPortal;
        private Portal _otherPortal;
        private Camera _playerCamera;
        public Quaternion portalRotationDifference;
        public Vector3[] vertices;
        
        #endregion
    
        private void Awake()
        {
            _thisPortal = new Portal(gameObject, portalMaterial);
            PortalManager.Instance.SetPortal(_thisPortal);
            
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
            ZoomInCamera();
        }
        
        private void MoveCamera()
        {
            portalRotationDifference = _otherPortal.Object.transform.rotation * Quaternion.Inverse(gameObject.transform.rotation);
            
            vectorToPlayerCamera = _playerCamera.transform.position - transform.position;
            Debug.DrawLine(transform.position, transform.position + vectorToPlayerCamera, Color.green);
            
            vectorToPlayerCamera = Quaternion.AngleAxis(180, _thisPortal.Object.transform.up) * vectorToPlayerCamera;
            Vector3 otherPortalPos = _otherPortal.Object.transform.position;
            
            // Debug.DrawLine(otherPortalPos, otherPortalPos + _portalRotationDifference*vectorToPlayerCamera, Color.orange);
            
            // Quaternion black magic to account for rotated portals
            vectorToPlayerCamera = Quaternion.Inverse(gameObject.transform.rotation) * vectorToPlayerCamera;
            
            _thisPortal.Camera.transform.position = otherPortalPos + _otherPortal.Object.transform.rotation * vectorToPlayerCamera;
        }

        private void RotateCamera()
        {
            Vector3 vectorToOtherPortal = _otherPortal.Object.transform.position - _thisPortal.Camera.transform.position;
            Vector3 upVector = Quaternion.AngleAxis(180, _otherPortal.Object.transform.up) * portalRotationDifference * player.transform.up;

            #region debug lines
            Debug.DrawLine(_thisPortal.Camera.transform.position, _thisPortal.Camera.transform.position + upVector, Color.red);
            Debug.DrawLine(_thisPortal.Camera.transform.position, _thisPortal.Camera.transform.position + vectorToOtherPortal, Color.orange);
            #endregion
            
            _thisPortal.Camera.transform.rotation = Quaternion.LookRotation(vectorToOtherPortal, upVector);
        }

        private void ZoomInCamera()
        {
            vertices = _thisPortal.Mesh.vertices;
            Vector2[] portalPositionOnCamera = new Vector2[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = Quaternion.AngleAxis(180, _thisPortal.Object.transform.up) * _thisPortal.Object.transform.rotation * vertices[i] * transform.localScale.x;
                Debug.DrawLine(_thisPortal.Camera.transform.position,  _otherPortal.Object.transform.transform.position + _otherPortal.Script.vertices[i], Color.red);
                
                portalPositionOnCamera[i] = _thisPortal.Camera.WorldToScreenPoint(_otherPortal.Object.transform.transform.position + _otherPortal.Script.vertices[i]);
                portalPositionOnCamera[i] /= 1024;
                print($"point {i}, position {portalPositionOnCamera[i]}, portal {_thisPortal.Index}");
            }
            _thisPortal.Mesh.SetUVs(0, portalPositionOnCamera);
        }
    }
}
