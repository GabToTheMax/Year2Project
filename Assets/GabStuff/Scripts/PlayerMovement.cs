using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveRate;
    private Rigidbody _rb;
    private Vector3 _moveDirection;
    private Vector3 _smoothMove;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveDirection = context.ReadValue<Vector2>();
        _moveDirection.z = _moveDirection.y;
        _moveDirection.y = 0;
        print(_moveDirection);
    }
    
    private void FixedUpdate()
    {
        _smoothMove = Vector3.Lerp(_smoothMove, _moveDirection, 0.1f);
        _rb.AddForce(_smoothMove * (moveRate * Time.fixedDeltaTime), ForceMode.VelocityChange);
    }
}
