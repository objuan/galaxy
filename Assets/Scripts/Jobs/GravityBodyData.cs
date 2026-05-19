using Unity.Mathematics;

public struct GravityBodyData
{
    public float3 centerOfMass;

    public float mass;

    public float radius;

    public float3 position;

    public float3 velocity;

    public int isBlackHole;
}