using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallPlayer : MonoBehaviour, IBall
{
    [Header("Ball Settings")]
    [SerializeField] private float minPower = 2f;
    [SerializeField] public float maxPower = 15f;
    [SerializeField] private float chargeTime = 1.5f;
    
    
    private PlayerInput _playerInput;
    private Vector3 _velocity;
    private Vector2 _direction;

    private float _scrollAngle;
    private float _rotationSpeed = 10f;
    private float _friction = 3f;
    private float _stopThreshold = 0.05f;

    private float _currentPower;
    private float _chargeTimer;
    private bool _isCharging;
    
    public event Action<float> OnChargeChanged;

    public Transform Transform => transform;
    public Vector3 Velocity { get => _velocity; set => _velocity = value; }
    public float Radius => transform.lossyScale.x / 2f;
    private bool IsMoving => _velocity.sqrMagnitude > 0f;
    
    public float Charge01 => Mathf.Clamp01(_chargeTimer / chargeTime);
    

    
    void OnEnable()  => BallRegistry.All.Add(this);
    void OnDisable() => BallRegistry.All.Remove(this);
    
    void FixedUpdate()
    {
        ChooseDirection();
        PerformMove();
    }

    void Update()
    {
        UpdateCharge();
        Debug.DrawRay(transform.position, GetDirection() * 5, Color.blue);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started && !IsMoving)
        {
            OnChargeChanged?.Invoke(0f);
            _isCharging = true;
            _chargeTimer = 0f;
        }
        else if (context.canceled && _isCharging)
        {
            _velocity = GetDirection() * _currentPower;
            _isCharging = false;
            _chargeTimer = 0f;
            OnChargeChanged?.Invoke(0f);
        }
    }

    public void OnChangeDirection(InputAction.CallbackContext context)
    {
        if (context.performed)
            _scrollAngle += context.ReadValue<Vector2>().y * _rotationSpeed;
    }

    void PerformMove()
    {
        if (!IsMoving) return;

        float dt = Time.fixedDeltaTime;
        float rayon = Radius;
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

            foreach (IBall other in BallRegistry.All)
            {
                if (other == (IBall)this) continue;
                BallCollision.Resolve(this, other);
            }
        }

        _velocity = Vector3.MoveTowards(_velocity, Vector3.zero, _friction * dt);
        if (_velocity.magnitude < _stopThreshold) _velocity = Vector3.zero;
    }

    void ChooseDirection()
    {
        if (_playerInput == null)
        {
            Quaternion targetRotation = Quaternion.Euler(0f, _scrollAngle, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
    
    private Vector3 GetDirection()
    {
        float radiantAngles = _scrollAngle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radiantAngles), 0f, Mathf.Cos(radiantAngles));
    }

    void UpdateCharge()
    {
        if (!_isCharging) return;

        _chargeTimer = Mathf.Min(_chargeTimer + Time.deltaTime, chargeTime);
        _currentPower = Mathf.Lerp(minPower, maxPower, Charge01);
        OnChargeChanged?.Invoke(Charge01);
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