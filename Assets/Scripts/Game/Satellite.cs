using NUnit.Framework;

using UnityEngine;

public class Satellite : MonoBehaviour
{

    public Void pivot;

    public float ray;
    public float angularSpeed = 1;

    private float angle;

    [Header("Self Rotation")]
    public bool randomSelfRotation = true;
    public float minSelfRotationSpeed = 10f;
    public float maxSelfRotationSpeed = 100f;

    private Vector3 selfRotationAxis;
    private float selfRotationSpeed;


    private void Start()
    {
        Vector3 offset = transform.position - pivot.transform.position;

        ray = offset.magnitude;

        if (randomSelfRotation)
        {
            selfRotationAxis = Random.onUnitSphere;
            selfRotationSpeed = Random.Range(
                minSelfRotationSpeed,
                maxSelfRotationSpeed
            );
        }
    }

    private void FixedUpdate()
    {
        // Orbita attorno al pivot
        transform.RotateAround(
            pivot.transform.position,
            Vector3.up,
            angularSpeed * Time.fixedDeltaTime
        );

        // Rotazione su sé stesso
        if (randomSelfRotation)
        {
            transform.Rotate(
                selfRotationAxis,
                selfRotationSpeed * Time.fixedDeltaTime,
                Space.Self
            );
        }
        else
        {
            // Punta sempre verso il pivot
            transform.LookAt(pivot.transform);
        }
    }
}
