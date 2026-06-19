
using System;
using System.Collections.Generic;
using UnityEngine;




[Serializable]
public class ShipDefCell 
{
    public Vector2Int pos;
    public Color color;
}


[Serializable]
public class ShipDefLayer
{
    public float animTime = 1;

    public List<ShipDefCell> data = new();
}

[CreateAssetMenu(
    fileName = "ShipDef",
    menuName = "Game Data/Ship Def"
)]
public class ShipDef : ScriptableObject
{
    public event Action OnChanged;

    public string id;
    public string enemyName;

    public int grid_w = 10;
    public int grid_h = 10;
    public float grid_size = 1;

    public Vector2Int pivot;

    public List<ShipDefLayer> layers = new();

    private void OnValidate()
    {
        OnChanged?.Invoke();
    }
}


