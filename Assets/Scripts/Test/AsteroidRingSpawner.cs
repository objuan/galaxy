using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public class AsteroidRingSpawner : MonoBehaviour
{
    public GameObject asteroidPrefab;

    public int count = 5000;

    public float innerRadius = 15f;
    public float outerRadius = 30f;

    public float centralMass = 10000f;

    const float G = 0.001f;

    Entity asteroidEntityPrefab;

    void Start()
    {
        var world = World.DefaultGameObjectInjectionWorld;

        var manager = world.EntityManager;

        asteroidEntityPrefab =
            manager.CreateEntityQuery(typeof(PrefabReference))
                   .GetSingleton<PrefabReference>()
                   .Value;

        for (int i = 0; i < count; i++)
        {
            Entity asteroid =
                manager.Instantiate(asteroidEntityPrefab);

            float angle =
                UnityEngine.Random.Range(0f, math.PI * 2f);

            float radius =
                UnityEngine.Random.Range(innerRadius, outerRadius);

            float eccentricity =
                UnityEngine.Random.Range(0.92f, 1.08f);

            float3 pos = new float3(
                math.cos(angle) * radius,
                0,
                math.sin(angle) * radius
            );

            manager.SetComponentData(
                asteroid,
                LocalTransform.FromPosition(pos));

            float speed =
                math.sqrt(G * centralMass / radius);

            float3 tangent =
                math.normalize(
                    math.cross(
                        math.normalize(pos),
                        new float3(0, 1, 0)));

            manager.SetComponentData(
                asteroid,
                new CelestialBody
                {
                    Mass = 0.01f,
                    Radius = 0.1f,
                    Velocity = tangent * speed * eccentricity,
                    IsBlackHole = 0
                });
        }
    }
}