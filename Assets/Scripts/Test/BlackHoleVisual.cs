
using UnityEngine;

public class BlackHoleVisual : MonoBehaviour
{
    public Material distortionMaterial;

    void Update()
    {
        transform.Rotate(Vector3.up * 10f * Time.deltaTime);
    }
}
