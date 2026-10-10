using UnityEngine;

namespace GabStuff.Scripts
{
    public class GrabbedObject
    {
        public readonly GameObject Object;
        public readonly PlayerGrabbable Script;
        public readonly Rigidbody Rigidbody;
        public readonly Collider Collider;
        public Vector3 Position => Object.transform.position;
        
        public GrabbedObject(GameObject g)
        {
            Object = g;
            Script = g.GetComponent<PlayerGrabbable>();
            Rigidbody = g.GetComponent<Rigidbody>();
            Collider = g.GetComponent<Collider>();
        }

    }
}