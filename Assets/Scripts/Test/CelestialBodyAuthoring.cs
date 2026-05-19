
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class CelestialBodyAuthoring : MonoBehaviour
{
    public float mass = 1000f;
    public float radius = 1f;
    public float3 initialVelocity;
    public bool isBlackHole;

    class Baker : Baker<CelestialBodyAuthoring>
    {
        public override void Bake(CelestialBodyAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new CelestialBody
            {
                Mass = authoring.mass,
                Radius = authoring.radius,
                Velocity = authoring.initialVelocity,
                IsBlackHole = authoring.isBlackHole ? 1 : 0
            });
        }
    }
}
