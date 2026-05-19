using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


namespace brickgame
{

    public class MultiMeshLayer
    {
        //   public MeshChunkCell cell;
        public string cellDesc;
        public List<int> tris = new List<int>();
        

    }

    public class MultiMesh
    {
  
        IMaterialSelector materialSelector;

        public MultiMeshLayer borderLayer;
        public MultiMeshLayer faceLayer;
        public MeshSource source;
        public MeshChunk chunk;
        Vector3 chunk_offset;
        Vector3 cell_size;
        int startVertexIndex;

        public List<Vector3> vertices ;
        public List<Color> vcolors;
        public List<Vector2> uvs1;
        public List<Vector2> uvs2;

        public MultiMesh(IMaterialSelector materialSelector,Vector3 chunk_offset, Vector3 cell_size, MeshSource source, MeshChunk chunk)
        {
            this.materialSelector = materialSelector;
            this.chunk_offset = chunk_offset;
            this.cell_size = cell_size;
            this.source = source;
            this.chunk = chunk;
            vertices = new List<Vector3>(10000);
            vcolors = new List<Color>(10000);
            uvs1 = new List<Vector2>(10000);
            uvs2 = new List<Vector2>(10000);
            borderLayer = new MultiMeshLayer();
            faceLayer = new MultiMeshLayer();
        }

        /// <summary>
        /// logica di add
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="source"></param>
        /// <param name="cell"></param>
        /// <param name="startVertexIndex"></param>
        /// <param name="cellIdx"></param>
        /// <param name="triIdx"></param>
        public static void AddTriangle(MultiMeshLayer layer , MeshSource source, int layerIdx, MeshChunk chunk,MeshChunkCell cell,int startVertexIndex, iVector3 cellIdx, int triIdx)
        {
            // controllo se e' in vista
            var tri = source.GetTri(layerIdx, triIdx);

            if (layerIdx==1 &&  !tri.nextCell.Equals(iVector3.zero)) // cell.cellType() == MeshChunkCellType.Filled
            {
                //if ( tri.Ncode == FaceCode.X_P) 
                var nearCell = chunk.Get(cellIdx.x + tri.nextCell.x, cellIdx.y + tri.nextCell.y, cellIdx.z + tri.nextCell.z);
                if (nearCell != null && nearCell.isVisible)
                {
                    // scip
                 //   if (nearCell.cellType() == MeshChunkCellType.Filled)
                        return;
                }
            }

            layer.tris.Add(startVertexIndex + tri.v1);
            layer.tris.Add(startVertexIndex + tri.v2);
            layer.tris.Add(startVertexIndex + tri.v3);
        }

        public void AddObject(MeshChunkCell cell, iVector3 cellIdx)
        {
           //  var offset = new Vector3(chunkWorldSize.x * chunkPos.x, chunkWorldSize.y * chunkPos.y, chunkWorldSize.z * chunkPos.z);
            var offset = chunk_offset +  new Vector3(cell_size.x* cellIdx.x, cell_size.y * cellIdx.y, cell_size.z * cellIdx.z);

            startVertexIndex = vertices.Count;
            vertices.AddRange(source.v.Select( X => X + offset));

            var off_mult = materialSelector.GetMaterialTile(cell.cellMaterial());
            Vector2 off = off_mult.main_offset;
            Vector2 scale = off_mult.main_scale;

            // uvs.AddRange(source.uvs.Select( X => off + X * scale) );
            uvs1.AddRange(source.uvs.Select(X => X * scale + off));

            off = off_mult.sub_offset;
            scale = off_mult.sub_scale;

            // uvs.AddRange(source.uvs.Select( X => off + X * scale) );
            uvs2.AddRange(source.uvs.Select(X => X * scale + off));

            Color color = cell.cellColor();
            for(int i=0;i< source.v.Length;i++)
                vcolors.Add(color);

            cell.startVertexIndex = startVertexIndex;
     
            if (source.tris_border != null)
            {
                for (int i = 0; i < source.tris_border.Length; i++)
                {
                    AddTriangle(borderLayer, source, 0, chunk, cell, startVertexIndex, cellIdx, i);
                }
            }
            for (int i = 0; i < source.tris_faces.Length; i++)
            {
                AddTriangle(faceLayer, source, 1, chunk, cell, startVertexIndex, cellIdx, i);
            }
        }

   

        public Mesh Elapse()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.uv = uvs1.ToArray();
            mesh.uv2 = uvs2.ToArray();
            mesh.colors = vcolors.ToArray();

            if (source.tris_border != null)
            {
                mesh.subMeshCount = 2;

                mesh.SetTriangles(borderLayer.tris.ToArray(), 0);
                mesh.SetTriangles(faceLayer.tris.ToArray(), 1);

            }
            else
            {
                mesh.subMeshCount = 1;
                mesh.SetTriangles(faceLayer.tris.ToArray(), 0);
            }

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        public void Update(Mesh mesh)
        {
            mesh.vertices = vertices.ToArray();
            mesh.uv = uvs1.ToArray();
            mesh.uv2 = uvs2.ToArray();
            mesh.colors = vcolors.ToArray();

            if (source.tris_border != null)
            {
                mesh.subMeshCount = 2;
                mesh.SetTriangles(borderLayer.tris.ToArray(), 0);
                mesh.SetTriangles(faceLayer.tris.ToArray(), 1);

            }
            else
            {
                mesh.subMeshCount = 1;
                mesh.SetTriangles(faceLayer.tris.ToArray(), 0);
            }

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

        }
    }


}
