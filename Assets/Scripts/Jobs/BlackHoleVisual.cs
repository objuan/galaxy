using UnityEngine;

public class BlackHoleVisual1 : MonoBehaviour
{
    public float rotationSpeed = 30f;

    void Update()
    {
        transform.Rotate(
            Vector3.up,
            rotationSpeed * Time.deltaTime);
    }
}