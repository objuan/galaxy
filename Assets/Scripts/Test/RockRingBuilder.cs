using Unity.Entities;
using Unity.Mathematics;

public struct RockRingBuilder : IComponentData
{
    public Entity AsteroidPrefab;

    public int AsteroidCount;

    public float InnerRadius;
    public float OuterRadius;

    public float3 CenterPosition;

    public float CenterMass;

    public float EccentricityMin;
    public float EccentricityMax;

    public float RandomVelocityNoise;

    public float GravitationalConstant;
}