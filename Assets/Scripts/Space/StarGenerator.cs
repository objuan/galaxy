using UnityEngine;

public class StarGenerator : MonoBehaviour
{
    public int starCount = 1000;
    public float areaSize = 100f;
    public float minSize = 0.05f;
    public float maxSize = 0.2f;

    void Start()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        var main = ps.main;
        main.startLifetime = Mathf.Infinity;
        main.startSpeed = 0;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        ParticleSystem.Particle[] stars = new ParticleSystem.Particle[starCount];

        for (int i = 0; i < starCount; i++)
        {
            stars[i].position = Random.insideUnitSphere * areaSize;
            stars[i].startSize = Random.Range(minSize, maxSize);
            stars[i].startColor = new Color(1, 1, 1, Random.Range(0.5f, 1f));
        }

        ps.SetParticles(stars, starCount);
    }
}