
using Unity.Mathematics;

public static class OrbitUtility
{
    public static float CalculateOrbitalVelocity(
        float G,
        float centralMass,
        float radius)
    {
        return math.sqrt(G * centralMass / radius);
    }

    public static float3 Tangential(float3 dir)
    {
        return math.normalize(math.cross(dir, new float3(0,1,0)));
    }
}
