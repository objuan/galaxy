using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


namespace brickgame
{

    public class MeshChunkMaterial
    {
    }
    public class TileInfo
    {
        public Vector2 main_offset;
        public Vector2 main_scale;
        public Vector2 sub_offset;
        public Vector2 sub_scale;
    }

    public interface IMaterialSelector
    {
        Material GetMaterial(bool isBorder);

        // offset + scale
        TileInfo GetMaterialTile(MeshChunkMaterial mat);

        //  int GetMaterialID(string cellDesc);

        // Material GetMaterial(MeshChunkCell cell);

        //  Material GetMaterial(MeshChunkCellType type);
    }

    public class MeshChunkBuilder : MonoBehaviour
    {
        //  [NonSerialized]
        // params

        public GameObject blockMeshObject;

        [NonSerialized]
        public iVector3 size;
    
        [NonSerialized]
        public MeshWorld world;

        [NonSerialized]
        public IMaterialSelector materialSelector;


        public Vector3 localToWorldOffset;

        public Vector3 cellWorldSize;


      //  [NonSerialized]
        public  Vector3 chunkWorldSize;

        [NonSerialized]
        public MeshSource source;

        [NonSerialized]
        public GameObject root;

        List<(MeshChunk, bool)> rebuildChunkList = new List<(MeshChunk, bool)>();

        bool firstTimeUpdate = true;

        Vector3 lastLeafSize;

        #region SOURCE

        void ElapseSourceMesh(Vector3 cellWorldSize)
        {
            if (source == null || !lastLeafSize.Equals(cellWorldSize) )//|| source.v.Length==0)
            {
                lastLeafSize = cellWorldSize;

                Mesh blockMesh = blockMeshObject.GetComponentInChildren<MeshFilter>().sharedMesh;

                source = new MeshSource();

                //blockMesh.RecalculateBounds();
                var bound = blockMesh.bounds;

                // in basso a sinistra
                Vector3 localToCenter = bound.center + new Vector3(bound.size.y/2, bound.size.y / 2, bound.size.y / 2);
                
                float freeSpace = 0.00f;
                Vector3 scaleToSize = new Vector3((cellWorldSize.x- freeSpace) / bound.size.x, (cellWorldSize.y- freeSpace) / bound.size.y, (cellWorldSize.z- freeSpace) / bound.size.z);

                var v = blockMesh.vertices.Select(X => new Vector3(
                  (X.x + localToCenter.x) * scaleToSize.x ,
                  (X.y + localToCenter.y) * scaleToSize.y,
                  (X.z + localToCenter.z) * scaleToSize.z )).ToArray();

                int subs = blockMesh.subMeshCount;

                for (int s = 0; s < subs; s++)
                {
                    var tris = blockMesh.GetIndices(s);
                    Vector3[] fn = new Vector3[tris.Length / 3];
                    Triangle[] triangles = new Triangle[tris.Length / 3];

                    for (int i = 0, c = 0; i < tris.Length; i += 3, c++)
                    {
                        var v1 = v[tris[i]];
                        var v2 = v[tris[i + 1]];
                        var v3 = v[tris[i + 2]];

                        var FN = -Vector3.Cross(v2 - v1, v2 - v3).normalized;

                        triangles[c] = new Triangle();
                        triangles[c].index = c;
                        triangles[c].v1 = tris[i];
                        triangles[c].v2 = tris[i + 1];
                        triangles[c].v3 = tris[i + 2];
                        triangles[c].N = FN;
                        triangles[c].merged = false;

                        fn[c] = FN;

                        triangles[c].nextCell = iVector3.zero;

                        if (Mathf.Abs(1 - FN.x) < 0.01f) triangles[c].nextCell = new iVector3(1, 0, 0);
                        else if (Mathf.Abs(-1 - FN.x) < 0.01f) triangles[c].nextCell = new iVector3(-1, 0, 0);
                        else if (Mathf.Abs(1 - FN.y) < 0.01f) triangles[c].nextCell = new iVector3(0, 1, 0);
                        else if (Mathf.Abs(-1 - FN.y) < 0.01f) triangles[c].nextCell = new iVector3(0, -1, 0);
                        else if (Mathf.Abs(1 - FN.z) < 0.01f) triangles[c].nextCell = new iVector3(0, 0, 1);
                        else if (Mathf.Abs(-1 - FN.z) < 0.01f) triangles[c].nextCell = new iVector3(0, 0, -1);
                    }
                    if (s==0)
                        source.tris_faces = triangles;
                    else
                        source.tris_border = triangles;
                }

                source.v = v;

                // TODO BETTER
                //Vector2 offset = new Vector2(0.5f, 0.5f);
                //Vector2 scale = new Vector2(1f / 100, 1f / 100);
                Vector2 offset = new Vector2(0,0);
                Vector2 scale = new Vector2(1,1);

                //source.uvs = blockMesh.uv;
                source.uvs = blockMesh.uv.Select(X => new Vector2(1f-X.x, X.y)).ToArray(); // sara la mesh

                //.Select(X => offset + rotate(X, -Mathf.PI / 2) * scale).ToArray();
                //  source.uvs = blockMesh.uv.Select( X => offset+rotate( X ,-Mathf.PI/2)* scale).ToArray();

                // source.tris = tris.Select( X => ;
                //  source.face_normals = fn;
                //   source.face_code = face_code;
                //source.Optimize();
            }
        //    return source;
        }
     
