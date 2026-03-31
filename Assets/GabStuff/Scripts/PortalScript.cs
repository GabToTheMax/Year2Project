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
        public Vector3[] vertices;
        public Vector3 vectorToPlayerCamera;
        private Portal _thisPortal;
        private Portal _otherPortal;
        private Quaternion _portalCameraRotation;
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
            ZoomInCamera();
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
            
            #region old rotation system
            Vector3 vectorToOtherPortal = _otherPortal.Position - _thisPortal.Camera.transform.position;
            Vector3 upVector = flip180 * portalRotationDifference * _player.Object.transform.up;
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
                
                vertices[i] = _otherPortal.Script.flip180 * _otherPortal.Object.transform.rotation * vertices[i] * _otherPortal.Object.transform.localScale.x;
                vertices[i] += _otherPortal.Position;
                
                //Debug.DrawLine(Vector3.zero, vertices[i]);
                
                    portalPositionOnCamera[i] = _thisPortal.Camera.WorldToScreenPoint(vertices[i]);
                        
                portalPositionOnCamera[i] /= new Vector2(1600, 900);
            }
            _thisPortal.Mesh.SetUVs(0, portalPositionOnCamera);
        }
    }
}
