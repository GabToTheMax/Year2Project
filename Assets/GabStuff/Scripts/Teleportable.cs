using UnityEngine;

namespace GabStuff.Scripts
{
    public class Teleportable : MonoBehaviour
    {
        private Rigidbody _rb;
        
        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }
    
        /// <summary>
        /// Inverts the things momentum
        /// </summary>
        public void RotateMomentum(Quaternion rotation)
        {
            _rb.linearVelocity = rotation * _rb.linearVelocity;
            _rb.angularVelocity = rotation * _rb.angularVelocity;
        }
    }
}
