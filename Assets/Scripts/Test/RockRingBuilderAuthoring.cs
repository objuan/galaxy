using Unity.Entities;
using UnityEngine;

public class RockRingBuilderAuthoring : MonoBehaviour
{
    public GameObject asteroidPrefab;

    public int asteroidCount = 5000;

    public float innerRadius = 25f;
    public float outerRadius = 40f;

    public Transform centerBody;

    public float centerMass = 100000f;

    public float eccentricityMin = 0.96f;
    public float eccentricityMax = 1.04f;

    public float randomVelocityNoise = 0.02f;

    public float gravitationalConstant = 0.001f;

    class Baker : Baker<RockRingBuilderAuthoring>
    {
        public override void Bake(
            RockRingBuilderAuthoring authoring)
        {
            Entity entity =
                GetEntity(TransformUsageFlags.None);

            AddComponent(entity,
                new RockRingBuilder
                {
                    AsteroidPrefab =
                        GetEntity(
                            authoring.asteroidPrefab,
                            TransformUsageFlags.Dynamic),

                    AsteroidCount =
                        authoring.asteroidCount,

                    InnerRadius =
                        authoring.innerRadius,

                    OuterRadius =
                        authoring.outerRadius,

                    CenterPosition =
                        authoring.centerBody.position,

                    CenterMass =
                        authoring.centerMass,

                    EccentricityMin =
                        authoring.eccentricityMin,

                    EccentricityMax =
                        authoring.eccentricityMax,

                    RandomVelocityNoise =
                        authoring.randomVelocityNoise,

                    GravitationalConstant =
                        authoring.gravitationalConstant
                });
        }
    }
}