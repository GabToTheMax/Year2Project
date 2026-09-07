using UnityEngine;

namespace GabStuff.Scripts
{
    public class GrabbedObject
    {
        public readonly GameObject Object;
        public readonly PlayerGrabbable Script;
        public readonly Rigidbody Rigidbody;
        public Vector3 Position => Object.transform.position;
        
        public GrabbedObject(GameObject g, PlayerGrabbable s, Rigidbody r)
        {
            Object = g;
            Script = s;
            Rigidbody = r;
        }

    }
}