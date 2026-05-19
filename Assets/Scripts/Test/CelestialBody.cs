
using Unity.Entities;
using Unity.Mathematics;

public struct CelestialBody : IComponentData
{
    public float Mass;
    public float Radius;
    public float3 Velocity;
    public int IsBlackHole;
}
