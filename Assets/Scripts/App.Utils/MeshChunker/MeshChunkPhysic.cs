using System;
using System.Collections;
using UnityEngine;


namespace brickgame
{

    public class MeshChunkPhysicHit
    {
        public MeshChunk chunk => cell.chunk;
        public iVector3 cellChunkPosition => cell.chunkInternalPosition;
        public MeshChunkCell cell;

        public Vector3 point;
        public Vector3 normal;

        public float Distance(Vector3 origin) => Vector3.Distance(point, origin);
    }

       
    public class MeshChunkPhysic
    {
        public static bool Raycast(Ray ray, out MeshChunkPhysicHit hit)
        {
            var builder = GO.Instance<MeshWorldSource>();
            return builder.meshWorld.Raycast(ray, out hit);

            //hit = new MeshChunkPhysicHit();
            //return true;
        }
        public static bool Collide(BoundingSphere shpere, out MeshChunkPhysicHit hit)
        {
            var builder = GO.Instance<MeshWorldSource>();
            return builder.meshWorld.Collide(shpere, out hit);

            //hit = new MeshChunkPhysicHit();
            //return true;
        }
        public static MeshChunkPhysicHit[] CollideAll(BoundingSphere shpere)
        {
            var builder = GO.Instance<MeshWorldSource>();
            return builder.meshWorld.CollideAll(shpere);

            //hit = new MeshChunkPhysicHit();
            //return true;
        }
    }
}
