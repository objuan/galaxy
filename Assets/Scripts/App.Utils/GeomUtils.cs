
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


public static class GeomUtils
{
    public static Vector3 TriangleNormal(Vector3[] vertices)
    {
        var d1 = vertices[0] - vertices[1];
        var d2 = vertices[0] - vertices[2];

        d1 = Vector3.Cross(d1, d2);
        return d1.normalized;
    }
    /*
 *  facce triangolari
 */
    public static Vector3[] ComputeFaceNormals(Vector3[] vertices, int[] faces)
    {
        int nv = vertices.Length;
        int nf = faces.Length / 3;
        Vector3[] normals = new Vector3[nf];

        Vector3 d1 = new Vector3();
        Vector3 d2 = new Vector3();

        /*calculate face normals*/
        for (int f = 0; f < nf; f++)
        {
            int iv1 = faces[f * 3];
            int iv2 = faces[f * 3 + 1];
            int iv3 = faces[f * 3 + 2];

            d1 = vertices[iv1] - vertices[iv2];
            d2 = vertices[iv1] - vertices[iv3];

            d1 = Vector3.Cross(d1, d2);
            d1 = d1.normalized;

            normals[f] = d1;

        }

        return normals;
    }

    public static Vector3[] RemoveUguals(Vector3[] vertices)
    {
        Dictionary<string, Vector3> nMap = new Dictionary<string, Vector3>();
        foreach (Vector3 v in vertices)
        {
            var key = "" + v.x + "" + v.y + "" + v.z;
            if (!nMap.ContainsKey(key)) nMap.Add(key, v);
        }
        return nMap.Values.ToArray();
    }
    public static Vector3 GetCentroid(Vector3[] vertices, ref float ray)
    {
        Vector3 centroid = new Vector3(0, 0, 0);
        foreach (var p in vertices)
        {
            centroid += p;
        }
        centroid = centroid / vertices.Length;

        ray = 999999;
        foreach (var p in vertices)
        {
            var dist = (centroid - p).magnitude;
            ray = Mathf.Min(dist, ray);
        }

        return centroid;
    }
    public static Vector3[] Vertices(this Bounds bounds)
    {
        Vector3[] o = new Vector3[8];
        var boundPoint1 = bounds.min;
        var  boundPoint2 = bounds.max;
        o[0] = boundPoint1;
        o[1] = boundPoint2;
        o[2] = new Vector3(boundPoint1.x, boundPoint1.y, boundPoint2.z);
        o[3] = new   Vector3(boundPoint1.x, boundPoint2.y, boundPoint1.z);
        o[4] = new   Vector3(boundPoint2.x, boundPoint1.y, boundPoint1.z);
        o[5] = new   Vector3(boundPoint1.x, boundPoint2.y, boundPoint2.z);
        o[6] = new   Vector3(boundPoint2.x, boundPoint1.y, boundPoint2.z);
        o[7] = new   Vector3(boundPoint2.x, boundPoint2.y, boundPoint1.z);
        return o;

    }
    public static Vector3[] VerticesScaled(this Bounds bound, Vector3 scale)
    {
        Vector3[] o = new Vector3[8];
        var c = bound.center;
        var size = new Vector3(bound.size.x  * scale.x, bound.size.y * scale.y, bound.size.z * scale.z);
        Bounds bounds = new Bounds(c,size);
        return bounds.Vertices();

    }

    public static Vector3 GetCentroid(Vector3[] pointArray)
    {
        float centroidX = 0.0f;
        float centroidY = 0.0f;
        float centroidZ = 0.0f;

        for (int i = 0; i < pointArray.Length; i++)
        {
            centroidX += pointArray[i].x;
            centroidY += pointArray[i].y;
            centroidZ += pointArray[i].z;
    }
        centroidX /= pointArray.Length;
        centroidY /= pointArray.Length;
        centroidZ /= pointArray.Length;

        return (new Vector3(centroidX, centroidY, centroidZ));
    }
}

 
