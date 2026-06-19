using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteInEditMode]
public class EnemyUFO : MonoBehaviour
{
    void Start()
    {
        Mesh mesh = new Mesh();
        mesh.name = "EnemyUFO";

        mesh.vertices = new Vector3[]
        {
            new Vector3(0, 0, 1f),     // 0 front
            new Vector3(0.8f, 0, 0),   // 1 right
            new Vector3(0, 0, -1f),    // 2 back
            new Vector3(-0.8f, 0, 0),  // 3 left
            new Vector3(0, 0.2f, 0)    // 4 top (cupola)
        };

        mesh.triangles = new int[]
        {
            0,1,4,
            1,2,4,
            2,3,4,
            3,0,4,

            0,3,2,
            0,2,1
        };

        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;
    }
}