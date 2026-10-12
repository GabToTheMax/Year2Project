using GabStuff.Scripts.ScriptableObjects;
using GabStuff.Scripts.Singletons;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GabStuff.Scripts
{
    public class MirrorObject
    {
        public readonly GameObject OriginalGameObject;
        public readonly GameObject MirrorGameObject;
        public bool IsTeleporting;
        public Material OriginalMaterial;
        
        public MirrorObject(GameObject originalObject, Material material)
        {
            OriginalGameObject = originalObject;
            GameObject mirrorObjectParent = PortalManager.Instance.MirrorObjectParent;
            
            MirrorGameObject = new($"{originalObject.name} mirror object");
            MirrorGameObject.tag = GameManager.Instance.GetGameSettings().MirrorTag;
            MirrorGameObject.transform.parent = mirrorObjectParent.transform;
            MirrorGameObject.AddComponent<MeshFilter>().mesh = originalObject.GetComponent<MeshFilter>().mesh;;
            MirrorGameObject.AddComponent<MeshRenderer>().material = material;
            MirrorGameObject.AddComponent<Rigidbody>();

            var collisionMesh = originalObject.GetComponent<MeshCollider>().sharedMesh;
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

        public void SetMirrorScale(Vector3 scale)
        {
            MirrorGameObject.transform.localScale = scale;
        }

        public void Cleanup()
        {
            Object.Destroy(MirrorGameObject);
            OriginalGameObject.GetComponent<MeshRenderer>().material = OriginalMaterial;
        }
    }
}