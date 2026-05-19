using Newtonsoft.Json;
using System.Collections;
using UnityEngine;


namespace brickgame
{
    public enum MeshChunkCellType
    {
        Empty,
        Filled,
       // Bonus
    }

    public class MeshChunkCell 
    {
        [JsonIgnore]
        /// <summary>
        /// in brick coordinate
        /// </summary>
        public Bounds bounds;

        // runtime

        public int meshMaterialIdx;

        public int startVertexIndex;

        /// <summary>
        /// for break events
        /// </summary>
        [JsonIgnore]
        public bool isVisible=true;

        // ==============

        [JsonIgnore]
        public MeshWorld  world => chunk.world;

        [JsonIgnore]
        public  MeshChunk chunk;

        [JsonIgnore]
        public iVector3 chunkInternalPosition;

        public iVector3 worldPosition => iVector3.Add(chunk.startWorldPosition,chunkInternalPosition);

        // in game world coordinate 
        public Vector3 position => chunk.world.localToWorldPoint(bounds.center);
     
        //public virtual string materialKey() { return ""; }

        public virtual MeshChunkMaterial cellMaterial() { return null; }

      //  public virtual MeshChunkCellType cellType() { return MeshChunkCellType.Empty; }

        public virtual Color cellColor() { return Color.white; }

        // utility

        public bool IsInternal()
        {
            var nearList = chunk.world.GetNearest(this,true);
            return nearList.Length == 6;
        }
    }
}
