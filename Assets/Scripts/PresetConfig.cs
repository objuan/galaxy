using UnityEngine;

public class PresetConfig : MonoBehaviour
{
    public GameObject star;

    [Header("Cube source")]
    public GameObject cubePrefab;

    [Header("Cube material")]

    public Material material_face;
    public Material material_side;


    [Header("Enemy Setup")]

    public EnemyPath[] paths;


    public EnemySpawnSource[] sources;
}