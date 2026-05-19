using NUnit.Framework;

using UnityEngine;

[ExecuteInEditMode]
public class Void : SpaceFieldElement
{
    public float ray = 1;
    public float power = 1;

 
    private void OnEnable()
    {
        transform.localScale = new Vector3(ray*2, ray * 2, ray * 2);
    }

}