        public static Vector2 rotate(Vector2 v, float delta)
        {
            return new Vector2(
                v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
                v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
            );
        }

        #endregion

        void OnEnable()
        {
            OnPreEnable();
          //  ElapseSourceMesh(cellWorldSize);
        }

        protected virtual void OnPreEnable()
        {
        }
        /// <summary>
        /// rebuild only cselected chunks
        /// </summary>
        /// <param name="chunks"></param>
        public void Update()
        {
#if UNITY_EDITOR
            //if (firstTimeUpdate)
            //{
            //    firstTimeUpdate = false;

            //    Build(root,world,localToWorldOffset,chunkWorldSize,cellWorldSize);
            //}
            //else
            {
                ElapseSourceMesh(cellWorldSize);

                // tutti 
                foreach (var e in rebuildChunkList)
                {
                    var chunk = e.Item1;
                    bool cutMode = e.Item2;

                    var old = root.transform.Find("chunk_" + chunk.chunkIndex.x + "_" + chunk.chunkIndex.y + "_" + chunk.chunkIndex.z).gameObject;

                    ReBuildChunk(old, chunkWorldSize, chunk.chunkIndex, cellWorldSize, chunk, source, cutMode);
                }
                rebuildChunkList.Clear();
            }
#else
            if (rebuildChunkList.Count > 0)
            {
                var chunk = rebuildChunkList[0].Item1;
                bool cutMode = rebuildChunkList[0].Item2;
                rebuildChunkList.RemoveAt(0);

                var old = root.transform.Find("chunk_" + chunk.chunkIndex.x + "_" + chunk.chunkIndex.y + "_" + chunk.chunkIndex.z).gameObject;

                ReBuildChunk(old, chunkWorldSize, chunk.chunkIndex, cellWorldSize, chunk, source, cutMode);

            }
#endif
        }


        public void Invalidate(MeshChunk chunk,bool cutMode)
        {
            var f = rebuildChunkList.FirstOrDefault(X => X.Item1 == chunk);
            if (f == default((MeshChunk, bool))) 
                rebuildChunkList.Add((chunk, cutMode));
            else 
                f.Item2 = f.Item2 & cutMode;
        }


        public void OnLoad(GameObject _root, Vector3 localToWorldOffset, Vector3 chunkWorldSize, Vector3 cellWorldSize)
        {
            this.root = _root;
            this.localToWorldOffset = localToWorldOffset;
            this.cellWorldSize = cellWorldSize;
            this.chunkWorldSize = chunkWorldSize;
            world = GetComponent<MeshWorldSource>().meshWorld;

            
        }

        public void Clear()
        {
            //rebuildChunkList.Clear();
        }

