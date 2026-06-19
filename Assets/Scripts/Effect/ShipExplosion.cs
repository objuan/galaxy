using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShipExplosion : MonoBehaviour
{
    public ShipBuilder shipBuilder;

    public float force = 2;
    public float upwardsForce = 1f;
    public float randomTorque = 5f;
    public float fragmentLifetime = 1f;

    public void Start()
    {
        if (shipBuilder == null)
            return;

        ShipDef shipDef = shipBuilder.shipDef;

        if (shipDef == null)
            return;

        Vector3 center = shipBuilder.transform.position;

        for (int layerIndex = 0;
             layerIndex < shipDef.layers.Count;
             layerIndex++)
        {
            var layer = shipDef.layers[layerIndex];

            foreach (var cell in layer.data)
            {
                SpawnFragment(
                    shipDef,
                    cell,
                    layerIndex,
                    center);
            }
        }

        shipBuilder.gameObject.SetActive(false);
        Destroy(shipBuilder.gameObject, fragmentLifetime);
    }

    private void SpawnFragment(
        ShipDef shipDef,
        ShipDefCell cell,
        int layerIndex,
        Vector3 center)
    {
        PresetConfig cfg = GO.Instance<PresetConfig>();

        GameObject cube =
            Instantiate(cfg.cubePrefab);

        float scale = shipDef.grid_size;

        Vector3 localPos = new Vector3(
            (cell.pos.x - shipDef.pivot.x) * scale,
            layerIndex * scale,
            (cell.pos.y - shipDef.pivot.y) * scale
        );

        cube.transform.position =
            shipBuilder.transform.TransformPoint(localPos);

        cube.transform.rotation =
            shipBuilder.transform.rotation;

        cube.transform.localScale =
            Vector3.one * scale;

        Mesh mesh = cube.GetComponent<MeshFilter>().mesh;
        int nv = mesh.vertexCount;
        var colors = new Color[nv];
        for (int i = 0; i < nv; i++)
            colors[i] = cell.color;

        mesh.colors = colors;

        MeshRenderer mr =
            cube.GetComponent<MeshRenderer>();

        Rigidbody rb =
            cube.AddComponent<Rigidbody>();

        Vector3 dir =
            (cube.transform.position - center).normalized;

        rb.AddForce(
            dir * force +
            Vector3.up * upwardsForce,
            ForceMode.Impulse);

        rb.AddTorque(
            Random.insideUnitSphere * randomTorque,
            ForceMode.Impulse);

        Destroy(cube, fragmentLifetime);
    }
}