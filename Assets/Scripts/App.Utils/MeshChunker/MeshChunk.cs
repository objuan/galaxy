using System;
using System.Collections;
using UnityEngine;


namespace brickgame
{

    [Serializable]
    public class MeshChunk
    {
        [NonSerialized]
        public MeshWorld world;

        public iVector3 chunkIndex;
        public iVector3 startWorldPosition;
        MeshChunkCell[] cells;
        int size1;
        int size2;
        public iVector3 size;

        public MeshChunkCell[] Cells { get => cells; }

        //  BoundsOctree<MeshChunkCell> tree;

        public MeshChunk(MeshWorld world, iVector3 chunkIndex, iVector3 size)
        {
            this.world = world;
            startWorldPosition = new iVector3(chunkIndex.x * size.x, chunkIndex.y * size.y, chunkIndex.z * size.z);
            this.chunkIndex = chunkIndex;
            this.size = size;
            cells = new MeshChunkCell[size.x* size.y* size.z];
            size1 = size.x;
            size2 = size.x*size.y;

         
        }

        public void Clear()
        {
            cells = new MeshChunkCell[size.x * size.y * size.z];
        }

        public MeshChunkCell Get(iVector3 index)
        {
         //   Debug.Log(index);
            var idx = index.x + index.y * size1+ index.z * size2;
            return cells[idx];

        }
        public MeshChunkCell Get(int x,int y,int z)
        {
            if (x >= 0 && y >= 0 && z >= 0
                && x < size.x && y < size.y && z < size.z)
            {
                var idx = x + y * size1 + z * size2;
                return cells[idx];
            }
            else
                return null;
        }

        public void Set(iVector3 index, MeshChunkCell cell)
        {
            var idx = index.x + index.y * size1 + index.z * size2;
            cells[idx] = cell;

            if (cell != null)
            {
                cell.chunk = this;
                cell.chunkInternalPosition = index;
            }

        }

        public void ComputeCollisions()
        {
            
        }


    }
}
