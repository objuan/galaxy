
using UnityEngine;

public class EnemySpawnSourcePoint : MonoBehaviour
{
    public EnemySpawnSource source;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 1);
    }
}
