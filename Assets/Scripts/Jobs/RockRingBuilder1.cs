using Unity.Mathematics;
using UnityEngine;

public class RockRingBuilder1 : MonoBehaviour
{
    public GameObject centerBody;

    public GameObject asteroidPrefab;

    public float centerMass = 100000f;

    public int asteroidCount = 10000;

    public float innerRadius = 20f;

    public float outerRadius = 40f;

    public float eccentricityMin = 0.96f;

    public float eccentricityMax = 1.04f;

    public float randomVelocityNoise = 0.02f;

    public float gravitationalConstant = 0.001f;

    void Start()
    {
        BuildRing();
    }

    void BuildRing()
    {
        float3 centerPos = centerBody.transform.position;

        for (int i = 0; i < asteroidCount; i++)
        {
            float angle =
                UnityEngine.Random.Range(0f, math.PI * 2f);

            float radius =
                UnityEngine.Random.Range(innerRadius, outerRadius);

            float3 radial =
                new float3(
                    math.cos(angle),
                    0,
                    math.sin(angle));

            float3 position =
                centerPos + radial * radius;

            GameObject asteroid =
                Instantiate(
                    asteroidPrefab,
                    position,
                    quaternion.identity);

            CelestialBody1 body =
                asteroid.GetComponent<CelestialBody1>();

            body.parent = centerBody.GetComponent<CelestialBody1>();

            float orbitalSpeed =
                OrbitUtility1.CalculateOrbitalSpeed(
                    gravitationalConstant,
                    centerMass,
                    radius);

            float eccentricity =
                UnityEngine.Random.Range(
                    eccentricityMin,
                    eccentricityMax);

            float3 tangent =
                OrbitUtility1.CalculateTangential(radial);

            float3 noise =
                new float3(
                    UnityEngine.Random.Range(
                        -randomVelocityNoise,
                        randomVelocityNoise),
                    0,
                    UnityEngine.Random.Range(
                        -randomVelocityNoise,
                        randomVelocityNoise));

            body.velocity =
                tangent * orbitalSpeed * eccentricity +
                noise;

            body.position = position;
        }
    }
}