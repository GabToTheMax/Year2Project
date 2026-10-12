using UnityEngine;

namespace GabStuff.Scripts
{
    public class GrabbedObject
    {
        public readonly GameObject GameObject;
        public readonly PlayerGrabbable Script;
        public readonly Rigidbody Rigidbody;
        public readonly Collider[] Colliders;
        public Vector3 Position => GameObject.transform.position;
        
        public GrabbedObject(GameObject g)
        {
            GameObject = g;
            Script = g.GetComponent<PlayerGrabbable>();
            Rigidbody = g.GetComponent<Rigidbody>();
            Colliders = g.GetComponents<Collider>();
        }

    }
}