using UnityEngine;
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteInEditMode]
public class EnemyDiamond : MonoBehaviour
{
    void Start()
    {
        Mesh mesh = new Mesh();
        mesh.name = "EnemyDiamond";

        mesh.vertices = new Vector3[]
        {
            new Vector3(0, 0, 1.5f),
            new Vector3(1f, 0, 0),
            new Vector3(0, 0, -1.5f),
            new Vector3(-1f, 0, 0)
        };

        mesh.triangles = new int[]
        {
            0,1,2,
            0,2,3
        };

        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;
    }
}