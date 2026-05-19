using UnityEngine;

public class AutoRegisterBody : MonoBehaviour
{
    CelestialBody1 body;

    void Awake()
    {
        body = GetComponent<CelestialBody1>();

        GravitySimulationManager.Instance
            .Register(body);
    }

    void OnDestroy()
    {
        if (GravitySimulationManager.Instance != null)
        {
            GravitySimulationManager.Instance
                .Unregister(body);
        }
    }
}