        /// <summary>
        /// build all
        /// </summary>
        /// <param name="_root"></param>
        /// <param name="offset"></param>
        /// <param name="chunkWorldSize"></param>
        /// <param name="cellWorldSize"></param>
        public void Build(GameObject _root,MeshWorld world,  Vector3 localToWorldOffset, Vector3 chunkWorldSize, Vector3 cellWorldSize)
        {
            Debug.Log("build all");

            ElapseSourceMesh(cellWorldSize);

            rebuildChunkList.Clear();

            this.root = _root;
            this.world = world;
            this.localToWorldOffset = localToWorldOffset;
            this.cellWorldSize = cellWorldSize;
            this.chunkWorldSize = chunkWorldSize;

           //  ElapseSourceMesh();

            if (root == null)
            {
                if (transform.Find("chunks"))
                {
                    transform.Find("chunks").Clear();
                    root = transform.Find("chunks").gameObject;

                }
                else
                {
                    root = new GameObject("chunks");
                    root.SetParentAtOrigin(gameObject);

                }
                root.transform.localPosition = localToWorldOffset;
            }

            //var s = GetComponent<MeshWorldSource>();
            //world = s.meshWorld;
            world.builder = this;

            // ================

            var chunksCount = world.chunksCount;

            for (int x = 0; x < chunksCount.x; x++)
            {
                for (int y = 0; y < chunksCount.y; y++)
                {
                    for (int z = 0; z < chunksCount.z; z++)
                    {
                        BuildChunk(null,chunkWorldSize,  new iVector3(x,y,z), cellWorldSize,  world.GetChunk(new iVector3(x,y,z)),source);
                        
                    }
                }
            }
     
        }

       

        protected virtual void OnCreateCell(GameObject chunkGO , MeshChunk chunk, MeshChunkCell cell )
        { 
        }


        public GameObject BuildChunk(GameObject chunkObj,Vector3 chunkWorldSize, iVector3 chunkPos,Vector3 cellSize, MeshChunk chunk, MeshSource sourceMesh)
        {
         //   Debug.Log("build " + chunkPos);

            var chunk_offset = localToWorldOffset+ new Vector3(chunkWorldSize.x * chunkPos.x, chunkWorldSize.y * chunkPos.y, chunkWorldSize.z * chunkPos.z);

            var offset = new Vector3(0, 0, 0);// root.transform.TransformPoint(new Vector3(0, 0, 0)) - localToWorldOffset;

            GameObject go = chunkObj;
            if (chunkObj == null)
            {
                go = new GameObject("chunk_" + chunkPos.x + "_" + chunkPos.y + "_" + chunkPos.z);
                go.SetParentAtOrigin(root);
                go.layer = LayerMask.NameToLayer("Chunk");

                go.AddComponent<MeshChunkRef>().position = chunkPos;
                go.AddComponent<MeshFilter>();
                go.AddComponent<MeshRenderer>();
            }
          
            MultiMesh mesh = new MultiMesh(materialSelector,chunk_offset, cellSize, sourceMesh, chunk);
            mesh.source = sourceMesh;
 
           var size = chunk.size;
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    for (int z = 0; z < size.z; z++)
                    {
                        var cell = chunk.Get(x, y, z);
                        if (cell != null && cell.isVisible)
                        {
                            // Debug.Log("Add "+ chunkPos+" _> " + x + " " +y+" " +z);
                            mesh.AddObject(cell, new iVector3(x, y, z));
                            OnCreateCell(go, chunk, cell);
                        }
                    }
                }
            }

            go.GetComponent<MeshFilter>().sharedMesh = mesh.Elapse();

            //   MeshChunkCell[] subsMat = mesh.LayersMats;

            Material[] mats = null;
            if (source.tris_border != null)
            {
                mats = new Material[2];

                // primo e' il boprdo
                mats[0] = materialSelector.GetMaterial(true);
                mats[1] = materialSelector.GetMaterial(false);

            }
            else
            {
                mats = new Material[1];

                // primo e' il boprdo
                mats[0] = materialSelector.GetMaterial(false);
            }


