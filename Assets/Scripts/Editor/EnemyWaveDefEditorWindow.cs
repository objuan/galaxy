using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EnemyWaveDefEditorWindow : EditorWindow
{
    private EnemyWaveDef enemyDef;

    private int selectedEnemy;
    private bool editPivot;
    private Vector2 scroll;

    [MenuItem("Tools/Enemy Wave Def Editor")]
    public static void Open()
    {
        GetWindow<EnemyWaveDefEditorWindow>("EnemyWaveDef");
    }

    private void OnGUI()
    {
        DrawAssetSelection();

        if (enemyDef == null)
            return;

        Undo.RecordObject(enemyDef, "EnemyDef Edit");

        DrawProperties();
        /*
        GUILayout.Space(10);
        GUILayout.Label("Pivot", EditorStyles.boldLabel);

        enemyDef.pivot =
            EditorGUILayout.Vector2IntField(
                "Pivot",
                enemyDef.pivot);

        */
        GUILayout.BeginHorizontal();

        editPivot =
            GUILayout.Toggle(
                editPivot,
                "Edit Pivot",
                "Button");
        
        if (GUILayout.Button("Clear"))
        {
            Undo.RecordObject(enemyDef, "Clear Wave");
            enemyDef.data.Clear();
        }

        GUILayout.EndHorizontal();

        DrawEnemySelection();
        GUILayout.Space(10);
        DrawGrid();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(enemyDef);
        }
    }

    void DrawAssetSelection()
    {
        if (enemyDef == null && Selection.activeObject is  GameObject && ((GameObject)Selection.activeObject).GetComponent< EnemyWaveEditor>())
        {
            enemyDef = ((GameObject)Selection.activeObject).GetComponent<EnemyWaveEditor>().waveDef;
        }

        enemyDef = (EnemyWaveDef)EditorGUILayout.ObjectField(
            "Enemy Wave Def",
            enemyDef,
            typeof(EnemyWaveDef),
            false);

    }

    void DrawProperties()
    {
        GUILayout.Label("Properties", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        enemyDef.id =
            EditorGUILayout.TextField("Id", enemyDef.id);

        enemyDef.desc =
            EditorGUILayout.TextField("Name", enemyDef.desc);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        enemyDef.grid_w =
            EditorGUILayout.IntField("Grid W", enemyDef.grid_w);

        enemyDef.grid_h =
            EditorGUILayout.IntField("Grid H", enemyDef.grid_h);

        
        GUILayout.EndHorizontal();

        enemyDef.padding =
            EditorGUILayout.FloatField("Padding", enemyDef.padding);
    }

    void DrawEnemySelection()
    {
        if (enemyDef == null || enemyDef.enemies.Count == 0)
            return;

        GUILayout.Space(10);

        const int size = 32;
        const int spacing = 2;

        float availableWidth = EditorGUIUtility.currentViewWidth - 30;
        int itemsPerRow = Mathf.Max(1, Mathf.FloorToInt(availableWidth / (size + spacing)));

        int rows = Mathf.CeilToInt((float)enemyDef.enemies.Count / itemsPerRow);

        Rect area = GUILayoutUtility.GetRect(
            availableWidth,
            rows * (size + spacing)
        );
        EditorGUI.DrawRect(area, new Color(0.2f, 0.2f, 0.2f, 1f));
        for (int i = 0; i < enemyDef.enemies.Count; i++)
        {
            int col = i % itemsPerRow;
            int row = i / itemsPerRow;

            Rect rect = new Rect(
                area.x + col * (size + spacing),
                area.y + row * (size + spacing),
                size,
                size
            );

            DrawEnemyMiniature(rect, enemyDef.enemies[i] ,(e)=>
            {
                selectedEnemy = enemyDef.enemies.IndexOf(e);
            }, i == selectedEnemy);
        }
    }

    void DrawEnemyPreview(EnemyDef def, float size=12)
    {
        Rect r =
            GUILayoutUtility.GetRect(
                120,
                120);

        EditorGUI.DrawRect(
            r,
            new Color(.15f, .15f, .15f));

        if (def.layers.Count == 0)
            return;

        var layer = def.layers[0];

        foreach (var cell in layer.data)
        {
            Rect cellRect =
                new Rect(
                    r.x + cell.pos.x * size,
                    r.y + (def.grid_h - 1 - cell.pos.y) * size,
                    size,
                    size);

            EditorGUI.DrawRect(
                cellRect,
                cell.color);
        }
    }

    void DrawGrid()
    {
        const float cellSize = 36;

        scroll =
            EditorGUILayout.BeginScrollView(
                scroll);

        for (int y = enemyDef.grid_h - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < enemyDef.grid_w; x++)
            {
                Vector2Int pos =
                    new Vector2Int(x, y);

                DrawCell(
                    pos,
                    cellSize);
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    EnemyWaveDefCell GetCell(
    Vector2Int pos)
    {
        return enemyDef.data.FirstOrDefault(
            x => x.pos == pos);
    }
    void DrawCell(
        Vector2Int pos,
        float size)
    {
        Rect rect =
            GUILayoutUtility.GetRect(
                size,
                size);

        // Colore delle linee della griglia
        EditorGUI.DrawRect(
            rect,
            new Color(0.1f, 0.1f, 0.1f));

        // Padding interno per lasciare visibili le linee
        const float padding = 1f;

        Rect contentRect = new Rect(
            rect.x + padding,
            rect.y + padding,
            rect.width - padding * 2,
            rect.height - padding * 2);

        // Sfondo della cella
        EditorGUI.DrawRect(
            contentRect,
            new Color(.25f, .25f, .25f));

      

        var cell =
            GetCell(pos);

        if (cell != null)
        {
            DrawEnemyMiniature(
                contentRect,
                enemyDef.enemies[cell.index]);
        }

        if (GUI.Button(
            rect,
            GUIContent.none,
            GUIStyle.none))
        {
            HandleClick(pos);
        }

        bool isPivot =
          enemyDef.pivot == pos;

        if (isPivot)
        {
            DrawBorder(
                contentRect,
                Color.yellow);
        }
    }

    void DrawEnemyMiniature(
    Rect rect,
    EnemyDef def, Action<EnemyDef> onClick=null, bool selected = false)
    {
        if (def.layers.Count == 0)
            return;

        // Gestione click
        if (onClick != null)
        {
            Event e = Event.current;
            if (e.type == EventType.MouseDown &&
                e.button == 0 &&
                rect.Contains(e.mousePosition))
            {
                onClick?.Invoke(def);
                e.Use();
            }
        }

        var layer = def.layers[0];

        float sx =
            rect.width /
            Mathf.Max(1, def.grid_w);

        float sy =
            rect.height /
            Mathf.Max(1, def.grid_h);

        foreach (var c in layer.data)
        {
            Rect r =
                new Rect(
                    rect.x + c.pos.x * sx,
                    rect.y + (def.grid_h - 1 - c.pos.y) * sy,
                    sx,
                    sy);

            EditorGUI.DrawRect(
                r,
                c.color);
        }
        if (selected)
        {
            Handles.DrawSolidRectangleWithOutline(
                rect,
                Color.clear,
                Color.yellow
            );
        }
    }
    void HandleClick(
    Vector2Int pos)
    {
        Undo.RecordObject(
            enemyDef,
            "Edit Wave");

        if (editPivot)
        {
            enemyDef.pivot = pos;
            return;
        }

        var existing =
            GetCell(pos);

        if (existing == null)
        {
            enemyDef.data.Add(
                new EnemyWaveDefCell
                {
                    pos = pos,
                    index = selectedEnemy
                });

            return;
        }

        if (existing.index == selectedEnemy)
        {
            enemyDef.data.Remove(
                existing);

            return;
        }

        existing.index =
            selectedEnemy;

        EditorUtility.SetDirty(
            enemyDef);
    }
    // ===========================================================
    void DrawBorder(Rect rect, Color color, float thickness = 1f)
    {
        // Top
        EditorGUI.DrawRect(
            new Rect(rect.x, rect.y, rect.width, thickness),
            color);

        // Bottom
        EditorGUI.DrawRect(
            new Rect(rect.x, rect.yMax - thickness, rect.width, thickness),
            color);

        // Left
        EditorGUI.DrawRect(
            new Rect(rect.x, rect.y, thickness, rect.height),
            color);

        // Right
        EditorGUI.DrawRect(
            new Rect(rect.xMax - thickness, rect.y, thickness, rect.height),
            color);
    }
}