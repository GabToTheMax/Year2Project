using System.Linq;
using EzySlice;
using GabStuff.Scripts.ScriptableObjects;
using GabStuff.Scripts.Singletons;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Plane = UnityEngine.Plane;

namespace GabStuff.Scripts
{
    public class PortalledObject
    {
        public readonly GameObject OriginalGameObject;
        public readonly MeshCollider WorldSideCollider;
        public readonly MeshCollider SpiritSideCollider;
        public readonly Mesh OriginalCollisionMesh;
        public bool IsTeleporting;
        
        public PortalledObject(GameObject og)
        {
            IsTeleporting = false;
            OriginalGameObject = og;
            WorldSideCollider = og.GetComponent<MeshCollider>();
            OriginalCollisionMesh = WorldSideCollider.sharedMesh;
            SpiritSideCollider = og.AddComponent<MeshCollider>();
            SpiritSideCollider.sharedMesh = OriginalCollisionMesh;
            SpiritSideCollider.convex = true;

            GameSettings gs = GameManager.Instance.GetGameSettings();

            GameObject[] portalObjects = new GameObject[2];
            Portal[] portals = PortalManager.Instance.Portals;
            for (var i = 0; i < portals.Length; i++)
            {
                portalObjects[i] = portals[i].GameObject;
            }

            // TODO make it also check subchildren
            foreach (GameObject go in GameManager.Instance.GetEveryObject())
            {
                Collider c;
                if (go.TryGetComponent(out c) == false || portalObjects.Contains(go)) continue;
                
                if(go.tag == gs.SpiritTag)
                    Physics.IgnoreCollision(WorldSideCollider, c, true);
                else
                    Physics.IgnoreCollision(SpiritSideCollider, c, true);
            }
        }

        public void Slice(Portal p)
        {
            Mesh scaledMesh = PortalManager.ScaleMesh(OriginalCollisionMesh, OriginalGameObject.transform.localScale); 
            
            Vector3 normal = Quaternion.Inverse(OriginalGameObject.transform.rotation) * p.GameObject.transform.forward;
            
            Vector3 objectToPortalVector = (p.GameObject.transform.position - OriginalGameObject.transform.position);
            Vector3 rotatedObjectToPortalVector = Quaternion.Inverse(OriginalGameObject.transform.rotation) * objectToPortalVector;
            
            EzySlice.Plane portalPlane = new(rotatedObjectToPortalVector, normal);
            var slicedHull = Slicer.Slice(scaledMesh, portalPlane, new TextureRegion(), 0);
            if (slicedHull == null) return;
            WorldSideCollider.sharedMesh = slicedHull.lowerHull;
            SpiritSideCollider.sharedMesh = slicedHull.upperHull;
            
        }
        
        public void Cleanup()
        {
            Object.Destroy(SpiritSideCollider);
            WorldSideCollider.sharedMesh = OriginalCollisionMesh;
        }
    }
}
