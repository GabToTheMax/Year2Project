using UnityEngine;

namespace GabStuff.Scripts
{
    public class Voodoodoll : MonoBehaviour
    {
        [SerializeField] private float proportion;
        [SerializeField] private float derivative;
        [SerializeField] private Vector3 targetOffset;
        [SerializeField] private GameObject twin;
        private Rigidbody _rb;
        private Rigidbody _twinRb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _twinRb = twin.GetComponent<Rigidbody>();
        }
        
        void Update()
        {
            Vector3 targetPosition = transform.position;
            targetPosition += targetOffset;
            
            Vector3 proportionForce = (targetPosition - twin.transform.position) * proportion;
            Vector3 derivativeForce = Vector3.Dot(_twinRb.linearVelocity, Vector3.Normalize(targetPosition - twin.transform.position)) *
                                      -Vector3.Normalize(targetPosition - twin.transform.position) * derivative;
            
            Debug.DrawLine(twin.transform.position, twin.transform.position + proportionForce, Color.red);
            Debug.DrawLine(twin.transform.position, twin.transform.position + derivativeForce, Color.blue);
            
            _twinRb.AddForce(proportionForce + derivativeForce, ForceMode.Acceleration);
            
            twin.transform.rotation = transform.rotation;
        }
    }
}
