using UnityEngine;

public class CameraFollowFixed : MonoBehaviour
{
    public Transform target;        // Player
    public Vector3 offset = new Vector3(0, 10, -10); // Posizione fissa relativa
    public float followSpeed = 5f;  // Smooth follow

    void LateUpdate()
    {
        if (target == null) return;

        // Posizione desiderata
        Vector3 desiredPosition = target.position + offset;

        // Smooth movement
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );

        // Guarda sempre il player (angolo fisso implicito)
        transform.LookAt(target);
    }
}