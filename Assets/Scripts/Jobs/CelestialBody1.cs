using Unity.Mathematics;
using UnityEngine;

public class CelestialBody1 : MonoBehaviour
{
    public CelestialBody1 parent;

    public float mass = 1000f;

    public float radius = 1f;

    public bool isBlackHole;

    [HideInInspector]
    public float3 velocity;

    [HideInInspector]
    public float3 position;

    void Awake()
    {
        position = transform.position;
    }

    private void LateUpdate()
    {
        transform.position = position;
    }
}