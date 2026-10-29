using System.Collections.Generic;
using UnityEngine;

public interface IBall
{
    Transform Transform { get; }
    Vector3 Velocity { get; set; }
    float Radius { get; }
}

public static class BallRegistry
{
    public static readonly List<IBall> All = new List<IBall>();
}

public static class BallCollision
{
    private const float Restitution = 0.95f;

    public static void Resolve(IBall a, IBall b)
    {
        Vector3 diff = a.Transform.position - b.Transform.position;
        diff.y = 0f;
        float dist = diff.magnitude;
        float minDist = a.Radius + b.Radius;

        if (dist >= minDist) return;

        Vector3 n = dist < 0.0001f ? Vector3.right : diff / dist;
        float overlap = minDist - dist;
        a.Transform.position += n * (overlap / 2f);
        b.Transform.position -= n * (overlap / 2f);

        float relative = Vector3.Dot(a.Velocity - b.Velocity, n);
        if (relative >= 0f) return;

        float impulse = -(1f + Restitution) * relative / 2f;
        a.Velocity += n * impulse;
        b.Velocity -= n * impulse;
    }
}

