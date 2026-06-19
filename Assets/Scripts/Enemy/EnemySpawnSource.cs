
using System;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

[CreateAssetMenu(
    fileName = "EnemySpawnSource",
    menuName = "Game Data/Enemy Spawn Source"
)]
public class EnemySpawnSource : ScriptableObject
{
    public string desc = "";
}
