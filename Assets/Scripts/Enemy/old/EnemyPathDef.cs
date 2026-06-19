
using System;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

[Serializable]
public class EnemyPathPoint11
{
    public Vector2 pos;
    public float time;
}

[Serializable]
public class EnemyPathDef11
{
    public string id;

    // punti della spline
    public List<EnemyPathPoint11> points = new();

}


