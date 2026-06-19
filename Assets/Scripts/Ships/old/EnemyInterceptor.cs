using UnityEngine;
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteInEditMode]
public class EnemyInterceptor : MonoBehaviour
{
    void Start()
    {
        Mesh mesh = new Mesh();
        mesh.name = "EnemyInterceptor";

        mesh.vertices = new Vector3[]
        {
            new Vector3(0, 0, 1.2f),   // 0 punta
            new Vector3(0.8f, 0, -1f), // 1 destra
            new Vector3(-0.8f, 0, -1f),// 2 sinistra
            new Vector3(0, 0, -0.2f)   // 3 centro
        };

        mesh.triangles = new int[]
        {
            0,1,3,
            0,3,2
        };

        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;
    }
}