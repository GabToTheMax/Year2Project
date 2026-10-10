using System.Collections.Generic;
using EzySlice;
using GabStuff.Scripts.Singletons;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.ARSubsystems;
using Plane = EzySlice.Plane;

namespace GabStuff.Scripts
{
    public class SpiritObject
    {
        private readonly GameObject _originalGameObject;
        public readonly GameObject SpiritGameObject;
        private readonly Mesh _collisionMesh;
        private readonly Portal _currentPortal;
        private readonly MeshCollider _spiritCollider;
        
        public SpiritObject(GameObject g, Portal p)
        {
            _originalGameObject = g;
            _collisionMesh = g.GetComponent<MeshCollider>().sharedMesh;
            
            SpiritGameObject = new GameObject($"{_originalGameObject.name} spirit object");
            GameObject spiritObjectParent = GameManager.Instance.SpiritObjectParent;
            SpiritGameObject.transform.SetParent(spiritObjectParent.transform);
            _spiritCollider = SpiritGameObject.AddComponent<MeshCollider>();
            _spiritCollider.sharedMesh = _collisionMesh;
            _spiritCollider.convex = true;
            
            _currentPortal = p;
            
            SpiritGameObject.layer = GameManager.Instance.GetGameSettings().SpiritRealmLayer;
        }

        /// <param name="thisPortalToObject">A vector from the portal, to the object which the SpiritObject is based off.</param>
        public void TransformSpiritObject(Vector3 localObjectPosition, Quaternion objectRotation, Portal targetPortal)
        {
            SpiritGameObject.transform.position = targetPortal.Position + localObjectPosition;
            SpiritGameObject.transform.rotation = objectRotation;
        }

        private static Mesh ScaleMesh(Mesh mesh, Vector3 scale)
        {
            if (mesh.vertices.Length == 0) return mesh;
            Vector3[] vertices = mesh.vertices;

            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i] = Vector3.Scale(mesh.vertices[i], scale);
            }

            Mesh newMesh = Object.Instantiate(mesh);
            newMesh.vertices = vertices;
            newMesh.RecalculateBounds();
            
            return newMesh;
        }
        
        public void SliceMeshAtPortal(Portal p)
        {
            Mesh scaledMesh = ScaleMesh(_collisionMesh, _originalGameObject.transform.localScale); 
            
            Vector3 normal = Quaternion.Inverse(_originalGameObject.transform.rotation) * p.Object.transform.forward;
            
            Vector3 objectToPortalVector = (p.Object.transform.position - _originalGameObject.transform.position);
            Vector3 rotatedObjectToPortalVector = Quaternion.Inverse(_originalGameObject.transform.rotation) * objectToPortalVector;
            
            Plane portalPlane = new(rotatedObjectToPortalVector, normal);
            var slicedHull = Slicer.Slice(scaledMesh, portalPlane, new TextureRegion(), 0);
            if (slicedHull == null) return;
            var slicedMesh = slicedHull.lowerHull;
            _spiritCollider.sharedMesh = slicedMesh;
        }
    }
}
