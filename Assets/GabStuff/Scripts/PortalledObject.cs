using System;
using System.Linq;
using EzySlice;
using GabStuff.Scripts.ScriptableObjects;
using GabStuff.Scripts.Singletons;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using Plane = UnityEngine.Plane;

namespace GabStuff.Scripts
{
    public class PortalledObject
    {
        public readonly GameObject OriginalGameObject;
        public readonly MeshCollider OriginalCollider;
        public readonly MeshCollider WorldSideCollider;
        public readonly MeshCollider SpiritSideCollider;
        public readonly Mesh OriginalCollisionMesh;
        private readonly Player _player = PlayerManager.Instance.GetPlayer();
        public PortalledObject(GameObject og)
        {
            OriginalGameObject = og;
            OriginalCollider = og.GetComponent<MeshCollider>();
            OriginalCollisionMesh = OriginalCollider.sharedMesh;
            
            WorldSideCollider = og.AddComponent<MeshCollider>();
            SpiritSideCollider = og.AddComponent<MeshCollider>();
            WorldSideCollider.sharedMesh = OriginalCollisionMesh;
            SpiritSideCollider.sharedMesh = OriginalCollisionMesh;
            WorldSideCollider.convex = true;
            SpiritSideCollider.convex = true;

            GameSettings gs = GameManager.Instance.GetGameSettings();

            // TODO make it also check subchildren
            foreach (GameObject go in GameManager.Instance.GetEveryObject())
            {
                Collider[] cs = go.GetComponents<Collider>();

                foreach (Collider c in cs)
                {
                    /*
                     * Foreach GameObject go
                     *
                     * if this is the grabbedObject, and go is the player
                     *      Ignore all collisions between go and this
                     * 
                     *  if go is not a portal
                     *      OGC ignores it
                     *  if go is a portal
                     *      WSC and SSC ignore it
                     *
                     *  if go is a spirit object
                     *      WSC ignores it
                     *  if go is a normal world object
                     *      SSC ignores it
                     *
                     */
                
                    if (c == null) continue;

                    if (_player.PlayerGrabber.CurrentlyGrabbing &&
                        _player.PlayerGrabber.GrabbedObject.GameObject == OriginalGameObject &&
                        c.gameObject == _player.GameObject)
                    {
                        Physics.IgnoreCollision(OriginalCollider, c, true);
                        Physics.IgnoreCollision(WorldSideCollider, c, true);
                        Physics.IgnoreCollision(SpiritSideCollider, c, true);
                        continue;
                    }
                    
                    if (go.tag != GameManager.Instance.GetGameSettings().PortalTag)
                    {
                        Physics.IgnoreCollision(OriginalCollider, c, true);
                    }
                    else
                    {
                        Physics.IgnoreCollision(WorldSideCollider, c, true);
                        Physics.IgnoreCollision(SpiritSideCollider, c, true);
                    }
                
                    if(go.tag == gs.SpiritTag)
                        Physics.IgnoreCollision(WorldSideCollider, c, true);
                    else
                        Physics.IgnoreCollision(SpiritSideCollider, c, true);

                }
            }
        }

        private static Vector3 ReciprocalVector(Vector3 v)
        {
            return new Vector3
            {
                x = Mathf.Pow(v.x,-1),
                y = Mathf.Pow(v.y,-1),
                z = Mathf.Pow(v.z,-1),
            };
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

            Vector3 unscaleFactor = ReciprocalVector(OriginalGameObject.transform.localScale);
            
            WorldSideCollider.sharedMesh = PortalManager.ScaleMesh(slicedHull.lowerHull, unscaleFactor);
            SpiritSideCollider.sharedMesh = PortalManager.ScaleMesh(slicedHull.upperHull, unscaleFactor);

        }
        
        public void Cleanup()
        {
            Object.Destroy(SpiritSideCollider);
            Object.Destroy(WorldSideCollider);
            foreach (GameObject go in GameManager.Instance.GetEveryObject())
            {
                if(go == OriginalGameObject) continue;
                
                Collider[] cs = go.GetComponents<Collider>();

                foreach (Collider c in cs)
                {
                    if (c == null) continue;

                    if (_player.PlayerGrabber.CurrentlyGrabbing &&
                        _player.PlayerGrabber.GrabbedObject.GameObject == OriginalGameObject &&
                        c.gameObject == _player.GameObject)
                    {
                        Physics.IgnoreCollision(OriginalCollider, c, true);
                        continue;
                    }
                    
                    //Debug.Log($"PortalManager.Instance.Portalled.Values {PortalManager.Instance.Portalled.Values.Count}");
                    foreach (PortalledObject p in PortalManager.Instance.Portalled.Values)
                    {
                        if (p == this)
                        {
                            Physics.IgnoreCollision(OriginalCollider, c, false); 
                            continue;
                        }
                        
                        bool ignore = p.SpiritSideCollider == c || p.OriginalCollider == c;
                        
                        // Debug.Log($"p.SpiritSideCollider == c : {p.SpiritSideCollider == c}");
                        // Debug.Log($"p.OriginalCollider == c : {p.OriginalCollider == c}");
                        //
                        // Debug.Log($"Collider: {OriginalCollider} and Collider: {c}. Are they colliding: {!ignore}");
                        Physics.IgnoreCollision(OriginalCollider, c, ignore);
                    }
                }           
            }
        }
    }
}
