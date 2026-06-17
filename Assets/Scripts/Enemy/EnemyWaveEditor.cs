using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[ExecuteInEditMode]
public class EnemyWaveEditor : MonoBehaviour
{
    public EnemyWaveDef waveDef;
    private EnemyWaveDef _lastDef;

    private int _lastHash;

    PresetConfig cfg;

    private void OnEnable()
    {
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

            EnemyDef enemyDef =
                waveDef.enemies[cell.index];

            GameObject go =
                new GameObject(enemyDef.name);

            go.transform.SetParent(
                transform,
                false);

            go.transform.localPosition =
                new Vector3(
                    (cell.pos.x - pivot.x) * waveDef.padding,
                    0,
                    (cell.pos.y - pivot.y) * waveDef.padding);

            var editor =
                go.AddComponent<EnemyEditor>();

            editor.enemyDef = enemyDef;
        }
    }


}