using System.Collections;
using UnityEngine;

public class Bullet : Weapon
{
    public float speed = 100;

    SpaceField sf;

    private void Start()
    {
        sf = GO.Instance<SpaceField>();     
    }
    private void Update()
    {
        var p = transform.position; 
        p =  p + speed * transform.forward * Time.deltaTime;
        transform.position = p;

        if (!sf.IsInside(p))
        {
            Destroy(gameObject);    
        }
    }
}