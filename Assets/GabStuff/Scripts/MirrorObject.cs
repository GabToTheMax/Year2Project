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
        
        public MirrorObject(Mesh mesh, Material material, Type colliderType, Collider collider)
        {
            MirrorGameObject = new("PlayerMirror");
            MirrorGameObject.AddComponent<MeshFilter>().mesh = mesh;
            MirrorGameObject.AddComponent<MeshRenderer>().material = material;
            MirrorGameObject.AddComponent<Rigidbody>();
            var mirrorCollider1 = MirrorGameObject.AddComponent(colliderType) as Collider;

            if (mirrorCollider1 == null) return;
            if (mirrorCollider1.GetType() == typeof(BoxCollider))
            {
                BoxCollider mirrorCollider = (mirrorCollider1 as BoxCollider)!;
                BoxCollider objectCollider = (collider as BoxCollider)!; 
                mirrorCollider.center = objectCollider.center; 
                mirrorCollider.size =  objectCollider.size;
            }
            else if (mirrorCollider1.GetType() == typeof(SphereCollider))
            {
                SphereCollider mirrorCollider = (mirrorCollider1 as SphereCollider)!;
                SphereCollider objectCollider = (collider as SphereCollider)!; 
                mirrorCollider.center = objectCollider.center; 
                mirrorCollider.radius =  objectCollider.radius;
            }
            else if (mirrorCollider1.GetType() == typeof(CapsuleCollider))
            {
                CapsuleCollider mirrorCollider = (mirrorCollider1 as CapsuleCollider)!;
                CapsuleCollider objectCollider = (collider as CapsuleCollider)!; 
                mirrorCollider.center = objectCollider.center;
                mirrorCollider.radius =  objectCollider.radius;
                mirrorCollider.direction = objectCollider.direction;
                mirrorCollider.height = objectCollider.height;
            }
        }
        
        public void SetMirrorPosition(Vector3 otherPortalToMirror, Portal otherPortal) 
        {
            MirrorGameObject.transform.position = otherPortal.Position + otherPortalToMirror;
        }

        public void SetMirrorRotation(Quaternion otherPortalRotationDifference, Quaternion mirroredRotation, GameObject otherPortal)
        {
            MirrorGameObject.transform.rotation = Quaternion.AngleAxis(180f, otherPortal.transform.up) * otherPortalRotationDifference * mirroredRotation;
        }
    }
}