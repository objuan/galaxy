
using System.Collections.Generic;
using System.Linq;

using UnityEngine;


public class MeshBuilder
{
    List<Vector3> m_Positions = new List<Vector3>();
    List<Vector3> m_Normals = new List<Vector3>();
    List<Vector2> m_Textures = new List<Vector2>();

    class SubMesh
    {
        public List<int> indices = new List<int>();
    }
    List<SubMesh> subMeshes = new List<SubMesh>();  

    public int VertexCount => m_Positions.Count; 

    public void Clear()
    {
        m_Positions.Clear();
        m_Normals.Clear();
        m_Textures.Clear();
        subMeshes.Clear();
        m_Positions.Capacity = 0;
        m_Normals.Capacity = 0;
        m_Textures.Capacity = 0;
        subMeshes.Capacity = 0;
    }

    public void Begin(int subMeshesCount)
    {
        for(int i=0;i< subMeshesCount; i++)
            subMeshes.Add(new SubMesh());
    }

    public void BeginFixed(int vertexCount,params int[] subTriangles)
    {
        m_Positions.Capacity += vertexCount;
        m_Normals.Capacity += vertexCount;
        m_Textures.Capacity += vertexCount;

        foreach (var triangleCount in subTriangles)
            subMeshes.Add(new SubMesh());
    }

    public void AddVertex(Vector3 position, Vector3 normal, Vector2 texture)
    {
        m_Positions.Add(position);
        m_Normals.Add(normal);
        m_Textures.Add(texture);
    }
    public void AddTri(int subMesh, int i1,int i2,int i3)
    {
        subMeshes[subMesh].indices.Add(i1);
        subMeshes[subMesh].indices.Add(i2);
        subMeshes[subMesh].indices.Add(i3);
    }
    public void AddQuad(int subMesh, int i1, int i2, int i3, int i4)
    {
        subMeshes[subMesh].indices.Add(i1);
        subMeshes[subMesh].indices.Add(i2);
        subMeshes[subMesh].indices.Add(i3);

        subMeshes[subMesh].indices.Add(i2);
        subMeshes[subMesh].indices.Add(i4);
        subMeshes[subMesh].indices.Add(i3);
    }
    public void ToMesh(Mesh mesh)
    {
        mesh.SetVertices(m_Positions);
        if (m_Normals.Count == m_Positions.Count)
            mesh.SetNormals(m_Normals);
        if (m_Textures.Count == m_Positions.Count)
            mesh.SetUVs(0, m_Textures);

        mesh.subMeshCount = subMeshes.Count;    
        // mesh.set(subMeshes.Count, UnityEngine.Rendering.MeshUpdateFlags.DontRecalculateBounds);
        for (int i=0;i< subMeshes.Count; i++)
        {
            var subMesh = subMeshes[i];
            mesh.SetTriangles(subMesh.indices, i);
        }
        //if (m_Indices.Count > 0)
        //    mesh.SetTriangles(m_Indices, 0);
        //else
        //    mesh.SetTriangles(Enumerable.Range(0, m_Positions.Count).ToArray(), 0);
    }

}