            //// assign material 
            //mats[0] = materialSelector.GetMaterial( MeshChunkCellType.Filled);
            //mats[1] = materialSelector.GetMaterial(MeshChunkCellType.Transparent);

            go.GetComponent<MeshRenderer>().sharedMaterials = mats;

            //go.GetComponent<MeshRenderer>().sharedMaterial = materialSelector.GetMaterial(null);
            go.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            go.GetComponent<MeshRenderer>().receiveShadows = false;

            // collisions

           // Debug.Log("build " + go.GetComponent<MeshFilter>().sharedMesh.triangles.Length);



            return go;
        }

        public void ReBuildChunk(GameObject chunkObj, Vector3 chunkWorldSize, iVector3 chunkPos, Vector3 cellSize, MeshChunk chunk, MeshSource sourceMesh,bool cutMode)
        {
          //  Debug.Log("REBUILD CHUNK " + chunkPos+ " cutMode "+ cutMode);

            if (!cutMode)
            {
                var chunk_offset = localToWorldOffset + new Vector3(chunkWorldSize.x * chunkPos.x, chunkWorldSize.y * chunkPos.y, chunkWorldSize.z * chunkPos.z);

                MultiMesh mesh = new MultiMesh(materialSelector,chunk_offset, cellSize, sourceMesh, chunk);
                mesh.source = sourceMesh;

                var size = chunk.size;
                for (int x = 0; x < size.x; x++)
                {
                    for (int y = 0; y < size.y; y++)
                    {
                        for (int z = 0; z < size.z; z++)
                        {
                            var cell = chunk.Get(x, y, z);
                            if (cell != null && cell.isVisible)
                            {
                                // Debug.Log("Add "+ chunkPos+" _> " + x + " " +y+" " +z);
                                mesh.AddObject(cell, new iVector3(x, y, z));
                            }
                        }
                    }
                }

                // var orig_mesh = chunkObj.GetComponent<MeshFilter>().sharedMesh;

                chunkObj.GetComponent<MeshFilter>().sharedMesh = mesh.Elapse();
                //mesh.Update(orig_mesh);

            }
            else
            {
            //   Debug.Log("REBUILD CHUNK " + chunkPos);

                var mesh = chunkObj.GetComponent<MeshFilter>().sharedMesh;
                int subs = mesh.subMeshCount;

                var borderLayer = new MultiMeshLayer();
                var faceLayer = new MultiMeshLayer();
              
                var size = chunk.size;
                int faceIDx = (mesh.subMeshCount==1) ? 0: 1;

                for (int x = 0; x < size.x; x++)
                {
                    for (int y = 0; y < size.y; y++)
                    {
                        for (int z = 0; z < size.z; z++)
                        {
                            var cell = chunk.Get(x, y, z);
                            if (cell != null  && cell.isVisible)
                            {
                                var cellIdx = new iVector3(x, y, z);

                                if (source.tris_border != null)
                                {
                                    for (int i = 0; i < source.tris_border.Length; i++)
                                    {
                                        MultiMesh.AddTriangle(borderLayer, source, 0, chunk, cell, cell.startVertexIndex, cellIdx, i);
                                    }
                                }
                                for (int i = 0; i < source.tris_faces.Length; i++)
                                {
                                    MultiMesh.AddTriangle(faceLayer, source, 1, chunk, cell, cell.startVertexIndex, cellIdx, i);
                                }

                            }
                        }
                    }
                }

                if (mesh.subMeshCount == 1)
                    mesh.SetTriangles(faceLayer.tris, 0);
                else
                {
                    mesh.SetTriangles(borderLayer.tris, 0);
                    mesh.SetTriangles(faceLayer.tris, 1);
                }
                mesh.RecalculateNormals();

            }
            //for (int i = 0; i < subs; i++)
            //    mesh.SetTriangles(newIndices[i].tris,i);

        }


           
        //void OnDrawGizmos()
        //{

        //   world.tree.DrawAllBounds();
        //   //if (world!=null && world.tree!=null)
        //   //     world.tree.DrawAllObjects();
        //}

    }
}
