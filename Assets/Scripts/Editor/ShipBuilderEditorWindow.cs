using JetBrains.Annotations;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ShipBuilderEditorWindow : EditorWindow
{
    private ShipBuilder shipBuilder;
    private ShipDef shipDef=> shipBuilder?.shipDef;

   // private int selectedLayer;
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
        GetWindow<ShipBuilderEditorWindow>("shipDef");
    }

    private void OnGUI()
    {
        DrawAssetSelection();

        if (shipDef == null)
            return;

        Undo.RecordObject(shipDef, "shipDef Edit");

        DrawProperties();

        DrawLayers();
        DrawPalette();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();    

        editPivot = GUILayout.Toggle(editPivot,   "Edit Pivot",   "Button");
        if (GUILayout.Button("Clear"))
        {
            if (shipDef.layers.Count > 0)
            {
                Undo.RecordObject(shipDef, "Clear Layer");

                shipDef.layers[shipBuilder.currentLayer].data.Clear();

                EditorUtility.SetDirty(shipDef);

               // shipDef.OnChanged?.Invoke();
            }
        }

        GUILayout.EndHorizontal();

        DrawGrid();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(shipDef);
        }
    }

    void DrawAssetSelection()
    {
        ShipBuilder newShip= null;
        if ( Selection.activeObject is GameObject && ((GameObject)Selection.activeObject).GetComponent<ShipBuilder>())
        {
            newShip = ((GameObject)Selection.activeObject).GetComponent<ShipBuilder>();
        }

        if (newShip != shipBuilder)
        {
            shipBuilder = newShip;   
          
        }

        shipBuilder = (ShipBuilder)EditorGUILayout.ObjectField(
              "Ship",
              shipBuilder,
              typeof(ShipBuilder),
              false);
    }

    void DrawProperties()
    {
        GUILayout.Label("Properties", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        shipDef.id =
            EditorGUILayout.TextField("Id", shipDef.id);

        shipDef.enemyName =
            EditorGUILayout.TextField("Name", shipDef.enemyName);

        GUILayout.EndHorizontal ();
        GUILayout.BeginHorizontal();
        shipDef.grid_w =
            EditorGUILayout.IntField("Grid W", shipDef.grid_w);

        shipDef.grid_h =
            EditorGUILayout.IntField("Grid H", shipDef.grid_h);
        GUILayout.EndHorizontal();
        shipDef.grid_size =
            EditorGUILayout.FloatField("Grid Size", shipDef.grid_size);
    }

    void DrawLayers()
    {
        GUILayout.Space(10);

        GUILayout.Label("Layers", EditorStyles.boldLabel);

        int count =
            EditorGUILayout.IntField(
                "Layer Count",
                shipDef.layers.Count);

        while (shipDef.layers.Count < count)
            shipDef.layers.Add(new ShipDefLayer());

        while (shipDef.layers.Count > count)
            shipDef.layers.RemoveAt(shipDef.layers.Count - 1);

        if (shipDef.layers.Count == 0)
            return;

        if (shipDef.layers.Count > 0)
        {
            int selectedLayer = EditorGUILayout.IntSlider(
                "Current Layer",
                shipBuilder.currentLayer,
                0,
                shipDef.layers.Count - 1);

            EditorGUILayout.LabelField(
                $"Layer {selectedLayer}");

            if (shipBuilder.currentLayer != selectedLayer)
                shipBuilder.SetLayer(selectedLayer);
        }

        /*
        selectedLayer =
            EditorGUILayout.Popup(
                "Current Layer",
                selectedLayer,
                Enumerable.Range(
                    0,
                    shipDef.layers.Count)
                .Select(x => $"Layer {x}")
                .ToArray());

        selectedLayer =
            Mathf.Clamp(
                selectedLayer,
                0,
                shipDef.layers.Count - 1);
        */
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
        for (int i = 0; i < shipDef.layers.Count; i++)
            DrawGrid(i);


    }

    void DrawGrid(int layerIdx)
    {
        ShipDefLayer layer  = shipDef.layers[layerIdx];

        GUILayout.BeginHorizontal();
        GUILayout.Label("Layer "+ layerIdx, EditorStyles.boldLabel);

        GUILayout.Label("Anim time");
        layer.animTime = EditorGUILayout.FloatField(
                layer.animTime,
                GUILayout.Width(60));

        GUILayout.EndHorizontal();

        for (int y = shipDef.grid_h - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < shipDef.grid_w; x++)
            {
                Vector2Int pos =
                    new Vector2Int(x, y);

                bool isPivot =
                    shipDef.pivot.x == x &&
                    shipDef.pivot.y == y;

                ShipDefCell cell =
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
                        shipDef.pivot = pos;
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
        ShipDefLayer layer,
        Vector2Int pos)
    {
        ShipDefCell cell =
            layer.data.FirstOrDefault(
                c => c.pos == pos);

        if (cell == null)
        {
            layer.data.Add(
                new ShipDefCell
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