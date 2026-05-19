using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace brickgame
{

    public class CellCoordinate
    {
        public iVector3 chunk;
        public iVector3 localCoord;
    }

    public class MeshWorldSource : MonoBehaviour
    {
        public MeshWorld meshWorld;
    }


    public class MeshWorld
    {
        public MeshChunk[] chunks;
        List<MeshChunkLayer> layers = new List<MeshChunkLayer>();

        // data 
        public IList<MeshChunkLayer> Layers  => layers;

        // called after the layers, no save
        List<MeshChunkLayer> tempLayers = new List<MeshChunkLayer>();
        public IList<MeshChunkLayer> TempLayers => tempLayers;

        public iVector3 chunksCount;
        public iVector3 worldSize;
        public iVector3 chunkSize;
        public int size;
        public int size2;
        public float leafSize;

        public float leafWorldSize => root.localToWorldMatrix.lossyScale.x * leafSize;

        public Vector3 pivot_pos = new Vector3(0, 0, 0);
        public Quaternion pivot_rot = Quaternion.identity;

        public Transform root;

        public bool modEnabled = false;
        public MeshChunkBuilder builder;
        int forceVisibleLayer;

        public BoundsOctree<MeshChunkCell> tree;

        public iVector3 maxCoordinate => worldSize + new iVector3(-1, -1, -1);

        public Vector3 realSize;// => new Vector3(leafSize * worldSize.x, leafSize * worldSize.y, leafSize * worldSize.z);

        public Bounds bounds => new Bounds(new Vector3(leafSize * 0.5f * worldSize.x, leafSize * 0.5f * worldSize.y, leafSize * worldSize.z * 0.5f),
            realSize);

        public MeshWorld()
        {
        }

        public MeshWorld(Transform root, iVector3 worldSize, iVector3 chunkSize, float leafSize)
        {
            Initialize(root, worldSize, chunkSize, leafSize);

        }

        public void Initialize(Transform root, iVector3 worldSize, iVector3 chunkSize, float leafSize)
        {
            this.worldSize = worldSize;
            this.chunkSize = chunkSize;
            this.leafSize = leafSize;
            //  this.origin = origin;
            this.root = root;
            realSize = new Vector3(leafSize * worldSize.x, leafSize * worldSize.y, leafSize * worldSize.z);

            Resize(worldSize, leafSize);

            if (layers.Count == 0)
                AddLayer();
        }


        public MeshChunkLayer AddLayer()
        {
            MeshChunkLayer layer = new MeshChunkLayer();
            layers.Add(layer);
            layer.Initialize(this, layers.Count);
           // RebuildAllLayerCells();
            return layer;
        }

        public void RemoveLayer(int index)
        {
            layers.RemoveAt(index);
            RebuildAllLayerCells();
        }

        public void RemoveLayer(MeshChunkLayer layer)
        {
            int idx = layers.IndexOf(layer);
           if (idx != -1) RemoveLayer(idx);
        }

        public MeshChunkLayer Layer(int index) => layers[index];

        public MeshChunkLayer AddTempLayer()
        {
            MeshChunkLayer layer = new MeshChunkLayer();
            tempLayers.Add(layer);
            layer.Initialize(this, tempLayers.Count);
            return layer;
        }
        public void ClearTempLayers()
        {
            tempLayers.Clear();
            RebuildAllLayerCells();
        }
        public void RemoveTempLayer(int index)
        {
            tempLayers.RemoveAt(index);
            RebuildAllLayerCells();
        }
        public MeshChunkLayer TempLayer(int index) => tempLayers[index];

        public void Resize(iVector3 newWorldSize,float leafSize)
        {
         //   if (!newWorldSize.Equals(worldSize))
            {
                worldSize = newWorldSize;
                this.leafSize = leafSize;
                realSize = new Vector3(leafSize * worldSize.x, leafSize * worldSize.y, leafSize * worldSize.z);

                Debug.Log("Reshape wd: " + worldSize + " ck: " + chunkSize + " leaf: " + leafSize);

                chunksCount = new iVector3(worldSize.x / chunkSize.x + 1, worldSize.y / chunkSize.y + 1, worldSize.z / chunkSize.z + 1);

                size = chunksCount.x;
                size2 = chunksCount.x * chunksCount.y;
                chunks = new MeshChunk[chunksCount.x * chunksCount.y * chunksCount.z];
                for (int x = 0; x < chunksCount.x; x++)
                {
                    for (int y = 0; y < chunksCount.y; y++)
                    {
                        for (int z = 0; z < chunksCount.z; z++)
                        {
                            chunks[x + y * size + z * size2] = new MeshChunk(this, new iVector3(x, y, z), chunkSize);
                        }
                    }
                }

                builder = GameObject.FindObjectOfType<MeshChunkBuilder>();

                tree = new BoundsOctree<MeshChunkCell>(worldSize.x * chunkSize.x, new Vector3(worldSize.x, worldSize.y, worldSize.z) / 2, leafSize, 1); // origin

                // pivot in mezzo

                pivot_pos = -bounds.center;

                foreach (var layer in layers)
                    layer.Resize(newWorldSize, leafSize);
                foreach (var layer in TempLayers)
                    layer.Resize(newWorldSize, leafSize);
            }
        }

        public IEnumerable<MeshChunkCell> Cells()
        {
            for (int i = 0; i < chunks.Length; i++)
            {
                var cells = chunks[i].Cells;
                for (int c = 0; c < cells.Length; c++)
                {
                    if (cells[c]!=null)
                        yield return cells[c];
                }
            }
        }

        bool intersect(Vector3 c, float radius, Bounds box, ref Vector3 hitVector)
        {
            // get box closest point to sphere center by clamping
            var x = Mathf.Max(box.min.x, Mathf.Min(c.x, box.max.x));
            var y = Mathf.Max(box.min.y, Mathf.Min(c.y, box.max.y));
            var z = Mathf.Max(box.min.z, Mathf.Min(c.z, box.max.z));

            hitVector = new Vector3((x - c.x), (y - c.y), (z - c.z));

            return hitVector.magnitude < radius;
        }

        public Vector3 localToWorldPoint(Vector3 v)
        {
            var localtoWorld = root.localToWorldMatrix;// * buildTrx.inverse;
            return localtoWorld.MultiplyPoint(v);
        }

        public Vector3 localToWorldVector(Vector3 v)
        {
            var localtoWorld = root.localToWorldMatrix;// * buildTrx.inverse;
            return localtoWorld.MultiplyVector(v);
        }
        public Vector3 worldToLocalPoint(Vector3 v)
        {
            var worldToLocal = root.localToWorldMatrix.inverse; // buildTrx * ..
            return worldToLocal.MultiplyPoint(v);
        }
        public Vector3 worldToCellPoint(Vector3 v)
        {
            var worldToLocal = root.localToWorldMatrix.inverse; // buildTrx * ..
            return worldToLocal.MultiplyPoint(v)/ leafSize;
        }

        public Vector3 GetLocalToCell(Vector3 localPosition)
        {
            return  localPosition / leafSize;
        }

        public bool GetCellIndexAt(Vector3 localPosition, ref iVector3 index)
        {
            iVector3 p = new iVector3((int)(localPosition.x / leafSize), (int)(localPosition.y / leafSize), (int)(localPosition.z / leafSize));

            if (p.x < 0 || p.y < 0 || p.z < 0)
                return false;
            if (p.x >= worldSize.x || p.y >= worldSize.y || p.z >= worldSize.z) return false;

            index = p;

            //  Debug.Log("cell at " + localPosition +"=> "+ index);

            return true;
        }

        public bool GetCellIndexAtFromWorld(Vector3 worldPosition, ref iVector3 index)
        {
            Vector3 local = worldToLocalPoint(worldPosition);
            return GetCellIndexAt(local, ref index);
        }

        public Vector3 GetCellLocalCenter(iVector3 cellIndex)
        {
            Vector3 pos = new Vector3(leafSize * 0.5f + cellIndex.x * leafSize, leafSize * 0.5f + cellIndex.y * leafSize, leafSize * 0.5f + cellIndex.z * leafSize);
            return (pos);
        }

        public Vector3 GetCellWorldCenter(iVector3 cellIndex)
        {
            Vector3 pos = new Vector3(leafSize * 0.5f + cellIndex.x * leafSize, leafSize * 0.5f + cellIndex.y * leafSize, leafSize * 0.5f + cellIndex.z * leafSize);
            return localToWorldPoint(pos);
        }

     

        public void Clear(bool clearLayers=true)
        {
            //  chunks = new MeshChunk[chunksCount.x * chunksCount.y * chunksCount.z];
            for (int x = 0; x < chunksCount.x; x++)
            {
                for (int y = 0; y < chunksCount.y; y++)
                {
                    for (int z = 0; z < chunksCount.z; z++)
                    {
                        chunks[x + y * size + z * size2].Clear();
                    }
                }
            }
            if (clearLayers)
            {
                while (layers.Count > 1) layers.RemoveAt(layers.Count - 1);
                layers[0].Clear();
            }

            tree = new BoundsOctree<MeshChunkCell>(worldSize.x * chunkSize.x, new Vector3(worldSize.x, worldSize.y, worldSize.z) / 2, leafSize, 1); // origin +
            builder.Clear();
        }

        public bool EmptyCell(iVector3 cell_index)
        {
            var cell = GetCell(cell_index);
            if (cell != null)
            {
                cell.isVisible = false;
                if (modEnabled)
                    builder.Invalidate(cell.chunk, true);
                tree.Remove(cell);
                //if (cell.bonus)
                //    GameObject.Destroy(cell.bonus);
                return true;
            }
            else return false;
        }

        public bool RefillCell(iVector3 cell_index)
        {
            var c = GetCell(cell_index);
            c.isVisible = true;
            SetChunkCell(cell_index,c,true);
            return true;
        }

        //public void HitCell(iVector3 cell_index)
        //{
        //    var cell = GetCell(cell_index);
        //    if (cell != null)
        //    {
               
        //    }
        //}

        /// <summary>
        /// se ce ne' solo uno visibile lo forzo
        /// </summary>
        /// <returns></returns>
        public int ForcedVisibleLayer()
        {
            int vc = 0;
            foreach (var l in layers)
                vc += l.visible ? 1 : 0;
            if (vc == 1)
                return layers.First(X => X.visible).layerIndex;
            else
                return -1;
        }

        /// <summary>
        /// rifà tutto
        /// </summary>
        public void RebuildAllLayerCells(bool updateMesh=true)
        {
            Clear(false);
            int forceLayer = ForcedVisibleLayer();
            for (int x = 0; x < worldSize.x; x++)
            {
                for (int y = 0; y < worldSize.y; y++)
                {
                    for (int z = 0; z < worldSize.z; z++)
                    {
                        WriteChunkCellMerged(new iVector3(x, y, z), forceLayer,true);
                    }
                }
            }
            if (updateMesh)
                 RebuildMesh();
        }

        public void RebuildMesh()
        {
            root.transform.Clear();

            builder.Build(root.gameObject, this, new Vector3(0, 0, 0), new Vector3(leafSize * chunkSize.x, leafSize * chunkSize.y, leafSize * chunkSize.z),
                    new Vector3(leafSize, leafSize, leafSize));
            
        }

        public void WriteChunkCellMerged( iVector3 cell_index,int forceVisibleLayer,bool enableCutMode)
        {
            // valuto la cella del chunk come fuzione della info dei layers
            MeshChunkCell chunkCell = null;
            for (int i = 0; i < layers.Count; i++)
            {
                if ((forceVisibleLayer==-1 && layers[i].visible) || (forceVisibleLayer>=0  && forceVisibleLayer==i))
                {
                    var cell = layers[i].GetCell(cell_index);
                    if (cell != null)
                    {
                        if (chunkCell == null)
                        {
                            if (layers[i].operation == LayerOperation.Additive || forceVisibleLayer>=0)
                                chunkCell = cell;
                        }
                        else
                        {
                            // merge ?? 
                            if (layers[i].operation == LayerOperation.Additive || forceVisibleLayer >= 0)
                                chunkCell = cell;
                            else
                                chunkCell = null;
                        }
                    }
                }
            }
            // TEMP DOPO TUTTO
            for (int i = 0; i < tempLayers.Count; i++)
            {
                var cell = tempLayers[i].GetCell(cell_index);
                if (cell != null)
                {
                    if (tempLayers[i].operation == LayerOperation.Additive)
                        chunkCell = cell;
                    else if (tempLayers[i].operation == LayerOperation.Substract)
                        chunkCell = null;
                    else if (tempLayers[i].operation == LayerOperation.LogicAnd)
                    {
                        if (chunkCell != null)
                        {
                            // NOP
                         //   chunkCell = cell;
                        }
                        else
                            chunkCell = null;
                    }
                    else if (tempLayers[i].operation == LayerOperation.AddIfEmpty)
                    {
                        if (chunkCell == null)
                        {
                            chunkCell = cell;
                        }
                    }
                }
                else
                {
                    if (tempLayers[i].operation == LayerOperation.LogicAnd)
                    {
                        if (chunkCell != null) chunkCell = null;
                    }
                }
            }

            SetChunkCell(cell_index, chunkCell, enableCutMode);
        }

        /// <summary>
        /// layer event
        /// </summary>
        public void OnSetCellByLayer(MeshChunkLayer layer, iVector3 cell_index,bool enableCutMode)
        {
            // valuto la cella del chunk come fuzione della info dei layers

            WriteChunkCellMerged(cell_index, forceVisibleLayer, enableCutMode);


        }

        /// <summary>
        /// 
        /// </summary>
        void SetChunkCell(iVector3 cell_index, MeshChunkCell cell,bool enableCutMode)
        {
            MeshChunkCell removedCell = null;
            var coord = worldToChunk(cell_index);
            var chunk = chunks[coord.chunk.x + coord.chunk.y * size + coord.chunk.z * size2];

            removedCell = chunk.Get(coord.localCoord);

            chunk.Set(coord.localCoord, cell);

            if (modEnabled)
                builder.Invalidate(chunk, enableCutMode && cell == null);
            //if (!commitChunkList.Contains(chunk))
            //    commitChunkList.Add(chunk);

            if (cell != null)
            {
                // var bounds = new Bounds(new Vector3(origin.x + leafSize * cell_index.x, origin.y + leafSize * cell_index.y, origin.z + leafSize * cell_index.z), new Vector3(leafSize, leafSize, leafSize));
                var bounds = new Bounds(new Vector3(leafSize * 0.5f + leafSize * cell_index.x, leafSize * 0.5f + leafSize * cell_index.y, leafSize * 0.5f + leafSize * cell_index.z), new Vector3(leafSize, leafSize, leafSize));

                cell.bounds = bounds;
                // add or replace ?? 
                if (removedCell != null)
                    tree.Remove(removedCell);
                tree.Add(cell, bounds);
            }
            else if (removedCell != null)
            {
                tree.Remove(removedCell);
                //if (removedCell.bonus)
                //    GameObject.Destroy(removedCell.bonus);
            }
        }

        //public bool IsLocalValid(Vector3 localPos)
        //{
        //    if (localPos.x < 0 || localPos.y < 0 || localPos.z < 0) return false;
        //    if (localPos.x >= realSize.x || localPos.y >= realSize.y || localPos.z >= realSize.z) return false;
        //    return true;
        //}
        public bool IsValid(Vector3 cellPos)
        {
            if (cellPos.x < 0 || cellPos.y < 0 || cellPos.z < 0) return false;
            if (cellPos.x >= worldSize.x || cellPos.y >= worldSize.y || cellPos.z >= worldSize.z) return false;
            return true;

        }
        public bool IsValid(iVector3 cellPos)
        {
            if (cellPos.x < 0 || cellPos.y < 0 || cellPos.z < 0) return false;
            if (cellPos.x >= worldSize.x || cellPos.y >= worldSize.y || cellPos.z >= worldSize.z) return false;
            return true;

        }

        public bool IsOnBound(iVector3 cellPos)
        {
            if (cellPos.x == 0 || cellPos.y == 0 || cellPos.z == 0
                || cellPos.x == worldSize.x - 1 || cellPos.y == worldSize.y - 1 || cellPos.z == worldSize.z - 1) return true;
            else
                return false;

        }
        public bool IsFull(iVector3 cell_index)
        {
            var coord = worldToChunk(cell_index);
            var chunk = chunks[coord.chunk.x + coord.chunk.y * size + coord.chunk.z * size2];
            var c = chunk.Get(coord.localCoord);
            return c != null && c.isVisible;
        }


        public MeshChunkCell GetCell(iVector3 cell_index)
        {
            var coord = worldToChunk(cell_index);
            var chunk = chunks[coord.chunk.x + coord.chunk.y * size + coord.chunk.z * size2];
            return chunk.Get(coord.localCoord);
        }

        public MeshChunkCell GetCell(int x, int y, int z)
        {
            var coord = worldToChunk(new iVector3(x, y, z));
            var chunk = chunks[coord.chunk.x + coord.chunk.y * size + coord.chunk.z * size2];
            return chunk.Get(coord.localCoord);
        }

        public MeshChunk GetChunk(iVector3 index)
        {
            return chunks[index.x + index.y * size + index.z * size2];
        }

        public CellCoordinate worldToChunk(iVector3 worldPos)
        {
            if (worldPos.x < 0 || worldPos.y < 0 || worldPos.z < 0) throw new Exception("bad world coordinate <:" + worldPos);
            if (worldPos.x >= worldSize.x || worldPos.y >= worldSize.y || worldPos.z >= worldSize.z)
                throw new Exception("bad world coordinate >:" + worldPos);

            CellCoordinate coord = new CellCoordinate();
            coord.chunk = new iVector3((int)((float)worldPos.x / chunkSize.x), (int)((float)worldPos.y / chunkSize.y), (int)((float)worldPos.z / chunkSize.z));
            coord.localCoord = new iVector3(worldPos.x - coord.chunk.x * chunkSize.x, worldPos.y - coord.chunk.y * chunkSize.y, worldPos.z - coord.chunk.z * chunkSize.z);
            return coord;
        }

        public void BeginChange()
        {
            modEnabled = true;
            forceVisibleLayer = ForcedVisibleLayer();
        }

        public void CommitChange(bool forceRedraw=false)
        {
            modEnabled = false;
            if (forceRedraw) RebuildMesh();
        }

#region COLLIDE

        public MeshChunkPhysicHit[] CollideAll(BoundingSphere wsphere)
        {
            List<MeshChunkPhysicHit> hitList = new List<MeshChunkPhysicHit>();

            var worldToLocal = root.localToWorldMatrix.inverse; // buildTrx * ..
            BoundingSphere sphere = new BoundingSphere(worldToLocal.MultiplyPoint(wsphere.position), wsphere.radius);


            List<MeshChunkCell> list = new List<MeshChunkCell>();
            tree.GetColliding(list, new Bounds(sphere.position, new Vector3(sphere.radius * 2, sphere.radius * 2, sphere.radius * 2)));
            foreach (var c in list)
            {
                Vector3 hitv = new Vector3();
                if (intersect(sphere.position, sphere.radius, c.bounds, ref hitv))
                {
                    MeshChunkPhysicHit hit = new MeshChunkPhysicHit();
                    hit.cell = c;
                    hit.point = worldToLocal.inverse.MultiplyPoint(sphere.position + hitv);
                    if (Mathf.Abs(hitv.x) > Mathf.Abs(hitv.y) && Mathf.Abs(hitv.x) > Mathf.Abs(hitv.z))
                        hit.normal = new Vector3(-Mathf.Sign(hitv.x), 0, 0);
                    else if (Mathf.Abs(hitv.y) > Mathf.Abs(hitv.z))
                        hit.normal = new Vector3(0, -Mathf.Sign(hitv.y), 0);
                    else
                        hit.normal = new Vector3(0, 0, -Mathf.Sign(hitv.z));
                    hit.normal = worldToLocal.inverse.MultiplyVector(hit.normal);
                    hitList.Add(hit);
                }
            }
            return hitList.ToArray();
        }


        public bool Collide(BoundingSphere wsphere, out MeshChunkPhysicHit hit)
        {
            var worldToLocal = root.localToWorldMatrix.inverse; // buildTrx * ..
            BoundingSphere local_sphere = new BoundingSphere(worldToLocal.MultiplyPoint(wsphere.position), worldToLocal.ExtractScale().x * wsphere.radius);

            hit = new MeshChunkPhysicHit();

            List<MeshChunkCell> list = new List<MeshChunkCell>();
            tree.GetColliding(list, new Bounds(local_sphere.position, new Vector3(local_sphere.radius * 2, local_sphere.radius * 2, local_sphere.radius * 2)));
            if (list.Count > 0)
            {
                MeshChunkCell bestCell = null;
                float bestDist = 999999;
                Vector3 hitVector = Vector3.zero;
                foreach (var c in list)
                {
                    //Debug.Log("dist=" + c.bounds.center);

                    Vector3 hitv = new Vector3();
                    if (intersect(local_sphere.position, local_sphere.radius, c.bounds, ref hitv))
                    {
                        //Debug.Log("hit=" + c.bounds.center);

                        if (hitv.magnitude < bestDist)
                        {
                            bestDist = hitv.magnitude;
                            bestCell = c;
                            hitVector = hitv;
                        }
                    }

                }
                if (bestCell != null)
                {
                    hit.cell = bestCell;
                    hit.point = worldToLocal.inverse.MultiplyPoint(local_sphere.position + hitVector);
                    if (Mathf.Abs(hitVector.x) > Mathf.Abs(hitVector.y) && Mathf.Abs(hitVector.x) > Mathf.Abs(hitVector.z))
                        hit.normal = new Vector3(-Mathf.Sign(hitVector.x), 0, 0);
                    else if (Mathf.Abs(hitVector.y) > Mathf.Abs(hitVector.z))
                        hit.normal = new Vector3(0, -Mathf.Sign(hitVector.y), 0);
                    else
                        hit.normal = new Vector3(0, 0, -Mathf.Sign(hitVector.z));

                    hit.normal = worldToLocal.inverse.MultiplyVector(hit.normal);
                    return true;
                }
                else
                    return false;
            }
            //int layerMask = 1 << LayerMask.NameToLayer("Chunk");

            //foreach (RaycastHit rhit in  Physics.RaycastAll(ray, 9999999, layerMask))
            //{
            //    int y = 0;
            //  //  Debug.Log("hit  = " + rhit.collider.GetComponent<MeshChunkRef>().position);


            //    hit = new MeshChunkPhysicHit();
            //    hit.point = rhit.point;

            //    return true;
            //}

            return false;
        }

        public bool Collide(BoxCollider collider, out MeshChunkPhysicHit hit)
        {
            hit = new MeshChunkPhysicHit();

            List<MeshChunkCell> list = new List<MeshChunkCell>();
            tree.GetColliding(list, collider.bounds);
            if (list.Count > 0)
            {
                MeshChunkCell bestCell = null;
                float bestDist = 999999;
                //foreach (var c in list)
                //{
                //    float dist;
                //   // if (c.bounds.co(ray, out dist))
                //    {
                //        if (dist < bestDist)
                //        {
                //            bestDist = dist;
                //            bestCell = c;
                //        }
                //    }
                //}
                //if (bestCell != null)
                //{
                //    hit.point = ray.GetPoint(bestDist);
                //    return true;
                //}
                //else
                //    return false;
            }

            return false;
        }


        static Vector3[] dirs = new Vector3[] { new Vector3(1, 0, 0), new Vector3(-1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, -1, 0), new Vector3(0, 0, 1), new Vector3(0, 0, -1) };

        public bool Raycast(Ray worldRay, out MeshChunkPhysicHit hit)
        {
            hit = new MeshChunkPhysicHit();

            //Ray ray = worldRay;
            var worldToLocal = root.localToWorldMatrix.inverse; 
            Ray localRay = new Ray(worldToLocal.MultiplyPoint(worldRay.origin), worldToLocal.MultiplyVector(worldRay.direction));
         
            // worldRay
            List<MeshChunkCell> list = new List<MeshChunkCell>();
            tree.GetColliding(list, localRay);

            if (list.Count > 0)
            {
                MeshChunkCell bestCell = null;
                float bestDist = 999999;
                foreach (var c in list)
                {
                    float dist;
                    if (c.bounds.IntersectRay(localRay, out dist))
                    {
                        if (dist < bestDist)
                        {
                            bestDist = dist;
                            bestCell = c;
                        }
                    }
                }
                if (bestCell != null)
                {
                    hit.cell = bestCell;

                    hit.point = worldRay.GetPoint(bestDist * root.localToWorldMatrix.lossyScale.x);
                    var hitVector = -worldRay.direction;
                    float best = 99999;
                    foreach (var dir in dirs)
                    {
                        float d = Mathf.Abs(Vector3.Distance(hit.point, bestCell.position + dir));
                        if (d < best)
                        {
                            best = d;
                            hit.normal = dir;
                        }
                    }
                    return true;
                }
                else
                    return false;
            }
            //int layerMask = 1 << LayerMask.NameToLayer("Chunk");

            //foreach (RaycastHit rhit in  Physics.RaycastAll(ray, 9999999, layerMask))
            //{
            //    int y = 0;
            //  //  Debug.Log("hit  = " + rhit.collider.GetComponent<MeshChunkRef>().position);


            //    hit = new MeshChunkPhysicHit();
            //    hit.point = rhit.point;

            //    return true;
            //}

            return false;
        }
#endregion

#region FIND
        /// <summary>
        ///  solo l'intorno immediato
        /// </summary>
        /// <param name="_from"></param>
        /// <param name="onlyVisible"></param>
        /// <returns></returns>
        public MeshChunkCell[] GetNearest(MeshChunkCell _from,bool onlyVisible=true)
        {
            List<MeshChunkCell> l = new List<MeshChunkCell>();
            var from = _from.worldPosition;
            for (int x = -1; x <= 1; x += 2)
            {
                for (int y = -1; y <= 1; y += 2)
                {
                    for (int z = -1; z <= 1; z += 2)
                    {
                        if (from.x + x >= 0 && from.y + y >= 0 && from.z + z >= 0
                            && from.x + x < worldSize.x &&   from.y + y < worldSize.y &&   from.z + z < worldSize.z)
                        {
                            var cell = GetCell(from.x + x, from.y + y, from.z + z);
                            if (cell != null && ((onlyVisible && cell.isVisible) || (!onlyVisible)))
                                l.Add(cell);
                        }

                    }
                }
            }
            return l.ToArray();
        }

        // get neibourn axis linked
        public MeshChunkCell[] GetAxisLinked(iVector3 from, bool onlyVisible = true)
        {
            List<MeshChunkCell> list = new List<MeshChunkCell>();

            if (from.x > 0)
            {
                var cell = GetCell(from.x - 1, from.y, from.z);
                if (cell != null && ((onlyVisible && cell.isVisible) || (!onlyVisible)))
                {
                    list.Add(cell);
                }
            }
            if (from.x < worldSize.x - 2)
            {
                var cell = GetCell(from.x + 1, from.y, from.z);
                if (cell != null && ((onlyVisible && cell.isVisible) || (!onlyVisible)))
                {
                    list.Add(cell);
                }
            }

            if (from.y > 0)
            {
                var cell = GetCell(from.x, from.y - 1, from.z);
                if (cell != null && ((onlyVisible && cell.isVisible) || (!onlyVisible)))
                {
                    list.Add(cell);
                }
            }
            if (from.y < worldSize.y - 2)
            {
                var cell = GetCell(from.x, from.y + 1, from.z);
                if (cell != null && ((onlyVisible && cell.isVisible) || (!onlyVisible)))
                {
                    list.Add(cell);
                }
            }

            if (from.z > 0)
            {
                var cell = GetCell(from.x, from.y, from.z - 1);
                if (cell != null && ((onlyVisible && cell.isVisible) || (!onlyVisible)))
                {
                    list.Add(cell);
                }
            }
            if (from.z < worldSize.z - 2)
            {
                var cell = GetCell(from.x, from.y, from.z + 1);
                if (cell != null && ((onlyVisible && cell.isVisible) || (!onlyVisible)))
                {
                    list.Add(cell);
                }
            }

            //for (int x = -1; x <= 1; x += 2)
            //{
            //    var cell = GetCell(cellPos.x + x, cellPos.y, cellPos.z);
            //    if (cell != null && ((onlyVisible && cell.isVisible) || (!onlyVisible)))
            //        l.Add(cell);
            //}

            //for (int y = -1; y <= 1; y += 2)
            //{
            //    var cell = GetCell(cellPos.x , cellPos.y+y, cellPos.z);
            //    if (cell != null && ((onlyVisible && cell.isVisible) || (!onlyVisible)))
            //        l.Add(cell);

            //}
            //for (int z = -1; z <= 1; z += 2)
            //{
            //    var cell = GetCell(cellPos.x, cellPos.y , cellPos.z + z);
            //    if (cell != null && ((onlyVisible && cell.isVisible) || (!onlyVisible)))
            //        l.Add(cell);

            //}
            return list.ToArray();
        }

        /// <summary>
        /// only visible
        /// </summary>
        public void _GetNear(MeshChunkCell _from, Func<MeshChunkCell, MeshChunkCell, bool> cond, List<MeshChunkCell> list)
        {
            //  var layer = layers[from.z];
            var from = _from.worldPosition;


            if (from.x > 0)
            {
                var next = GetCell(from.x - 1, from.y, from.z);

                if (next!=null && next.isVisible && 
                    cond(GetCell(from), next)
                    && !list.Contains(next))
                {
                    list.Add(next);
                    _GetNear(next, cond, list);
                }
            }
            if (from.x < worldSize.x-1)
            {
                var next = GetCell(from.x + 1, from.y, from.z);
                if (next != null && next.isVisible && 
                    cond(GetCell(from), next)
                    && !list.Contains(next))
                {
                    list.Add(next);
                    _GetNear(next, cond, list);
                }
            }

            if (from.y > 0)
            {
                var next = GetCell(from.x, from.y-1, from.z);
                if (next != null && next.isVisible && 
                    cond(GetCell(from), next)
                    && !list.Contains(next))
                {
                    list.Add(next);
                    _GetNear(next, cond, list);
                }
            }
            if (from.y < worldSize.y - 1)
            {
                var next = GetCell(from.x, from.y+1, from.z);
                if (next != null && next.isVisible && 
                    cond(GetCell(from), next)
                    && !list.Contains(next))
                {
                    list.Add(next);
                    _GetNear(next, cond, list);
                }
            }

            if (from.z > 0)
            {
                var next = GetCell(from.x, from.y, from.z-1);
                if (next != null && next.isVisible && 
                    cond(GetCell(from), next)
                    && !list.Contains(next))
                {
                    list.Add(next);
                    _GetNear(next, cond, list);
                }
            }
            if (from.z < worldSize.z - 1)
            {
                var next = GetCell(from.x, from.y, from.z+1);
                if (next != null && next.isVisible && 
                    cond(GetCell(from), next)
                    && !list.Contains(next))
                {
                    list.Add(next);
                    _GetNear(next, cond, list);
                }
            }
        }

        /// <summary>
        /// only visible
        /// </summary>
        /// <returns></returns>
        public List<MeshChunkCell> GetNear(MeshChunkCell from, Func<MeshChunkCell, MeshChunkCell, bool> cond)
        {
            var list = new List<MeshChunkCell>();
            list.Add(from);
            _GetNear(from, cond, list);
            return list.Distinct().ToList();
        }

    }
    #endregion
}
