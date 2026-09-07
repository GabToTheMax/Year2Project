using UnityEngine;

namespace GabStuff.Scripts
{
    public class PlayerGrabbable : MonoBehaviour
    {
        [SerializeField] private float radius;
        public float GetRadius() => radius;
    }
}
