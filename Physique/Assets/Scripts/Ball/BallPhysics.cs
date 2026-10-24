using System.Collections.Generic;
using UnityEngine;

public class BallPhysics : MonoBehaviour
{
    public static readonly List<BallPhysics> All = new List<BallPhysics>();

    private Vector3 _velocity;
    private float _scrollAngle;
    private float _rotationSpeed = 10f;
    private float _friction = 3f;
    private const float Restitution = 0.95f;
    private const float StopThreshold = 0.05f;

    public float Radius => transform.lossyScale.x / 2f;
    public bool IsMoving => _velocity.sqrMagnitude > 0f;

    void OnEnable()  => All.Add(this);
    void OnDisable() => All.Remove(this);

    void FixedUpdate()
    {
        PerformMove();
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
                if (DetectsBounds(wall.transform, transform.position, rayon, out Vector3 normal, 
                        out float through))
                {
                    transform.position += normal * through;
                    if (Vector3.Dot(_velocity, normal) < 0f)
                        _velocity = Vector3.Reflect(_velocity, normal);
                }
            }
            foreach (BallPhysics other in All)
            {
                if (other == this) continue;
                ResolveBallCollision(other);
            }
        }

        _velocity = Vector3.MoveTowards(_velocity, Vector3.zero, _friction * dt);
        if (_velocity.magnitude < StopThreshold) _velocity = Vector3.zero;
    }

    void ResolveBallCollision(BallPhysics other)
    {
        Vector3 diff = transform.position - other.transform.position;
        diff.y = 0f;
        float dist = diff.magnitude;
        float minDist = Radius + other.Radius;

        if (dist >= minDist) return;
        Vector3 n = dist < 0.0001f ? Vector3.right : diff / dist;
        float overlap = minDist - dist;
        transform.position += n * (overlap / 2f);
        other.transform.position -= n * (overlap / 2f);
        float relative = Vector3.Dot(_velocity - other._velocity, n);
        if (relative >= 0f) return;

        float impulse = -(1f + Restitution) * relative / 2f;
        _velocity += n * impulse;
        other._velocity -= n * impulse;
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