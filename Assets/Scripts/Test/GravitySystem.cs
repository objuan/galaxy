
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

/*
[BurstCompile]
public partial struct GravitySystem : ISystem
{
    const float G = 0.001f;
    const float C = 100f;

    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        var entities = new NativeList<Entity>(Allocator.Temp);
        var transforms = new NativeList<LocalTransform>(Allocator.Temp);
        var bodies = new NativeList<CelestialBody>(Allocator.Temp);

        foreach (var (transform, body, entity) in
                 SystemAPI.Query<RefRW<LocalTransform>, RefRW<CelestialBody>>()
                 .WithEntityAccess())
        {
            entities.Add(entity);
            transforms.Add(transform.ValueRO);
            bodies.Add(body.ValueRO);
        }

        for (int i = 0; i < entities.Length; i++)
        {
            var bodyA = bodies[i];
            float3 accel = float3.zero;

            for (int j = 0; j < entities.Length; j++)
            {
                if (i == j) continue;

                var bodyB = bodies[j];

                float3 dir = transforms[j].Position - transforms[i].Position;

                dir.y = 0;

                float distSqr = math.lengthsq(dir) + 0.01f;

                float dist = math.sqrt(distSqr);

                float gravity = G * bodyB.Mass / distSqr;

                // semplificazione relativistica vicino ai buchi neri
                if (bodyB.IsBlackHole == 1)
                {
                    float schwarzschild = 2f * G * bodyB.Mass / (C * C);

                    gravity *= 1f + schwarzschild / math.max(dist, 0.1f);
                }

                accel += math.normalize(dir) * gravity;
            }

            bodyA.Velocity += accel * dt;

            var transform = transforms[i];

            transform.Position += bodyA.Velocity * dt;

            transform.Position.y = 0;

            state.EntityManager.SetComponentData(entities[i], bodyA);
            state.EntityManager.SetComponentData(entities[i], transform);
        }

        entities.Dispose();
        transforms.Dispose();
        bodies.Dispose();
    }
}
*/