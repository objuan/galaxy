using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct RockRingBuildSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb =
            new EntityCommandBuffer(
                Unity.Collections.Allocator.Temp);

        foreach (var (ringBuilder, entity)
            in SystemAPI.Query<
                RefRO<RockRingBuilder>>()
            .WithEntityAccess())
        {
            var data = ringBuilder.ValueRO;

            for (int i = 0; i < data.AsteroidCount; i++)
            {
                //--------------------------------
                // CREATE ASTEROID
                //--------------------------------

                Entity asteroid =
                    ecb.Instantiate(
                        data.AsteroidPrefab);

                //--------------------------------
                // POSITION
                //--------------------------------

                float angle =
                    UnityEngine.Random.Range(
                        0f,
                        math.PI * 2f);

                float radius =
                    UnityEngine.Random.Range(
                        data.InnerRadius,
                        data.OuterRadius);

                float3 radialDir =
                    new float3(
                        math.cos(angle),
                        0,
                        math.sin(angle));

                float3 position =
                    data.CenterPosition +
                    radialDir * radius;

                //--------------------------------
                // TRANSFORM
                //--------------------------------

                ecb.SetComponent(
                    asteroid,
                    LocalTransform.FromPosition(
                        position));

                //--------------------------------
                // ORBITAL VELOCITY
                //--------------------------------

                float orbitalSpeed =
                    math.sqrt(
                        data.GravitationalConstant *
                        data.CenterMass /
                        radius);

                float eccentricity =
                    UnityEngine.Random.Range(
                        data.EccentricityMin,
                        data.EccentricityMax);

                float3 tangent =
                    math.normalize(
                        math.cross(
                            radialDir,
                            new float3(0, 1, 0)));

                float3 noise =
                    new float3(
                        UnityEngine.Random.Range(
                            -data.RandomVelocityNoise,
                            data.RandomVelocityNoise),
                        0,
                        UnityEngine.Random.Range(
                            -data.RandomVelocityNoise,
                            data.RandomVelocityNoise));

                float3 velocity =
                    tangent *
                    orbitalSpeed *
                    eccentricity +
                    noise;

                //--------------------------------
                // CELESTIAL BODY
                //--------------------------------
                
                ecb.SetComponent(
                    asteroid,
                    new CelestialBody
                    {
                        Mass = 0.01f,
                        Radius = 0.1f,
                        Velocity = velocity,
                        IsBlackHole = 0
                    });
                
            }

            //--------------------------------
            // REMOVE BUILDER
            //--------------------------------

            ecb.RemoveComponent<
                RockRingBuilder>(
                entity);
        }

        //--------------------------------
        // PLAYBACK
        //--------------------------------

        ecb.Playback(state.EntityManager);

        ecb.Dispose();
    }
}