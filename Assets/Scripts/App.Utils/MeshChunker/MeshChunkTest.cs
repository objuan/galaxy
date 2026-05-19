using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace brickgame
{
    [ExecuteInEditMode]
    public class MeshChunkTest : MonoBehaviour
    {
        Vector3 hitPoint;
        List<Vector3> hitSphere = new List<Vector3>();
        List<Vector3> hitSphereN = new List<Vector3>();

        
        Ray ray;
        public float hitRay = 0.2f;

        private void OnEnable()
        {
#if UNITY_EDITOR
            SceneView.onSceneGUIDelegate += UpdateRaycast;
            // OnGUI();
#endif
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            SceneView.onSceneGUIDelegate -= UpdateRaycast;
#endif
        }
        private void Update()
        {
#if UNITY_EDITOR
            MeshChunkPhysicHit hit;
            Ray ray;

            if (Application.isPlaying)
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (MeshChunkPhysic.Raycast(ray, out hit))
                {
                    // Debug.Log("hit " + hit.chunk.worldPosition+" " + hit.cellChunkPosition);
                }
            }

            //if (MeshChunkPhysic.Collide(new BoundingSphere(transform.position, hitRay), out hit))
            //{
            //    hitSphere = hit.point;
            //    hitSphereN = hit.normal;

            //    // Debug.Log("hit " + hit.chunk.worldPosition+" " + hit.cellChunkPosition);
            //}
            //else
            //    hitSphere = Vector3.zero;
            hitSphere.Clear();
            hitSphereN.Clear();

            var all = MeshChunkPhysic.CollideAll(new BoundingSphere(transform.position, hitRay));
            foreach(var h in all)
            {
                hitSphere.Add( h.point);
                hitSphereN.Add( h.normal);

                // Debug.Log("hit " + hit.chunk.worldPosition+" " + hit.cellChunkPosition);
            }
#endif
        }

        void OnDrawGizmos()
        {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(hitPoint, 0.1f);

            Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            Gizmos.DrawSphere(transform.position, hitRay);

            Gizmos.color = Color.yellow;
            for(int i=0;i< hitSphere.Count;i++)
            {
                Gizmos.DrawSphere(hitSphere[i], 0.1f);

                Gizmos.DrawLine(hitSphere[i], hitSphere[i] + hitSphereN[i]);
            }
            // Gizmos.DrawLine(ray.origin, ray.origin + ray.direction * 10);
            //// Gizmos.DrawLine(new Vector3(0,0,0), new Vector3(10,10, 10));

            //Gizmos.DrawSphere(ray.origin + ray.direction*2,0.5f);


        }

#if UNITY_EDITOR
        void UpdateRaycast(SceneView sceneView)
        {
            // then in that function raycast

            if (Event.current.type == EventType.MouseDown)
            {
                Vector3 mousePosition = Event.current.mousePosition;
                mousePosition.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mousePosition.y;
                var mouseRay = sceneView.camera.ScreenPointToRay(mousePosition);

                MeshChunkPhysicHit hit;

                ray = mouseRay;

                //   Debug.Log("ray = " + ray);

                //ray = mouseRay;
                if (MeshChunkPhysic.Raycast(mouseRay, out hit))
                {
                    hitPoint = hit.point;

                    // Debug.Log("hit " + hit.chunk.worldPosition+" " + hit.cellChunkPosition);
                }
            }

        //    Debug.Log(mouseRay);

          //  raycastHits = Physics.RaycastAll(mouseRay, maxDistance)
          // be sure to remove the event if you're not using it or call it from an update function
          //            SceneView.onSceneGUIDelegate -= UpdateRaycast;

            //if (Event.current.type == EventType.MouseDown)
            //{
            //    MeshChunkPhysicHit hit;
            //    var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            //    if (MeshChunkPhysic.Raycast(ray, out hit))
            //    {
            //        // Debug.Log("hit " + hit.chunk.worldPosition+" " + hit.cellChunkPosition);
            //    }
            //}
        }
#endif

    }
}
