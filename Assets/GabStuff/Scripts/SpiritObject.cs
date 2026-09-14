using EzySlice;
using GabStuff.Scripts.Singletons;
using UnityEngine;
using Plane = EzySlice.Plane;

namespace GabStuff.Scripts
{
    public class SpiritObject
    {
        public readonly GameObject OriginalGameObject;
        public readonly GameObject SpiritGameObject;
        private readonly Mesh _collisionMesh;
        private readonly Portal _currentPortal;
        private readonly MeshCollider _spiritCollider;
        
        public SpiritObject(GameObject g, Portal p)
        {
            OriginalGameObject = g;
            SpiritGameObject = new GameObject();
            _spiritCollider = SpiritGameObject.AddComponent<MeshCollider>();
            _collisionMesh = g.GetComponent<MeshCollider>().sharedMesh;
            _spiritCollider.sharedMesh = _collisionMesh;
            _spiritCollider.convex = true;
            _currentPortal = p;
            SpiritGameObject.layer = GameManager.Instance.GetGameSettings().SpiritRealmLayer;
            SpiritGameObject.name = $"{OriginalGameObject.name} spirit object";
        }

        /// <param name="thisPortalToObject">A vector from the portal, to the object which the SpiritObject is based off.</param>
        public void TransformSpiritObject(Vector3 localObjectPosition, Quaternion objectRotation, Portal targetPortal)
        {
            SpiritGameObject.transform.position = targetPortal.Position + localObjectPosition;
            SpiritGameObject.transform.rotation = objectRotation;
            SpiritGameObject.transform.localScale = OriginalGameObject.transform.localScale;
        }
        
        public void SliceMeshAtPortal(Portal p)
        {
            Vector3 scale = OriginalGameObject.transform.localScale;
            Vector3 normal = Quaternion.Inverse(OriginalGameObject.transform.rotation) * p.Object.transform.forward;
            Vector3 objectToPortalVector = (p.Object.transform.position - OriginalGameObject.transform.position);
            objectToPortalVector = Quaternion.Inverse(OriginalGameObject.transform.rotation) * objectToPortalVector;
            objectToPortalVector = Vector3.Scale(scale, objectToPortalVector);
            Plane portalPlane = new Plane(objectToPortalVector, normal);
            var slicedHull = Slicer.Slice(_collisionMesh, portalPlane, new TextureRegion(), 0);
            if (slicedHull == null) return;
            var slicedMesh = slicedHull.lowerHull;
            _spiritCollider.sharedMesh = slicedMesh;
        }
    }
}
