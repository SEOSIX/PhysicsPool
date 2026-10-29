using UnityEngine;

public class BallPhysics : MonoBehaviour, IBall
{
    private Vector3 _velocity;
    private float _friction = 3f;
    private const float StopThreshold = 0.05f;

    public Transform Transform => transform;
    public Vector3 Velocity { get => _velocity; set => _velocity = value; }
    public float Radius => transform.lossyScale.x / 2f;
    public bool IsMoving => _velocity.sqrMagnitude > 0f;

    void OnEnable()  => BallRegistry.All.Add(this);
    void OnDisable() => BallRegistry.All.Remove(this);

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

            foreach (IBall other in BallRegistry.All)
            {
                if (other == (IBall)this) continue;
                BallCollision.Resolve(this, other);
            }
        }

        _velocity = Vector3.MoveTowards(_velocity, Vector3.zero, _friction * dt);
        if (_velocity.magnitude < StopThreshold) _velocity = Vector3.zero;
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