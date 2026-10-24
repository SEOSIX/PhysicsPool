using UnityEngine;
using UnityEngine.InputSystem;

public class BallPlayer : MonoBehaviour
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
    
    private const float Marge = 0.01f;
    
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

        float dt = Time.fixedDeltaTime;
        float rayon = transform.lossyScale.x / 2f;
        float distance = _velocity.magnitude * dt;
        int steps = Mathf.Max(1, Mathf.CeilToInt(distance / (rayon * 0.5f)));
        float stepDt = dt / steps;

        for (int i = 0; i < steps; i++)
        {
            transform.position += _velocity * stepDt;

            foreach (Wall wall in Wall.All)
            {
                if (DetectsBounds(wall.transform, transform.position, rayon,
                        out Vector3 normal, out float through))
                {
                    transform.position += normal * through;
                    if (Vector3.Dot(_velocity, normal) < 0f)
                        _velocity = Vector3.Reflect(_velocity, normal);
                }
            }
        }

        _velocity = Vector3.MoveTowards(_velocity, Vector3.zero, _friction * dt);
    }

    
    void ChooseDirection()
    {
        if (playerInput == null)
        {
            Quaternion targetRotation = Quaternion.Euler(0f, _scrollAngle, 0f);
            gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
    
    bool DetectsBounds(Transform wall, Vector3 center, float ray, out Vector3 normal, out float through)
    {
       Vector3 local = Quaternion.Inverse(wall.rotation) * (center - wall.position);
       Vector3 half = wall.lossyScale / 2f;
       
       Vector3 closestLocal = new Vector3(
           Mathf.Clamp(local.x, -half.x, half.x),
           Mathf.Clamp(local.y, -half.y, half.y),
           Mathf.Clamp(local.z, -half.z, half.z));
       
       Vector3 closest = wall.position + wall.rotation * closestLocal;
       
       Vector3 diff = center - closest;
       diff.y = 0f;
       float distance = diff.magnitude;
       if (distance < 0.0001f)
       {
           normal = center - wall.position;
           normal.y = 0f;
           normal = normal.normalized;
           through = ray;
           return true;
       }
       normal = diff / distance;
       through = ray - distance;
       return distance < ray;
    }
}
