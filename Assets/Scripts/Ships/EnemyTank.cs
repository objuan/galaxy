using UnityEngine;
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteInEditMode]
public class EnemyTank : MonoBehaviour
{
    void Start()
    {
        Mesh mesh = new Mesh();
        mesh.name = "EnemyTank";

        mesh.vertices = new Vector3[]
        {
            new Vector3(-0.8f, 0, 1f),
            new Vector3(0.8f, 0, 1f),
            new Vector3(0.8f, 0, -1f),
            new Vector3(-0.8f, 0, -1f)
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