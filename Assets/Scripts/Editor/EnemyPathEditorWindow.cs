using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static GalagaPaths;

public class EnemyPathEditorWindow : EditorWindow
{
    EnemyPath path;

    Vector2 pan;
    float zoom = 1f;

    bool draggingPoint;
    int selectedPoint = -1;

    bool previewPlaying;
    double previewStartTime;

    const float GridSize = 32f;

    FormationPath selectedPath;

    [MenuItem("Tools/Galaga Path Editor")]
    static void Open()
    {
        GetWindow<EnemyPathEditorWindow>("Path Editor");
    }

    void OnGUI()
    {
        DrawToolbar();

        if (path == null)
            return;

        Rect canvas = GUILayoutUtility.GetRect(
            position.width,
            position.height,
            GUILayout.ExpandWidth(true),
            GUILayout.ExpandHeight(true));

        HandleInput(canvas);

        DrawGrid(canvas);
        DrawSpline(canvas);
        DrawPoints(canvas);
        DrawPreview(canvas);

        if (previewPlaying)
            Repaint();
    }

    #region Toolbar

    void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        path = (EnemyPath)EditorGUILayout.ObjectField(
            path,
            typeof(EnemyPath),
            false,
            GUILayout.Width(250));

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        EditorGUILayout.LabelField(
    "Preset",
    GUILayout.Width(40));

        var newPath =
            (FormationPath)EditorGUILayout.EnumPopup(
                selectedPath,
                GUILayout.Width(150));

        if (newPath != selectedPath)
        {
            selectedPath = newPath;

            if (path != null)
            {
                Undo.RecordObject(path, "Load Preset");

                path.points = new List<Vector2>(
                    PathPresets.Get(selectedPath));

                EditorUtility.SetDirty(path);
            }
        }

        path.lastFixedPoint= EditorGUILayout.IntField("Last Fixed Idx", path.lastFixedPoint);

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar); 

        GUILayout.Space(10);

        if (GUILayout.Button("Add Point", EditorStyles.toolbarButton))
        {
            if (path != null)
            {
                Undo.RecordObject(path, "Add Point");

                path.points.Add(Vector2.zero);

                EditorUtility.SetDirty(path);
            }
        }

        if (GUILayout.Button("Remove Last", EditorStyles.toolbarButton))
        {
            if (path != null && path.points.Count > 0)
            {
                Undo.RecordObject(path, "Remove Point");

                path.points.RemoveAt(path.points.Count - 1);

                EditorUtility.SetDirty(path);
            }
        }

        GUILayout.Space(20);

        if (GUILayout.Button("Play", EditorStyles.toolbarButton))
        {
            previewPlaying = true;
            previewStartTime = EditorApplication.timeSinceStartup;
        }

        if (GUILayout.Button("Stop", EditorStyles.toolbarButton))
        {
            previewPlaying = false;
        }

        GUILayout.FlexibleSpace();

        EditorGUILayout.EndHorizontal();
    }

    #endregion

    #region Coordinate Conversion

    Vector2 WorldToScreen(Vector2 world, Rect canvas)
    {
        Vector2 center = canvas.center;

        return center + pan + world * zoom;
    }

    Vector2 ScreenToWorld(Vector2 screen, Rect canvas)
    {
        Vector2 center = canvas.center;

        return (screen - center - pan) / zoom;
    }

    #endregion

    #region Grid

    void DrawGrid(Rect canvas)
    {
        Handles.BeginGUI();

        Color old = Handles.color;
        Handles.color = new Color(0.25f, 0.25f, 0.25f);

        float step = GridSize * zoom;

        Vector2 offset = new Vector2(
            pan.x % step,
            pan.y % step);

        for (float x = offset.x; x < canvas.width; x += step)
        {
            Handles.DrawLine(
                new Vector2(x, 0),
                new Vector2(x, canvas.height));
        }

        for (float y = offset.y; y < canvas.height; y += step)
        {
            Handles.DrawLine(
                new Vector2(0, y),
                new Vector2(canvas.width, y));
        }

        Handles.color = old;

        Handles.EndGUI();
    }

    #endregion

    #region Points

    void DrawPoints(Rect canvas)
    {
        for (int i = 0; i < path.points.Count; i++)
        {
            Vector2 pos =
                WorldToScreen(path.points[i], canvas);

            Rect r = new Rect(
                pos.x - 6,
                pos.y - 6,
                12,
                12);

            EditorGUI.DrawRect(
                r,
                i == selectedPoint
                    ? Color.yellow
                    : Color.cyan);

            GUI.Label(
                new Rect(pos.x + 8, pos.y - 10, 50, 20),
                $"P{i}");
        }
    }

    #endregion

    #region Input

    void HandleInput(Rect canvas)
    {
        Event e = Event.current;

        if (e.type == EventType.ScrollWheel)
        {
            float delta = -e.delta.y * 0.05f;

            zoom = Mathf.Clamp(
                zoom + delta,
                0.2f,
                8f);

            e.Use();
        }

        if (e.button == 2)
        {
            if (e.type == EventType.MouseDrag)
            {
                pan += e.delta;
                Repaint();
            }
        }

        if (e.type == EventType.MouseDown &&
            e.button == 0)
        {
            selectedPoint = -1;

            for (int i = 0; i < path.points.Count; i++)
            {
                Vector2 screen =
                    WorldToScreen(path.points[i], canvas);

                if (Vector2.Distance(
                    screen,
                    e.mousePosition) < 10f)
                {
                    selectedPoint = i;
                    draggingPoint = true;
                    break;
                }
            }
        }

        if (e.type == EventType.MouseUp)
        {
            draggingPoint = false;
        }

        if (draggingPoint &&
            selectedPoint >= 0 &&
            e.type == EventType.MouseDrag)
        {
            Undo.RecordObject(path, "Move Point");

            path.points[selectedPoint] =
                ScreenToWorld(
                    e.mousePosition,
                    canvas);

            EditorUtility.SetDirty(path);

            Repaint();
        }
    }

    #endregion

    #region Spline

    void DrawSpline(Rect canvas)
    {
        if (path.points.Count < 4)
            return;

        Handles.BeginGUI();

        Handles.color = Color.green;

        Vector2 prev =
            WorldToScreen(path.points[1], canvas);

        const int Resolution = 20;

        for (int seg = 0;
             seg < path.points.Count - 3;
             seg++)
        {
            for (int i = 1; i <= Resolution; i++)
            {
                float t =
                    i / (float)Resolution;

                Vector2 p =
                    CatmullRom(
                        path.points[seg],
                        path.points[seg + 1],
                        path.points[seg + 2],
                        path.points[seg + 3],
                        t);

                Vector2 current =
                    WorldToScreen(p, canvas);

                Handles.DrawLine(prev, current);

                prev = current;
            }
        }

        Handles.EndGUI();
    }

    #endregion

    #region Preview

    void DrawPreview(Rect canvas)
    {
        if (!previewPlaying)
            return;

        if (path.points.Count < 4)
            return;

        float duration = 5f;

        float t =
            (float)(
                EditorApplication.timeSinceStartup -
                previewStartTime);

        t = Mathf.Repeat(t / duration, 1f);

        Vector2 pos =
            path.GetPath().EvaluatePath(t);

        Vector2 screen =
            WorldToScreen(pos, canvas);

        Rect r = new Rect(
            screen.x - 5,
            screen.y - 5,
            10,
            10);

        EditorGUI.DrawRect(r, Color.red);
    }

 

    #endregion

 
}