using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{

    public GameObject bullet;

    Weapons weapons;

    public float fireRatio = 0.1f;    

    private void Start()
    {
        weapons = GO.Instance<Weapons>();

        StartCoroutine(FireWork()); 
    }

    IEnumerator FireWork()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireRatio);

            Fire();
        }
    }
    public void Fire()
    {
        var go = Instantiate(bullet);
        go.SetParentAtOrigin(weapons.gameObject);
        go.transform.rotation = transform.rotation;
        go.transform.position = transform.position;
    }
}