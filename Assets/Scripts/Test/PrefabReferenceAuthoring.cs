using Unity.Entities;
using UnityEngine;

public class PrefabReferenceAuthoring : MonoBehaviour
{
    public GameObject prefab;

    class Baker : Baker<PrefabReferenceAuthoring>
    {
        public override void Bake(
            PrefabReferenceAuthoring authoring)
        {
            var entity =
                GetEntity(TransformUsageFlags.None);

            AddComponent(entity,
                new PrefabReference
                {
                    Value = GetEntity(
                        authoring.prefab,
                        TransformUsageFlags.Dynamic)
                });
        }
    }
}