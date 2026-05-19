using JetBrains.Annotations;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor.SpeedTree.Importer;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class SpaceField : MonoBehaviour
{
    /*
    struct StartFieldPos
    {
        public Vector2 up_pos;
        public float down_factor;

        public Vector3 GetLocalPos()
        {
            return new Vector3(up_pos.x, 0, up_pos.y);
        }
        public Vector3 GetWorldPos()
        {
            return new Vector3(up_pos.x, 0, up_pos.y);
        }
    }
    */
    struct VoidSnap
    {
        public Void voidObj;
        public Vector2 pos;
        public float ray;
        public float power;
    }

    class StarFieldLayer
    {
        public Vector2[,] points;
    }

    public int seed = 0;
    public float force_range = 10;
    public int sun_count = 1;

    public int width = 100;
    public int depth = 100;
    public float step_line = 1;

    Mesh mesh = null;
    List<VoidSnap> voidList = new List<VoidSnap>();

    StarFieldLayer base_layer;
    Vector3[,] up_layer;

    public AnimationCurve distCurve;

    private void OnEnable()
    {
        Compute();
    }

    void Start()
    {
        Compute();
        //  UnityEngine.Random.InitState(seed);

        CreatePlaneMesh();


    }

    void Update()
    {
        bool isChanged = false;
        foreach (VoidSnap v in voidList)
        {
            var p = v.voidObj.fieldPosition;
            if (p.x != v.pos.x || p.y != v.pos.y || v.ray != v.voidObj.ray || v.power != v.voidObj.power)
                isChanged = true;
        }
        if (isChanged)
        {
            Compute();
            CreatePlaneMesh();
        }
    }

    public Vector3 FieldToWorld(Vector2 base_point)
    {
        float down_factor = 0;
        Vector2 forceSum = Vector2.zero;
        foreach (VoidSnap v in voidList) {
            var diff = base_point - v.pos;
            var dist = diff.magnitude;
            // float dist = Mathf.Max(v.ray, Mathf.Min(diff.magnitude, force_range));
            /*  if (dist < v.ray)
              {
                  return v.pos + diff.normalized * v.ray;
              }


              var max_dist = 20;
              dist = dist - v.ray;
              dist = Mathf.Min(dist, max_dist);   

              var factor = dist / max_dist;
              //var curve = 1f- distCurve.Evaluate(factor) ;
             */
            forceSum += diff.normalized * (v.power);

            dist = 1f- Mathf.Min(v.power,dist ) / v.power;

            down_factor -= dist*10;

            /*
            if (dist < v.power)
            {
                var delta = v.power - v.ray;

                dist = dist - v.ray;

                float f = (dist) / delta;

                Debug.Assert(f >= 0);
                Debug.Assert(f <= 1);

                var curve =  distCurve.Evaluate(f) * 2;

                // float factor = (v.power - v- ray) - dist / v.power;

                forceSum += diff.normalized * curve;
            }
            */

        }
        //Debug.Log(forceSum);
        var p = base_point + forceSum;

        return new Vector3(p.x,0,p.y);// { up_pos = base_point + forceSum, down_factor = down_factor };

    }

    void Compute()
    {
        Debug.Log("Compute");
        voidList.Clear();
        foreach (Void v in GetComponentsInChildren<Void>())
        {
            var snap = new VoidSnap()
            {
                voidObj = v,
                pos = v.fieldPosition,
                ray = v.ray,
                power = v.power
            };
            voidList.Add(snap);
        }

        base_layer = new StarFieldLayer();
        up_layer = new Vector3[width, depth];

        float w_width = width * step_line;
        float w_depth = depth * step_line;

        float widthStep = step_line;
        float depthStep = step_line;

        float halfWidth = w_width / 2.0f;
        float halfDepth = w_depth / 2.0f;

        base_layer.points = new Vector2[width, depth];
        //  up_layer.points = new Vector2[width, depth];

        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                var wx = -halfWidth + x * step_line;
                var wz = -halfDepth + z * step_line;
                base_layer.points[x, z] = new Vector2(wx, wz);

                up_layer[x, z] = FieldToWorld(base_layer.points[x, z]);
            }
        }

        foreach (VoidSnap v in voidList)
        {
            v.voidObj.transform.position = FieldToWorld(v.voidObj.fieldPosition);
        }
    }


    public void CreatePlaneMesh()
    {
        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.name = "SpaceField";

            GetComponent<MeshFilter>().sharedMesh = mesh;
        }

        int vertexCount = (width ) * (depth );

        Vector3[] planeVertices = new Vector3[vertexCount];
        Vector3[] planeNormals = new Vector3[vertexCount];
        Vector2[] planeUVs = new Vector2[vertexCount];

        int vc = 0;
        for (int z = 0; z < depth; z++)
            for (int x = 0; x < width; x++)
            {
                var p = up_layer[x, z];
                planeVertices[vc] = p;
                planeNormals[vc++] = Vector3.up;
            }
        
        //int[] indices = new int[(width + depth) * 2];
        List<int> indices = new List<int>();
  
        for (int z = 0; z < depth-1; z++)
        {
            int startVert = 0 + (z * width);

            for (int x = 0; x < width-1; x++)
            {
                indices.Add(startVert + x);
                indices.Add(startVert + x+1);

                indices.Add(startVert + x);
                indices.Add(startVert + x + width);
            }
        }
        /*
        for (int x = 0; x < width; x++)
        {
            int startVert = x ;
            indices[currentIndex++] = startVert;
            indices[currentIndex++] = startVert + (depth -1)* width  ;

        }
        */


        /*
        // 3. Creiamo le linee VERTICALI (lungo l'asse Z)
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                int startVert = x + (z * width);
                indices[currentIndex++] = startVert;
                indices[currentIndex++] = startVert + depth;
            }
        }
        */

        mesh.vertices = planeVertices;
        mesh.normals = planeNormals;
       // mesh.uv = planeUVs;

        // 4. IMPORTANTE: Impostiamo la mesh come linee invece di triangoli
        mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);


    }

}