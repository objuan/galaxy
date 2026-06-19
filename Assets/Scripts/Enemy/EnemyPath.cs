
using System;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

[CreateAssetMenu(
    fileName = "EnemyPath",
    menuName = "Game Data/Enemy Path"
)]
public class EnemyPath : ScriptableObject
{
    public string desc = "";
    public List<Vector2> points = new();
}