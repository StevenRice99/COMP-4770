using UnityEngine;

public static class Steering
{
    // 114
    public static Vector2 Seek(Vector2 position, Vector2 velocity, Vector2 target, float speed)
    {
        // return (target - position).normalized * Mathf.Min(speed, Vector2.Distance(position, target)) - velocity;
        return (target - position).normalized * speed - velocity;
    }

    // 115
    // Panic distance in implementation
    public static Vector2 Flee(Vector2 position, Vector2 velocity, Vector2 pursuer, float speed)
    {
        return (position - pursuer).normalized * speed - velocity;
    }

    // 117
    public static Vector2 Pursuit(Vector2 position, Vector2 velocity, Vector2 target, Vector2 targetLastPosition, float speed, float deltaTime)
    {
        Vector2 toEvader = target - position;
        float lookAheadTime = toEvader.magnitude / (speed + Vector2.Distance(target, targetLastPosition) * deltaTime);
        return Seek(position, velocity, target + (target - targetLastPosition) / deltaTime * lookAheadTime, speed);
    }

    // 119
    public static Vector2 Evade(Vector2 position, Vector2 velocity, Vector2 pursuer, Vector2 pursuerLastPosition, float speed, float deltaTime)
    {
        Vector2 toPursuer = pursuer - position;
        float lookAheadTime = toPursuer.magnitude / (speed + Vector2.Distance(pursuer, pursuerLastPosition) * deltaTime);
        return Flee(position, velocity, pursuer + (pursuer - pursuerLastPosition) / deltaTime * lookAheadTime, speed);
    }

    // 120
    public static Vector2 Wander(Transform tr, Vector2 target, float wanderJitter, float wanderRadius, float wanderDistance)
    {
        target += new Vector2(Random.value * wanderJitter, Random.value * wanderJitter);
        target.Normalize();
        target *= wanderRadius;
        Vector2 targetLocal = target + new Vector2(0, wanderDistance);
        Vector3 targetWorld = tr.TransformVector(new Vector3(targetLocal.x, 0, targetLocal.y));
        Vector3 position = tr.position;
        return new Vector2(targetWorld.x, targetWorld.z) - new Vector2(position.x, position.z);
    }
}