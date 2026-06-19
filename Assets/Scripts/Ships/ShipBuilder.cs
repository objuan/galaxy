using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[ExecuteInEditMode]
public class ShipBuilder : MonoBehaviour
{
    PresetConfig cfg;

    public ShipDef shipDef;

    private ShipDef _lastDef;
    private ShipDef currentDef;
    private int _lastHash;

    public int currentLayer;
    private int _currentLayer;

    private bool isPlaying=false;
    private float playTime =0;

    List<GameObject> layers = new List<GameObject>();

    private void OnEnable()
    {
        cfg = GO.Instance<PresetConfig>();
    }

    private void Start()
    {
        Build();

        SetLayer(currentLayer);

        Play();
    }

    private void Update()
    {
#if UNITY_EDITOR

        if (!Application.isPlaying)
        {

            if (shipDef == null)
                return;

            int hash = JsonUtility.ToJson(shipDef).GetHashCode();

            if (hash != _lastHash)
            {
                _lastHash = hash;
                Build();
                SetLayer(currentLayer);
            }


            if (currentLayer != _currentLayer)
            {
                _currentLayer = currentLayer;
                SetLayer(currentLayer);
            }
        }
#endif
        if (isPlaying)
        {
            playTime += Time.deltaTime;
            if (playTime > shipDef.layers[currentLayer].animTime)
            {
                int layer = (currentLayer + 1) % shipDef.layers.Count;
                SetLayer(layer);
                playTime = 0;
            }
        }
    }

    public void Play()
    {
        isPlaying = true;
        playTime = Time.time;
    }

    public void SetLayer(int index)
    {
        currentLayer = index;

        for (int i = 0; i < layers.Count; i++)
        {
            layers[i].SetActive(i == currentLayer);
        }
    }

    public void Build()
    {
        if (shipDef == null)
            return;

        layers.Clear();

        if (cfg.cubePrefab == null)
        {
            Debug.LogError("Cube Prefab missing");
            return;
        }

        MeshFilter sourceFilter =
            cfg.cubePrefab.GetComponent<MeshFilter>();

        if (sourceFilter == null)
        {
            Debug.LogError("Cube prefab has no MeshFilter");
            return;
        }

        Mesh cubeMesh = sourceFilter.sharedMesh;

        ClearChildren();

        for (int layerIndex = 0;
             layerIndex < shipDef.layers.Count;
             layerIndex++)
        {
            BuildLayer(
                layerIndex,
                shipDef.layers[layerIndex],
                cubeMesh);
        }
    }

    private void BuildLayer(
        int layerIndex,
        ShipDefLayer layer,
        Mesh cubeMesh)
    {
        GameObject go =
            new GameObject($"Layer_{layerIndex}");

        go.transform.SetParent(transform, false);

        layers.Add(go);

        MeshFilter mf =
            go.AddComponent<MeshFilter>();

        MeshRenderer mr =
            go.AddComponent<MeshRenderer>();


       mr.sharedMaterials = new Material[]{ cfg.material_face, cfg.material_side };  

        Mesh mesh = BuildMesh(
            layerIndex,
            layer,
            cubeMesh);

        mf.sharedMesh = mesh;
    }

    private Mesh BuildMesh(
        int layerIndex,
        ShipDefLayer layer,
        Mesh cubeMesh)
    {
        List<Vector3> vertices = new();
       // List<int> triangles = new();
        List<Color> colors = new();
        List<Vector2> uvs = new();

        Vector3[] cubeVertices = cubeMesh.vertices;
        int[] cubeTriangles = cubeMesh.triangles;
        Vector2[] cubeUvs = cubeMesh.uv;

        int subMeshCount = cubeMesh.subMeshCount;

        List<int>[] submeshTriangles = new List<int>[subMeshCount];

        for (int i = 0; i < subMeshCount; i++)
        {
            submeshTriangles[i] = new List<int>();
        }

        float scale = shipDef.grid_size;
        float px = shipDef.pivot.x;
        float pz = shipDef.pivot.y;

        foreach (var cell in layer.data)
        {
            int vertexOffset = vertices.Count;

            Vector3 cubePos = new Vector3(
                (cell.pos.x - shipDef.pivot.x ) * scale ,
               0,// layerIndex * scale,
                (cell.pos.y - shipDef.pivot.y ) * scale
            );

            Matrix4x4 matrix = Matrix4x4.TRS(
                cubePos,
                Quaternion.identity,
                Vector3.one * scale
            );

            foreach (var v in cubeVertices)
            {
                vertices.Add(
                    matrix.MultiplyPoint3x4(v));

                colors.Add(cell.color);
            }

            foreach (var uv in cubeUvs)
            {
                uvs.Add(uv);
            }
            
            for (int sm = 0; sm < subMeshCount; sm++)
            {
                int[] tris = cubeMesh.GetTriangles(sm);

                foreach (var t in tris)
                {
                    submeshTriangles[sm].Add(
                        vertexOffset + t
                    );
                }
            }
        }

        Mesh mesh = new Mesh();

        if (vertices.Count > 65000)
            mesh.indexFormat =
                UnityEngine.Rendering.IndexFormat.UInt32;

        mesh.SetVertices(vertices);
        //  mesh.SetTriangles(triangles, 0);
        mesh.subMeshCount = subMeshCount;

        for (int sm = 0; sm < subMeshCount; sm++)
        {
            mesh.SetTriangles(
                submeshTriangles[sm],
                sm
            );
        }
        mesh.SetColors(colors);
        mesh.SetUVs(0, uvs);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    private void ClearChildren()
    {
        List<GameObject> toDelete = new();

        foreach (Transform child in transform)
        {
            toDelete.Add(child.gameObject);
        }

        foreach (var go in toDelete)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(go);
            else
#endif
                Destroy(go);
        }
    }
}