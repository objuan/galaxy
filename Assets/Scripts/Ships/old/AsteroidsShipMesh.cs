using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteInEditMode]
public class AsteroidsShipMesh : MonoBehaviour
{
    void Start()
    {
        Mesh mesh = new Mesh();
        mesh.name = "AsteroidsShip";

        // 1. Definiamo i vertici (Punta, Base Destra, Base Sinistra, Centro rientrante)
        mesh.vertices = new Vector3[]
        {
            new Vector3(0, 0, 1.5f),    // 0: Punta (Naso)
            new Vector3(0.7f, 0, -0.5f), // 1: Ala Destra
            new Vector3(-0.7f, 0, -0.5f),// 2: Ala Sinistra
            new Vector3(0, 0, 0)         // 3: Centro (per l'effetto a "V" sul retro)
        };

        // 2. Definiamo i triangoli (ordine orario per la visibilità dall'alto)
        mesh.triangles = new int[]
        {
            0, 1, 3, // Metà destra
            0, 3, 2  // Metà sinistra
        };

        // 3. Ricalcoliamo le normali per la luce
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
    }
}