using System.Collections.Generic;
using System.Linq;
using UnityEngine;


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

    public Vector3 LocalToWorld( EnemyWaveDefCell cell)
    {
        return waveDef.LocalToWorld(transform.position, cell);
    }
    public Vector3 GetPivot(EnemyWaveDefCell firstCell)
    {
        var enemy_list = enemies.Select(X => X.GetComponent<Enemy>()).Where(X => X.cell.group_idx == firstCell.group_idx).ToList();

        Vector3 accum = Vector3.zero;

        foreach (var enemy in enemy_list)
            accum += LocalToWorld(enemy.cell);

        return accum / enemy_list.Count;    

    }
}