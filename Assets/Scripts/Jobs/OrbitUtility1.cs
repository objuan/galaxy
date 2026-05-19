using Unity.Mathematics;

public static class OrbitUtility1
{
    public static float CalculateOrbitalSpeed(
        float G,
        float centralMass,
        float radius)
    {
        return math.sqrt(
            G * centralMass / radius);
    }

    public static float3 CalculateTangential(
        float3 radial)
    {
        return math.normalize(
            math.cross(
                radial,
                new float3(0, 1, 0)));
    }
}