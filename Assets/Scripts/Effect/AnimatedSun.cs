using UnityEngine;

public class AnimatedSun : MonoBehaviour
{
    [Header("Impostazioni Pulsazione")]
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.05f;

    [Header("Impostazioni Colore")]
    public Color colorA = Color.yellow;
    public Color colorB = new Color(1f, 0f, 0.5f); // Rosa Neon

    private Vector3 initialScale;
    private Material sunMaterial;

    void Start()
    {
        initialScale = transform.localScale;
        sunMaterial = GetComponent<Renderer>().material;
    }

    void Update()
    {
        // 1. Pulsazione della scala (Respiro del sole)
        float scaleOffset = Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = initialScale + new Vector3(scaleOffset, scaleOffset, 0);

        // 2. Oscillazione del colore HDR (opzionale)
        float colorLerp = (Mathf.Sin(Time.time * 0.5f) + 1f) / 2f;
        sunMaterial.SetColor("_EmissionColor", Color.Lerp(colorA, colorB, colorLerp) * 2f);
    }
}