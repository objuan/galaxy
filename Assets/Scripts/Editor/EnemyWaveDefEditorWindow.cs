using System;
using System.Collections.Generic;
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

    // private int selectedPath;

    private HashSet<EnemyWaveDefCell> selectedShips = new();
    private EnemyWaveDefCell lastSelectedShip = null;

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

        DrawGroupEdit();

        DrawPathEdit();
        

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

        //  DrawCellSelection();

        GUILayout.BeginHorizontal();

        GUILayout.Label(
            $"Selected: {selectedShips.Count}",
            EditorStyles.boldLabel);

        if (selectedShips.Count > 0)
        {
            GUILayout.Label(
                string.Join(
                    ", ",
                    selectedShips.Select(
                        s => $"[{s.pos.x},{s.pos.y}]")));
        }

        GUILayout.EndHorizontal();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(waveDef);
        }
    }

    void DrawGroupEdit()
    {
        // ENTER GROUPS

        EditorGUILayout.Space();
        EditorGUILayout.LabelField(
            "Enemy Groups",
            EditorStyles.boldLabel);

        for (int i = 0; i < waveDef.groups.Count; i++)
        {
            var entry = waveDef.groups[i];

            GUILayout.BeginHorizontal();

            GUILayout.Label(
                $"#{i}",
                GUILayout.Width(25));

            entry.name = GUILayout.TextField(entry.name, GUILayout.Width(80));

         
            entry.color = EditorGUILayout.ColorField(
                GUIContent.none,
                entry.color,
                false, // showEyedropper
                true,  // showAlpha
                false, // hdr
                GUILayout.Width(60));

            if (GUILayout.Button(
               "Assign Current",
               GUILayout.Width(150)))
            {
                waveDef.Assign(selectedShips.ToArray(), entry);

                EditorUtility.SetDirty(waveDef);

                //break;
            }

            // REMOVE
            if (GUILayout.Button(
                "Remove",
                GUILayout.Width(80)))
            {
                bool remove =
                EditorUtility.DisplayDialog(
                    "Remove Group",
                    $"Remove group #{i} ?",
                    "Remove",
                    "Cancel");

                        if (remove)
                        {
                    Undo.RecordObject(
                        waveDef,
                        "Remove Group");

                    waveDef.groups.RemoveAt(i);

                    EditorUtility.SetDirty(waveDef);

                    break;
                }
                break;
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.Space(5);

        if (GUILayout.Button("Add Group"))
        {
            Undo.RecordObject(
                waveDef,
                "Add Group");

            waveDef.groups.Add(
                new EnemyWaveGroup());

            EditorUtility.SetDirty(waveDef);
        }
    }

    void DrawPathEdit()
    {
        // ENTER PATHS

        EditorGUILayout.Space();
        EditorGUILayout.LabelField(
            "Enter Paths",
            EditorStyles.boldLabel);

        EnemyPath[] availablePaths =
            GO.Instance<PresetConfig>().paths;

        EnemySpawnSource[] availableSources =
            waveDef.sources.ToArray();

        string[] pathNames = availablePaths
            .Select(x => x.name)
            .Prepend("<NULL>")
            .ToArray();

        string[] sourceNames = availableSources
            .Select(x => x.name)
            .Prepend("<NULL>")
            .ToArray();

        string[] groupNames = waveDef.groups
            .Select(g => g.name)
            .Prepend("<ALL>")
            .ToArray();

        for (int i = 0; i < waveDef.enter_paths.Count; i++)
        {
            var entry = waveDef.enter_paths[i];

            GUILayout.BeginHorizontal();

            // PATH
            int pathIdx =
                Array.FindIndex(
                    availablePaths,
                    p => p == entry.path);

            pathIdx++;

            GUILayout.Label(
                $"#{i}",
                GUILayout.Width(25));

            pathIdx = EditorGUILayout.Popup(
                pathIdx,
                pathNames,
                GUILayout.Width(100));

            EnemyPath newPath = null;

            if (pathIdx > 0)
                newPath = availablePaths[pathIdx - 1];

            if (newPath != entry.path)
            {
                Undo.RecordObject(
                    waveDef,
                    "Change Enter Path");

                entry.path = newPath;

                EditorUtility.SetDirty(waveDef);
            }

            // SOURCE
            int sourceIdx =
                Array.FindIndex(
                    availableSources,
                    s => s == entry.spawnSource);

            sourceIdx++;

            sourceIdx = EditorGUILayout.Popup(
                sourceIdx,
                sourceNames,
                GUILayout.Width(150));

            EnemySpawnSource newSource = null;

            if (sourceIdx > 0)
                newSource = availableSources[sourceIdx - 1];

            if (newSource != entry.spawnSource)
            {
                Undo.RecordObject(
                    waveDef,
                    "Change Spawn Source");

                entry.spawnSource = newSource;

                EditorUtility.SetDirty(waveDef);
            }
         

            // GROUP

            int groupIdx = entry.group_idx + 1;

            Rect popupRect = GUILayoutUtility.GetRect(
                40,
                EditorGUIUtility.singleLineHeight,
                GUILayout.Width(40));

            groupIdx = EditorGUI.Popup(
                popupRect,
                groupIdx,
                groupNames);

            entry.group_idx = groupIdx - 1;

            // disegno colore gruppo
            if (entry.group_idx >= 0 &&
                entry.group_idx < waveDef.groups.Count)
            {
                Rect colorRect = new Rect(
                    popupRect.x + 2,
                    popupRect.y + 2,
                    14,
                    popupRect.height - 4);

                EditorGUI.DrawRect(
                    colorRect,
                    waveDef.groups[entry.group_idx].color);
            }


            // DELAY
            float newDelay = EditorGUILayout.FloatField(
                entry.spawnDelay,
                GUILayout.Width(40));

            if (newDelay != entry.spawnDelay)
            {
                Undo.RecordObject(
                    waveDef,
                    "Change Spawn Delay");

                entry.spawnDelay = newDelay;

                EditorUtility.SetDirty(waveDef);
            }

            // REMOVE
            if (GUILayout.Button(
                "Remove",
                GUILayout.Width(80)))
            {
            
                    bool remove =
                    EditorUtility.DisplayDialog(
                        "Remove Path",
                        $"Remove Path #{i} ?",
                        "Remove",
                        "Cancel");
                if (remove)
                {
                    Undo.RecordObject(
                    waveDef,
                    "Remove Enter Path");

                    waveDef.enter_paths.RemoveAt(i);

                    EditorUtility.SetDirty(waveDef);

                    break;
                }
                
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.Space(5);

        if (GUILayout.Button("Add Enter Path"))
        {
            Undo.RecordObject(
                waveDef,
                "Add Enter Path");

            waveDef.enter_paths.Add(
                new EnemyWavePath_Enter());

            EditorUtility.SetDirty(waveDef);
        }
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

            if (cell.group_idx != -1)
            {
                Color bgColor = waveDef.groups[cell.group_idx].color;

                Rect bgRect = new Rect(
                    contentRect.x + 1,
                    contentRect.y + 1,
                    16,
                    12);

                EditorGUI.DrawRect(
                    bgRect,
                    bgColor);
                
                GUI.Label(
                    bgRect,
                    $"{cell.group_idx}",
                    EditorStyles.whiteMiniLabel);
                
            }
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
        if (cell != null && selectedShips.Contains(cell))
        {
            DrawBorder(
                contentRect,
                Color.white,
                2);
        }
        return rect;
    }

    Rect? DrawEnemyMiniature(
    Rect rect,
    ShipDef def, Action<ShipDef> onClick=null, bool selected = false)
    {
        if (def.layers.Count == 0)
            return null;

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
        return rect;
    }
    void HandleClick( Vector2Int pos)
    {
        Undo.RecordObject(
            waveDef,
            "Edit Wave");

        if (editMode == EditMode.Pivot)
        {
            waveDef.pivot = pos;
            return;
        }
        if(editMode == EditMode.Select)
{
            var ship = GetCell(pos);

            if (ship == null)
            {
                if (!Event.current.shift)
                    selectedShips.Clear();

                return;
            }

            // SHIFT = aggiungi/togli dalla selezione
            if (Event.current.shift)
            {
                if (selectedShips.Contains(ship))
                    selectedShips.Remove(ship);
                else
                    selectedShips.Add(ship);
            }
            else
            {
                selectedShips.Clear();
                selectedShips.Add(ship);
            }

            lastSelectedShip = ship;
            return;
        }
        var existing =     GetCell(pos);

        if (existing == null)
        {
            var cell = new EnemyWaveDefCell
            {
                pos = pos,
                index = selectedEnemy
            };

            waveDef.data.Add(cell);
            selectedShips.Clear();
            selectedShips.Add(cell);
            lastSelectedShip = cell;
            return;
        }

        if (existing.index == selectedEnemy)
        {
            waveDef.data.Remove(
                existing);
            selectedShips.Clear();
            lastSelectedShip = null;
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