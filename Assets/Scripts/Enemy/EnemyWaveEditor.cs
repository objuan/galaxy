using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[ExecuteInEditMode]
[RequireComponent(typeof(EnemyWave))]
public class EnemyWaveEditor : MonoBehaviour
{
    public EnemyWaveDef waveDef;
    private EnemyWaveDef _lastDef;

    private int _lastHash;

    PresetConfig cfg;

    public EnemyWave wave;

    private void OnEnable()
    {
        wave = GetComponent<EnemyWave>();
        wave.waveDef = waveDef;     

        cfg = GO.Instance<PresetConfig>();
    }

    private void Start()
    {
        Build();
    }

    private void Update()
    {
#if UNITY_EDITOR

        if (!Application.isPlaying)
        { 

            if (waveDef == null)
                return;

            int hash = JsonUtility.ToJson(waveDef).GetHashCode();

            if (hash != _lastHash)
            {
                _lastHash = hash;
                Build();
            }
        }

#endif
    }

    public void Build()
    {
        if (waveDef == null)
            return;

        transform.Clear();
        wave.enemies.Clear();

        /*
#if UNITY_EDITOR
        // Cancella i figli esistenti
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
#else
    foreach (Transform child in transform)
        Destroy(child.gameObject);
#endif
        */
        Vector2Int pivot = waveDef.pivot;

        foreach (var cell in waveDef.data)
        {
            if (cell.index < 0 ||
                cell.index >= waveDef.enemies.Count)
                continue;
            
            var pos = 
                new Vector3(
                    (cell.pos.x - pivot.x) * waveDef.padding,
                    0,
                    (cell.pos.y - pivot.y) * waveDef.padding);

            var enemy = ShipSpawner.Spawn(waveDef.enemies[cell.index], transform, pos);
            wave.enemies.Add(enemy);

            enemy.AddComponent<Enemy>().cell = cell;
            enemy.GetComponent<Enemy>().wave = wave;

        }
    }


}