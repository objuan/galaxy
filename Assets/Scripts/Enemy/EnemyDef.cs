
using System;
using System.Collections.Generic;
using UnityEngine;




[Serializable]
public class EnemyDefCell 
{
    public Vector2Int pos;
    public Color color;
}


[Serializable]
public class EnemyDefLayer
{

    public List<EnemyDefCell> data = new();
}

[CreateAssetMenu(
    fileName = "EnemyDef",
    menuName = "Game Data/Enemy Def"
)]
public class EnemyDef : ScriptableObject
{
    public event Action OnChanged;

    public string id;
    public string enemyName;

    public int grid_w = 10;
    public int grid_h = 10;
    public float grid_size = 1;

    public Vector2Int pivot;

    public List<EnemyDefLayer> layers = new();

    private void OnValidate()
    {
        OnChanged?.Invoke();
    }
}


