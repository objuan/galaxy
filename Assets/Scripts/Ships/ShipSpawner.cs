
using System;
using System.Collections.Generic;
using UnityEngine;


public class ShipSpawner 
{
    public static GameObject Spawn(ShipDef shipDef,Transform parent, Vector3 localPos)
    {
        GameObject go =
            new GameObject(shipDef.name);

        go.transform.SetParent(
            parent,
            false);

        go.transform.localPosition = localPos;

        var editor =
            go.AddComponent<ShipBuilder>();

        editor.shipDef = shipDef;

        return go;
    }
}


