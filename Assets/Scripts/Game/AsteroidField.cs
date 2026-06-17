using UnityEngine;

public class AsteroidField : MonoBehaviour
{
    [Header("References")]
    public Transform pivot;
    public GameObject asteroidPrefab;

    [Header("Count")]
    public int minAsteroids = 50;
    public int maxAsteroids = 100;

    [Header("Orbit Radius")]
    public float minRadius = 20f;
    public float maxRadius = 100f;

    [Header("Angular Speed")]
    public float minSpeed = 5f;
    public float maxSpeed = 30f;

    [Header("Scale")]
    public float minScale = 0.5f;
    public float maxScale = 3f;

    [Header("Info")]
    public bool inverseDirection = false;

    private void Start()
    {
        int count = Random.Range(minAsteroids, maxAsteroids + 1);

        for (int i = 0; i < count; i++)
        {
            CreateAsteroid();
        }
    }

    private void CreateAsteroid()
    {
        float radius = Random.Range(minRadius, maxRadius);
        float angle = Random.Range(0f, 360f);

        Vector3 position =
            pivot.position +
            new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
                //Random.Range(-5f, 5f),
                0,
                Mathf.Sin(angle * Mathf.Deg2Rad) * radius
            );

        GameObject asteroid = Instantiate(
            asteroidPrefab,
            position,
            Random.rotation,
            transform
        );

        asteroid.transform.localScale =
            Vector3.one * Random.Range(minScale, maxScale);

        Satellite satellite = asteroid.GetComponent<Satellite>();

        if (satellite == null)
            satellite = asteroid.AddComponent<Satellite>();

        satellite.pivot = pivot.GetComponent<Void>(); // oppure cambia il tipo in Transform

        satellite.ray = radius;
        satellite.angularSpeed = Random.Range(minSpeed, maxSpeed);
        satellite.randomSelfRotation = true;

        if (inverseDirection && Random.value > 0.5f)
            satellite.angularSpeed *= -1f;
    }
}