using NUnit.Framework;

using UnityEngine;

[ExecuteInEditMode]
public class Void :MonoBehaviour// : SpaceFieldElement
{
    public float mass = 100f;
    public float ray = 10f;
    public float attractionRadius = 20f;
    public float eventHorizon = 12f;
  
    private void Start()
    {
        transform.localScale = new Vector3(ray * 2, ray * 2, ray * 2);

        GO.Instance<SpaceField>().Voids.Add(this);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color  = Color.green;

        GizmosEx.DrawCircle(transform.position, attractionRadius, 20);


        Gizmos.color = Color.red;

        GizmosEx.DrawCircle(transform.position, eventHorizon, 20);
    }

}