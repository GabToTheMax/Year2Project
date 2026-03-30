using System;
using UnityEngine;

namespace GabStuff.Scripts
{
    
    public class PortalScript : MonoBehaviour
    {
        #region Variables
        
        // TODO FIX PERMISSIONS
        [SerializeField] private GameObject player;
        [SerializeField] private int debugRotationValue;
        [SerializeField] private Material portalMaterial;
        public int index;
        private Portal _thisPortal;
        private Portal _otherPortal;
        private Camera _playerCamera;
        private Quaternion _portalCameraRotation;
        public Quaternion portalRotationDifference;
        public Vector3[] vertices;
        public Vector3 vectorToPlayerCamera;
        public Quaternion Flip180 { get; private set; }

        #endregion
    
        private void Awake()
        {
            _thisPortal = new Portal(gameObject, portalMaterial);
            PortalManager.Instance.SetPortal(_thisPortal);
            
            _playerCamera = player.GetComponentInChildren<Camera>();
        }

        private void Start()
        {
            _otherPortal = PortalManager.Instance.GetPortal(_thisPortal);
        }

        private void Update()
        {
            Flip180 = Quaternion.AngleAxis(180, _thisPortal.Object.transform.up);
            MoveCamera();
            RotateCamera();
            ZoomInCamera();
        }
        
        private void MoveCamera()
        {
            vectorToPlayerCamera = _playerCamera.transform.position - transform.position;
            Debug.DrawLine(transform.position, transform.position + vectorToPlayerCamera, Color.green);
            
            vectorToPlayerCamera = Flip180 * vectorToPlayerCamera;
            Vector3 otherPortalPos = _otherPortal.Object.transform.position;
            
            // Quaternion black magic to account for rotated portals
            vectorToPlayerCamera = _otherPortal.Object.transform.rotation * Quaternion.Inverse(gameObject.transform.rotation) * vectorToPlayerCamera;
            
            _thisPortal.Camera.transform.position = otherPortalPos + vectorToPlayerCamera;
        }

        private void RotateCamera()
        {
            portalRotationDifference = _otherPortal.Object.transform.rotation * Quaternion.Inverse(gameObject.transform.rotation);
            _portalCameraRotation = portalRotationDifference * (Flip180 * _playerCamera.transform.rotation);
            _thisPortal.Camera.transform.rotation = _portalCameraRotation;
            
            #region old rotation system
            Vector3 vectorToOtherPortal = _otherPortal.Object.transform.position - _thisPortal.Camera.transform.position;
            Vector3 upVector = Flip180 * portalRotationDifference * player.transform.up;
            #endregion
            #region debug lines
            //Debug.DrawLine(_thisPortal.Camera.transform.position, _thisPortal.Camera.transform.position + upVector, Color.red);
            //Debug.DrawLine(_thisPortal.Camera.transform.position, _thisPortal.Camera.transform.position + vectorToOtherPortal, Color.orange);
            Debug.DrawLine(_thisPortal.Camera.transform.position, _thisPortal.Camera.transform.position + _portalCameraRotation * Vector3.forward, Color.red);
            Debug.DrawLine(_thisPortal.Camera.transform.position, _thisPortal.Camera.transform.position + _portalCameraRotation * Vector3.up, Color.limeGreen);
            #endregion
            
        }

        private void ZoomInCamera()
        {
            vertices = _otherPortal.Mesh.vertices;
            Vector2[] portalPositionOnCamera = new Vector2[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                // Mesh.vertices gets the position of the vertices in the mesh file, not independent of the game object,
                // therefore they will be at the world origin. The following set of multiplications moves the vertices
                // to align with the game object
                
                vertices[i] = _otherPortal.Script.Flip180 * _otherPortal.Object.transform.rotation * vertices[i] * _otherPortal.Object.transform.localScale.x;
                vertices[i] += _otherPortal.Object.transform.transform.position;
                
                Debug.DrawLine(Vector3.zero, vertices[i]);
                
                    portalPositionOnCamera[i] = _thisPortal.Camera.WorldToScreenPoint(vertices[i]);
                        
                portalPositionOnCamera[i] /= new Vector2(1600, 900);
            }
            _thisPortal.Mesh.SetUVs(0, portalPositionOnCamera);
        }
    }
}
