using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class EnemyWave : MonoBehaviour
{
    public EnemyWaveDef waveDef;
    public List<GameObject> enemies = new List<GameObject>();

    public List<EnemySpawnSource> spawnSources = new List<EnemySpawnSource>();  

    PresetConfig cfg;

    private void OnEnable()
    {
        cfg = GO.Instance<PresetConfig>();
    }


}