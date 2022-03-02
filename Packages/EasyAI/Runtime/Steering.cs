using UnityEngine;

public static class Steering
{
    // 114
    public static Vector2 Seek(Vector2 position, Vector2 velocity, Vector2 evader, float speed)
    {
        return (evader - position).normalized * speed - velocity;
    }

    // 115
    // Panic distance in implementation
    public static Vector2 Flee(Vector2 position, Vector2 velocity, Vector2 pursuer, float speed)
    {
        return (position - pursuer).normalized * speed - velocity;
    }

    // 117
    public static Vector2 Pursuit(Vector2 position, Vector2 velocity, Vector2 evader, Vector2 targetLastPosition, float speed, float deltaTime)
    {
        Vector2 toEvader = evader - position;
        float lookAheadTime = toEvader.magnitude / (speed + Vector2.Distance(evader, targetLastPosition) * deltaTime);
        return Seek(position, velocity, evader + (evader - targetLastPosition) / deltaTime * lookAheadTime, speed);
    }

    // 119
    public static Vector2 Evade(Vector2 position, Vector2 velocity, Vector2 pursuer, Vector2 pursuerLastPosition, float speed, float deltaTime)
    {
        Vector2 toPursuer = pursuer - position;
        float lookAheadTime = toPursuer.magnitude / (speed + Vector2.Distance(pursuer, pursuerLastPosition) * deltaTime);
        return Flee(position, velocity, pursuer + (pursuer - pursuerLastPosition) / deltaTime * lookAheadTime, speed);
    }
    
    // aRandom = Random.value - Random.value;
}