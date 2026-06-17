using System.Linq;
using UnityEditor;
using UnityEngine;

public class EnemyDefEditorWindow : EditorWindow
{
    private EnemyDef enemyDef;

    private int selectedLayer;
    private Color selectedColor = Color.red;
    private bool editPivot;

    private readonly Color[] palette =
    {
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow,
        Color.magenta,
        Color.cyan,
        Color.white,
        Color.black,
        new Color(1f,0.5f,0),
        new Color(0.5f,0,1)
    };

    [MenuItem("Tools/Enemy Def Editor")]
    public static void Open()
    {
        GetWindow<EnemyDefEditorWindow>("EnemyDef");
    }

    private void OnGUI()
    {
        DrawAssetSelection();

        if (enemyDef == null)
            return;

        Undo.RecordObject(enemyDef, "EnemyDef Edit");

        DrawProperties();

        GUILayout.Space(10);
        GUILayout.Label("Pivot", EditorStyles.boldLabel);

        enemyDef.pivot =
            EditorGUILayout.Vector2IntField(
                "Pivot",
                enemyDef.pivot);

        DrawLayers();
        DrawPalette();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();    

        editPivot = GUILayout.Toggle(editPivot,   "Edit Pivot",   "Button");
        if (GUILayout.Button("Clear"))
        {
            if (enemyDef.layers.Count > 0)
            {
                Undo.RecordObject(enemyDef, "Clear Layer");

                enemyDef.layers[selectedLayer].data.Clear();

                EditorUtility.SetDirty(enemyDef);

               // enemyDef.OnChanged?.Invoke();
            }
        }

        GUILayout.EndHorizontal();

        DrawGrid();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(enemyDef);
        }
    }

    void DrawAssetSelection()
    {
        if (enemyDef == null && Selection.activeObject is GameObject && ((GameObject)Selection.activeObject).GetComponent<EnemyEditor>())
        {
            enemyDef = ((GameObject)Selection.activeObject).GetComponent<EnemyEditor>().enemyDef;
        }

        enemyDef = (EnemyDef)EditorGUILayout.ObjectField(
            "Enemy Def",
            enemyDef,
            typeof(EnemyDef),
            false);
    }

    void DrawProperties()
    {
        GUILayout.Label("Properties", EditorStyles.boldLabel);

        enemyDef.id =
            EditorGUILayout.TextField("Id", enemyDef.id);

        enemyDef.enemyName =
            EditorGUILayout.TextField("Name", enemyDef.enemyName);

        enemyDef.grid_w =
            EditorGUILayout.IntField("Grid W", enemyDef.grid_w);

        enemyDef.grid_h =
            EditorGUILayout.IntField("Grid H", enemyDef.grid_h);

        enemyDef.grid_size =
            EditorGUILayout.FloatField("Grid Size", enemyDef.grid_size);
    }

    void DrawLayers()
    {
        GUILayout.Space(10);

        GUILayout.Label("Layers", EditorStyles.boldLabel);

        int count =
            EditorGUILayout.IntField(
                "Layer Count",
                enemyDef.layers.Count);

        while (enemyDef.layers.Count < count)
            enemyDef.layers.Add(new EnemyDefLayer());

        while (enemyDef.layers.Count > count)
            enemyDef.layers.RemoveAt(enemyDef.layers.Count - 1);

        if (enemyDef.layers.Count == 0)
            return;

        selectedLayer =
            EditorGUILayout.Popup(
                "Current Layer",
                selectedLayer,
                Enumerable.Range(
                    0,
                    enemyDef.layers.Count)
                .Select(x => $"Layer {x}")
                .ToArray());

        selectedLayer =
            Mathf.Clamp(
                selectedLayer,
                0,
                enemyDef.layers.Count - 1);
    }

    void DrawPalette()
    {
        GUILayout.Space(10);

        GUILayout.Label("Palette", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();

        foreach (var c in palette)
        {
            GUI.backgroundColor = c;

            if (GUILayout.Button("", GUILayout.Width(30), GUILayout.Height(30)))
            {
                selectedColor = c;
            }
        }

        GUI.backgroundColor = Color.white;

        EditorGUILayout.EndHorizontal();

        selectedColor =
            EditorGUILayout.ColorField(
                "Selected",
                selectedColor);
    }

    void DrawGrid()
    {
        if (enemyDef.layers.Count == 0)
            return;

        EnemyDefLayer layer =
            enemyDef.layers[selectedLayer];

        GUILayout.Label("Grid", EditorStyles.boldLabel);

        for (int y = enemyDef.grid_h - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < enemyDef.grid_w; x++)
            {
                Vector2Int pos =
                    new Vector2Int(x, y);

                bool isPivot =
                    enemyDef.pivot.x == x &&
                    enemyDef.pivot.y == y;

                EnemyDefCell cell =
                    layer.data.FirstOrDefault(
                        c => c.pos == pos);

                Color oldColor =
                    GUI.backgroundColor;

                GUI.backgroundColor =
                    cell != null
                        ? cell.color
                        : Color.gray;


                if (GUILayout.Button(
                        isPivot ? "X": "",
                        GUILayout.Width(24),
                        GUILayout.Height(24)))
                {
                    // ToggleCell(layer, pos);
                    if (editPivot)
                    {
                        enemyDef.pivot = pos;
                    }
                    else
                    {
                        ToggleCell(layer, pos);
                    }
                }

                GUI.backgroundColor = oldColor;
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    void ToggleCell(
        EnemyDefLayer layer,
        Vector2Int pos)
    {
        EnemyDefCell cell =
            layer.data.FirstOrDefault(
                c => c.pos == pos);

        if (cell == null)
        {
            layer.data.Add(
                new EnemyDefCell
                {
                    pos = pos,
                    color = selectedColor
                });
        }
        else
        {
            if (cell.color == selectedColor)
            {
                layer.data.Remove(cell);
            }
            else
            {
                cell.color = selectedColor;
            }
        }
    }
}