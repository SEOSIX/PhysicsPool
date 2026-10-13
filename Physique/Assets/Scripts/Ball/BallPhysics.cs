using UnityEngine;
using UnityEngine.InputSystem;

public class BallPhysics : MonoBehaviour
{
    private PlayerInput playerInput;
    
    private Vector3 _velocity;
    private Vector2 _direction;
    private Vector3 _currentPostion;

    private float gravity;
    private float _scrollAngle;
    private float _rotationSpeed = 10f;
    private float _friction = 3f;
    
    private bool _isMoving;
    
    private Vector3 Bounds()
    {
        Vector3 size = transform.localScale;
        Vector3 col = size / 2;
        return col;
    }

    private Vector3 GetDirection()
    {
        float radiantAngles = _scrollAngle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radiantAngles), 0f, Mathf.Cos(radiantAngles));
    }


    void FixedUpdate()
    {
        ChooseDirection();
        PerformMove();
        
    }

    void Update()
    {
        Debug.DrawRay(transform.position, _direction * 5, Color.blue);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _velocity = GetDirection() * 10f;
            _isMoving = true;
        }
    }

    public void OnChangeDirection(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _scrollAngle += context.ReadValue<Vector2>().y * _rotationSpeed;
        }
    }

    void PerformMove()
    {
        if (!_isMoving) return;
        
        float t = Time.fixedDeltaTime;
        
        transform.position += _velocity * t;
        _velocity = Vector3.MoveTowards(_velocity, Vector3.zero, _friction * t);
    }

    
    void ChooseDirection()
    {
        if (playerInput == null)
        {
            Quaternion targetRotation = Quaternion.Euler(0f, _scrollAngle, 0f);
            gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
    
    void DetectsBounds()
    {
        
    }
}
