using UnityEngine;

public class Weapon : MonoBehaviour
{
    private void Awake()
    {
        gameObject.tag = "weapon";
        gameObject.layer = LayerMask.NameToLayer("weapon");
    }

}