using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace brickgame
{
    public enum LayerOperation
    {
        Additive,
        Substract,
        AddIfEmpty,
        LogicAnd
    }

    public class MeshChunkLayer
    {
        MeshWorld world;

        // PROPS
        public bool _visible = true;
        public LayerOperation _operation = LayerOperation.Additive;

        // 

        public int layerIndex => world.Layers.IndexOf(this);
        //   public MeshChunk[] chunks;
        MeshChunkCell[] cells;

        public iVector3 chunksCount;
        public iVector3 worldSize;
     //   public iVector3 chunkSize;
        public int size;
        public int size2;
        public float leafSize;

        public float leafWorldSize => root.localToWorldMatrix.lossyScale.x * leafSize;

        public Vector3 pivot_pos = new Vector3(0, 0, 0);
        public Quaternion pivot_rot = Quaternion.identity;

        Transform root;

      //  public bool modEnabled = false;
   
        public iVector3 maxCoordinate => worldSize + new iVector3(-1, -1, -1);

        public Vector3 realSize => new Vector3(leafSize * worldSize.x, leafSize * worldSize.y, leafSize * worldSize.z);

        public Bounds bounds => new Bounds(new Vector3(leafSize * 0.5f * worldSize.x, leafSize * 0.5f * worldSize.y, leafSize * worldSize.z * 0.5f),
            realSize);

        public bool visible
        {
            get => _visible;
            set
            {
                if (_visible != value)
                {
                    _visible = value; OnChanged();
                }
            }
        }

        public LayerOperation operation { get => _operation;
            set
            {
                if (_operation != value)
                {
                    _operation = value; OnChanged();
                }
            }
        }

        public MeshChunkLayer()
        {
        }

        public void Initialize(MeshWorld world,  int layerIndex)
        {
            this.world = world;
            this.worldSize = world.worldSize;
           // this.chunkSize = world.chunkSize;
            this.leafSize = world.leafSize;
            //  this.origin = origin;
            this.root = world.root;

            Resize(worldSize, leafSize);
        }

        void OnChanged()
        {
            this.world.RebuildAllLayerCells();
        }

        public void Resize(iVector3 newWorldSize,float leafSize)
        {
         //   if (!newWorldSize.Equals(worldSize))
            {
                worldSize = newWorldSize;
                this.leafSize = leafSize;

                Debug.Log("Reshape layer " + layerIndex+" " +worldSize + " leaf: " + leafSize);

                size = worldSize.x;
                size2 = worldSize.x * worldSize.y;

                cells = new MeshChunkCell[size2 * worldSize.z];

                pivot_pos = -bounds.center;
            }
        }

        public IEnumerable<MeshChunkCell> Cells()
        {
            return cells;
        }

        public bool GetCellPosFromLocal(Vector3 pos, ref iVector3 cellPos)
        {
            var p = new iVector3((int)(pos.x + 0.5f), (int)(pos.y + 0.5f), (int)(pos.z + 0.5f));
            if (p.x < 0 || p.y < 0 || p.z < 0)
                return false;
            if (p.x >= worldSize.x || p.y >= worldSize.y || p.z >= worldSize.z) return false;
            cellPos = p;
            return true;
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

   
        public void Clear()
        {
            var list = cells.ToArray();
            cells = new MeshChunkCell[size2 * worldSize.z];
            foreach (var cell in list)
            {
                if (cell!=null && cell.chunk!=null)
                    world.OnSetCellByLayer(this, cell.worldPosition,false);
            }
        }

        public bool EmptyCell(iVector3 cell_index)
        {
            var cell = GetCell(cell_index);
            if (cell != null)
            {
                cell.isVisible = false;
                //if (modEnabled)
                //    builder.Invalidate(cell.chunk, true);
                //tree.Remove(cell);
                //if (cell.bonus)
                //    GameObject.Destroy(cell.bonus);
                return true;
            }
            else return false;
        }


        public void SetCell(int x, int y, int z, MeshChunkCell cell)
        {
            SetCell(new iVector3(x, y, z), cell);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cell_index"></param>
        /// <param name="cell"></param>
        public void SetCell(iVector3 cell_index, MeshChunkCell cell)
        {
          //  Debug.Log("Set (" + layerIndex+") " +cell_index + "=" +cell);
          //  MeshChunkCell removedCell = null;
            //var coord = worldToChunk(cell_index);
            //var chunk = chunks[coord.chunk.x + coord.chunk.y * size + coord.chunk.z * size2];

          //  removedCell = GetCell(cell_index);

            cells[cell_index.x + cell_index.y * size + cell_index.z * size2] = cell;
            // SetCell(coord.localCoord, cell);

            world.OnSetCellByLayer(this, cell_index,true);
            //if (modEnabled)
            //    builder.Invalidate(chunk, cell == null);
            //if (!commitChunkList.Contains(chunk))
            //    commitChunkList.Add(chunk);

            //if (cell != null)
            //{
            //    // var bounds = new Bounds(new Vector3(origin.x + leafSize * cell_index.x, origin.y + leafSize * cell_index.y, origin.z + leafSize * cell_index.z), new Vector3(leafSize, leafSize, leafSize));
            //    var bounds = new Bounds(new Vector3(leafSize * 0.5f + leafSize * cell_index.x, leafSize * 0.5f + leafSize * cell_index.y, leafSize * 0.5f + leafSize * cell_index.z), new Vector3(leafSize, leafSize, leafSize));
            //    cell.bounds = bounds;
            //    // add or replace ?? 
            //    //if (removedCell != null)
            //    //    tree.Remove(removedCell);
            //    //tree.Add(cell, bounds);
            //}
            //else if (removedCell != null)
            //{
            // //   tree.Remove(removedCell);
            //    //if (removedCell.bonus)
            //    //    GameObject.Destroy(removedCell.bonus);
            //}
        }

        public bool IsValid(iVector3 worldPos)
        {
            if (worldPos.x < 0 || worldPos.y < 0 || worldPos.z < 0) return false;
            if (worldPos.x >= worldSize.x || worldPos.y >= worldSize.y || worldPos.z >= worldSize.z) return false;
            return true;

        }

        public bool IsOnBound(iVector3 worldPos)
        {
            if (worldPos.x == 0 || worldPos.y == 0 || worldPos.z == 0
                || worldPos.x == worldSize.x-1 || worldPos.y == worldSize.y-1 || worldPos.z== worldSize.z-1) return true;
           else
                return false;

        }
        public bool IsFull(iVector3 cell_index)
        {
            //var coord = worldToChunk(cell_index);
            //var chunk = chunks[coord.chunk.x + coord.chunk.y * size + coord.chunk.z * size2];
            var c = GetCell(cell_index);
            return c!=null && c.isVisible;
        }


        public MeshChunkCell GetCell(iVector3 cell_index)
        {
            return cells[cell_index.x + cell_index.y * size + cell_index.z * size2];

            //var coord = worldToChunk(cell_index);
            //var chunk = chunks[coord.chunk.x + coord.chunk.y * size + coord.chunk.z * size2];
            //return chunk.Get(coord.localCoord);
        }

        public MeshChunkCell GetCell(int x, int y, int z)
        {
            return cells[x + y * size + z * size2];

            //var coord = worldToChunk(new iVector3(x, y, z));
            //var chunk = chunks[coord.chunk.x + coord.chunk.y * size + coord.chunk.z * size2];
            //return chunk.Get(coord.localCoord);
        }

        //public MeshChunk GetChunk(iVector3 index)
        //{
        //    return chunks[index.x + index.y * size + index.z * size2];
        //}

        //public CellCoordinate worldToChunk(iVector3 worldPos)
        //{
        //    if (worldPos.x < 0 || worldPos.y < 0 || worldPos.z < 0) throw new Exception("bad world coordinate <:" + worldPos);
        //    if (worldPos.x >= worldSize.x || worldPos.y >= worldSize.y || worldPos.z >= worldSize.z) 
        //        throw new Exception("bad world coordinate >:" + worldPos);

        //    CellCoordinate coord = new CellCoordinate();
        //    coord.chunk = new iVector3((int)((float)worldPos.x / chunkSize.x), (int)((float)worldPos.y / chunkSize.y), (int)((float)worldPos.z / chunkSize.z));
        //    coord.localCoord = new iVector3(worldPos.x - coord.chunk.x * chunkSize.x, worldPos.y - coord.chunk.y * chunkSize.y, worldPos.z - coord.chunk.z * chunkSize.z);
        //    return coord;
        //}

        //public void BeginChange()
        //{
        //    modEnabled = true;
        //    // commitChunkList.Clear();
        //}

        //public void CommitChange()
        //{
        //    modEnabled = false;
        //    //if (commitChunkList.Count > 0)
        //    //{
        //    //    builder.Build(commitChunkList.ToArray());
        //    //    commitChunkList.Clear();
        //    //}
        //}

        //public MeshChunkCell[] Getnear(MeshChunkCell from,int  bool onlyVisible = true)
        //{
        //    List<MeshChunkCell> l = new List<MeshChunkCell>();
        //    var from = from.worldPosition;
        //    for (int x = -1; x <= 1; x += 2)
        //    {
        //        for (int y = -1; y <= 1; y += 2)
        //        {
        //            for (int z = -1; z <= 1; z += 2)
        //            {
        //                var cell = GetCell(from.x + x, from.y + y, from.z + z);
        //                if (cell != null && ((onlyVisible && cell.isVisible) || (!onlyVisible)))
        //                    l.Add(cell);

        //            }
        //        }
        //    }
        //    return l.ToArray();
        //}

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
            if (from.x < worldSize.x-2)
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
            if (from.y < worldSize.y - 2)
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
            if (from.z < worldSize.z - 2)
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
}
