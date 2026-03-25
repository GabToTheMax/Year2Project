using System;
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
        private Quaternion _180Flip;
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
            _otherPortal = PortalManager.Instance.GetPortal(_thisPortal);
        }

        private void Update()
        {
            _180Flip = Quaternion.AngleAxis(180, _thisPortal.Object.transform.up);
            MoveCamera();
            RotateCamera();
            ZoomInCamera();
        }
        
        private void MoveCamera()
        {
            vectorToPlayerCamera = _playerCamera.transform.position - transform.position;
            Debug.DrawLine(transform.position, transform.position + vectorToPlayerCamera, Color.green);
            
            vectorToPlayerCamera = _180Flip * vectorToPlayerCamera;
            Vector3 otherPortalPos = _otherPortal.Object.transform.position;
            
            // Debug.DrawLine(otherPortalPos, otherPortalPos + _portalRotationDifference*vectorToPlayerCamera, Color.orange);
            
            // Quaternion black magic to account for rotated portals
            vectorToPlayerCamera = _otherPortal.Object.transform.rotation * Quaternion.Inverse(gameObject.transform.rotation) * vectorToPlayerCamera;
            
            _thisPortal.Camera.transform.position = otherPortalPos + vectorToPlayerCamera;
        }

        private void RotateCamera()
        {
            portalRotationDifference = _otherPortal.Object.transform.rotation * Quaternion.Inverse(gameObject.transform.rotation);

            Vector3 vectorToOtherPortal = _otherPortal.Object.transform.position - _thisPortal.Camera.transform.position;
            Vector3 upVector = _180Flip * portalRotationDifference * player.transform.up;

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
                // Mesh.vertices gets the position of the vertices in the mesh file, not independent of the game object,
                // therefore they will be at the world origin. The following set of multiplications moves the vertices
                // to align with the game object
                
                vertices[i] = _180Flip * _thisPortal.Object.transform.rotation * vertices[i] * transform.localScale.x;
                try
                {
                    portalPositionOnCamera[i] = _thisPortal.Camera.WorldToScreenPoint(
                        _otherPortal.Object.transform.transform.position + _otherPortal.Script.vertices[i]);
                }
                catch(IndexOutOfRangeException)
                {
                    print(i);
                }
                        
                portalPositionOnCamera[i] /= 1024;
            }
            _thisPortal.Mesh.SetUVs(0, portalPositionOnCamera);
        }
    }
}
