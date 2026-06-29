using System;
using UnityEngine;

namespace GabStuff.Scripts
{
    public class MirrorObject
    {
        public readonly GameObject MirrorGameObject;
        private readonly Collider _mirrorCollider;
        
        public MirrorObject(Mesh mesh, Material material, Type colliderType, Collider collider)
        {
            MirrorGameObject = new("PlayerMirror");
            MirrorGameObject.AddComponent<MeshFilter>().mesh = mesh;
            MirrorGameObject.AddComponent<MeshRenderer>().material = material;
            MirrorGameObject.AddComponent<Rigidbody>();
            _mirrorCollider = MirrorGameObject.AddComponent(colliderType) as Collider;

            if (_mirrorCollider == null) return;
            if (_mirrorCollider.GetType() == typeof(BoxCollider))
            {
                BoxCollider mirrorCollider = (_mirrorCollider as BoxCollider)!;
                BoxCollider objectCollider = (collider as BoxCollider)!; 
                mirrorCollider.center = objectCollider.center; 
                mirrorCollider.size =  objectCollider.size;
            }
            else if (_mirrorCollider.GetType() == typeof(SphereCollider))
            {
                SphereCollider mirrorCollider = (_mirrorCollider as SphereCollider)!;
                SphereCollider objectCollider = (collider as SphereCollider)!; 
                mirrorCollider.center = objectCollider.center; 
                mirrorCollider.radius =  objectCollider.radius;
            }
            else if (_mirrorCollider.GetType() == typeof(CapsuleCollider))
            {
                CapsuleCollider mirrorCollider = (_mirrorCollider as CapsuleCollider)!;
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