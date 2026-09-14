using System;
using GabStuff.Scripts.Singletons;
using UnityEngine;

namespace GabStuff.Scripts
{
    public class MirrorObject
    {
        public readonly GameObject MirrorGameObject;
        public bool IsTeleporting;
        public Material OriginalMaterial;
        
        public MirrorObject(Mesh mesh, Material material, Mesh collisionMesh)
        {
            MirrorGameObject = new("PlayerMirror");
            MirrorGameObject.AddComponent<MeshFilter>().mesh = mesh;
            MirrorGameObject.AddComponent<MeshRenderer>().material = material;
            MirrorGameObject.AddComponent<Rigidbody>();

            if (collisionMesh == null) return;
            
            var meshCollider = MirrorGameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = collisionMesh;
            meshCollider.convex = true;
        }
        
        public void SetMirrorPosition(Vector3 otherPortalToMirror, Portal otherPortal) 
        {
            MirrorGameObject.transform.position = otherPortal.Position + otherPortalToMirror;
        }

        public void SetMirrorRotation(Quaternion otherPortalRotationDifference, Quaternion objectRotation, GameObject otherPortal)
        {
            MirrorGameObject.transform.rotation = Quaternion.AngleAxis(180f, otherPortal.transform.up) * otherPortalRotationDifference * objectRotation;
        }
    }
}