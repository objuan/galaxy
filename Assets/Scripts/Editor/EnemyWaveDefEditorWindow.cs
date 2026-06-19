using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;


public class EnemyWaveDefEditorWindow : EditorWindow
{
    public enum EditMode
    {
        AddDel, Pivot, Select
    }
    private EnemyWaveDef waveDef;

    private int selectedEnemy;
    private int selectedPath;
    private EnemyWaveDefCell selectedShip=null;

    private EditMode editMode = EditMode.AddDel;
    private Vector2 scroll;

    [MenuItem("Tools/Enemy Wave Def Editor")]
    public static void Open()
    {
        GetWindow<EnemyWaveDefEditorWindow>("EnemyWaveDef");
    }

    private void OnGUI()
    {
        DrawAssetSelection();

        if (waveDef == null)
            return;

        Undo.RecordObject(waveDef, "waveDef Edit");

        DrawProperties();
       // DrawSourceEditor();
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();

        editMode = (EditMode)GUILayout.Toolbar(
          (int)editMode,
          new string[] { "AddDel", "Pivot", "Select" });


        if (GUILayout.Button("Clear"))
        {
            Undo.RecordObject(waveDef, "Clear Wave");
            waveDef.data.Clear();
        }

        GUILayout.EndHorizontal();

        
        DrawEnemySelection();
        GUILayout.Space(10);
        DrawGrid();

        DrawCellSelection();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(waveDef);
        }
    }

    void DrawCellSelection()
    {
        if (selectedShip!=null)
        {
            // PATHS 

            EnemyPath[] availablePaths = GO.Instance<PresetConfig>().paths;

            string[] pathNames = availablePaths
                .Select(x => x.name)
                .Prepend("<NULL>")
                .ToArray();

            GUILayout.BeginHorizontal();

            int idx = Array.FindIndex( availablePaths,  p => p == selectedShip.enter_path);
            idx=idx+1; 
            
            GUILayout.Label($"Path {selectedShip.index} {selectedShip.pos}");

            idx = EditorGUILayout.Popup(  "",idx,pathNames);

            if (idx != -1)
            {
                EnemyPath newID  = null;
                if (idx > 0 ) 
                  newID = availablePaths[idx-1];

                if (newID != selectedShip.enter_path)
                {
                    selectedShip.enter_path = newID;
                    EditorUtility.SetDirty(waveDef);
                }
              

                if (idx > 0)
                {

                    if (GUILayout.Button(
                       "Remove",
                       GUILayout.Width(80)))
                    {
                        Undo.RecordObject(
                            waveDef,
                            "Remove Path");

                        selectedShip.enter_path = null;

                        EditorUtility.SetDirty(waveDef);

                    }
                }
            }
            GUILayout.EndHorizontal();

            // SOURCE
            GUILayout.BeginHorizontal();

            EnemySpawnSource[] availableSources = GO.Instance<PresetConfig>().sources;

            string[] names  = availableSources
                .Select(x => x.name)
                .Prepend("<NULL>")
                .ToArray();

            idx = Array.FindIndex(availableSources, p => p == selectedShip.enter_source);
            idx = idx + 1;

            GUILayout.Label($"Source ");

            idx = EditorGUILayout.Popup("", idx, names);

            if (idx != -1)
            {
                EnemySpawnSource newID = null;
                if (idx > 0)
                    newID = availableSources[idx-1];

                if (newID != selectedShip.enter_source)
                {
                    selectedShip.enter_source = newID;
                    EditorUtility.SetDirty(waveDef);
                }
                
            }
            GUILayout.EndHorizontal();

        }
    }

    void DrawSourceEditor()
    {
        EditorGUILayout.Space();
        /*EditorGUILayout.LabelField(
            "Enemy Paths",
            EditorStyles.boldLabel);
        */

        EditorGUILayout.LabelField("Spawn Sources");

        for (int i = 0; i < waveDef.sources.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            waveDef.sources[i] =
                (EnemySpawnSource)EditorGUILayout.ObjectField(
                    waveDef.sources[i],
                    typeof(EnemySpawnSource),
                    true); // true = consente scene objects

            if (GUILayout.Button("-", GUILayout.Width(25)))
            {
                waveDef.sources.RemoveAt(i);
                GUIUtility.ExitGUI();
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Source"))
        {
            waveDef.sources.Add(null);
        }
    }
    void DrawPathEditor()
    {
        /*
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(
            "Enemy Paths",
            EditorStyles.boldLabel);

        EnemyPath[] availablePaths = GO.Instance<PresetConfig>().paths;

        string[] pathNames =
            availablePaths
            .Select(x => x.name)
            .ToArray();
        GUILayout.BeginHorizontal();
        selectedPath = EditorGUILayout.Popup(
            "Add Path",
            selectedPath,
            pathNames);

        if (GUILayout.Button("Add",GUILayout.Width(80)))
        {
            Undo.RecordObject(waveDef, "Add Path");

            waveDef.paths.Add(
                availablePaths[selectedPath]);

            EditorUtility.SetDirty(waveDef);
        }
        GUILayout.EndHorizontal();

        // VIEW

        for (int i = 0; i < waveDef.paths.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            int idx = Array.FindIndex(
                   availablePaths,
                   p => p.id == waveDef.paths[i].id);

            idx = EditorGUILayout.Popup(
                $"Path {i}",
                Mathf.Max(0, idx),
                pathNames);

            waveDef.paths[i] =
                availablePaths[idx];

            if (GUILayout.Button(
                "Remove",
                GUILayout.Width(80)))
            {
                Undo.RecordObject(
                    waveDef,
                    "Remove Path");

                waveDef.paths.RemoveAt(i);

                EditorUtility.SetDirty(waveDef);

                break;
            }

            EditorGUILayout.EndHorizontal();
        }
        */
    }

    void DrawAssetSelection()
    {
        EnemyWaveDef newWave = null;

        if (Selection.activeObject is  GameObject && ((GameObject)Selection.activeObject).GetComponent< EnemyWaveEditor>())
        {
            newWave = ((GameObject)Selection.activeObject).GetComponent<EnemyWaveEditor>().waveDef;
        }
        if (newWave!= waveDef)
            waveDef=newWave;    

        waveDef = (EnemyWaveDef)EditorGUILayout.ObjectField(
            "Enemy Wave Def",
            waveDef,
            typeof(EnemyWaveDef),
            false);

    }

    void DrawProperties()
    {
        GUILayout.Label("Properties", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        waveDef.id =
            EditorGUILayout.TextField("Id", waveDef.id);

        waveDef.desc =
            EditorGUILayout.TextField("Name", waveDef.desc);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        waveDef.grid_w =
            EditorGUILayout.IntField("Grid W", waveDef.grid_w);

        waveDef.grid_h =
            EditorGUILayout.IntField("Grid H", waveDef.grid_h);

        
        GUILayout.EndHorizontal();

        waveDef.padding =
            EditorGUILayout.FloatField("Padding", waveDef.padding);
    }

    void DrawEnemySelection()
    {
        if (waveDef == null || waveDef.enemies.Count == 0)
            return;

        GUILayout.Space(10);

        const int size = 32;
        const int spacing = 2;

        float availableWidth = EditorGUIUtility.currentViewWidth - 30;
        int itemsPerRow = Mathf.Max(1, Mathf.FloorToInt(availableWidth / (size + spacing)));

        int rows = Mathf.CeilToInt((float)waveDef.enemies.Count / itemsPerRow);

        Rect area = GUILayoutUtility.GetRect(
            availableWidth,
            rows * (size + spacing)
        );
        EditorGUI.DrawRect(area, new Color(0.2f, 0.2f, 0.2f, 1f));
        for (int i = 0; i < waveDef.enemies.Count; i++)
        {
            int col = i % itemsPerRow;
            int row = i / itemsPerRow;

            Rect rect = new Rect(
                area.x + col * (size + spacing),
                area.y + row * (size + spacing),
                size,
                size
            );

            DrawEnemyMiniature(rect, waveDef.enemies[i] ,(e)=>
            {
                selectedEnemy = waveDef.enemies.IndexOf(e);
            }, i == selectedEnemy);
        }
    }

    void DrawGrid()
    {
        const float cellSize = 36;

        scroll =
            EditorGUILayout.BeginScrollView(
                scroll);

        for (int y = waveDef.grid_h - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < waveDef.grid_w; x++)
            {
                Vector2Int pos =
                    new Vector2Int(x, y);

                var rect = DrawCell(
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
        return waveDef.data.FirstOrDefault(
            x => x.pos == pos);
    }
    Rect DrawCell(  Vector2Int pos, float size)
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
                waveDef.enemies[cell.index]);
        }

        if (GUI.Button(
            rect,
            GUIContent.none,
            GUIStyle.none))
        {
            HandleClick(pos);
        }


        if (waveDef.pivot == pos)
        {
            DrawBorder(
                contentRect,
                Color.yellow);
        }
        if (selectedShip != null && cell == selectedShip)
        {
            DrawBorder(
                contentRect,
                Color.white);
        }
        return rect;
    }

    void DrawEnemyMiniature(
    Rect rect,
    ShipDef def, Action<ShipDef> onClick=null, bool selected = false)
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
            waveDef,
            "Edit Wave");

        if (editMode == EditMode.Pivot)
        {
            waveDef.pivot = pos;
            return;
        }
        if (editMode == EditMode.Select)
        {
            var ship = GetCell(pos);
            if (ship != null) {
                selectedShip = ship;
             }
            else
                selectedShip=null;

            return;
        }
        var existing =
            GetCell(pos);

        if (existing == null)
        {
            var cell = new EnemyWaveDefCell
            {
                pos = pos,
                index = selectedEnemy
            };

            waveDef.data.Add(cell);
            selectedShip = cell;
            return;
        }

        if (existing.index == selectedEnemy)
        {
            waveDef.data.Remove(
                existing);
            selectedShip = null;
            return;
        }

        existing.index =
            selectedEnemy;

        EditorUtility.SetDirty(
            waveDef);
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