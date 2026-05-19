using UnityEngine;

public class Probe : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white; 
        Gizmos.DrawSphere(transform.position,1.5f);
    }
}