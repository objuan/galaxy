using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyEditor))]
public class EnemyEditorScene : Editor
{
    private EnemyEditor editor;

    private void OnSceneGUI()
    {
        editor = (EnemyEditor)target;

        if (editor.enemyDef == null)
            return;

        DrawGrid();
    }

    void DrawGrid()
    {
        var def = editor.enemyDef;
        Vector3 offset = -new Vector3(def.grid_size * def.grid_w / 2,0, def.grid_size * def.grid_h / 2);      

        Handles.color = Color.gray;

        for (int x = 0; x <= def.grid_w; x++)
        {
            Vector3 p1 = offset+new Vector3(x * def.grid_size, 0, 0);
            Vector3 p2 = offset + new Vector3(x * def.grid_size, 0, def.grid_h * def.grid_size);

            Handles.DrawLine(p1, p2);
        }

        for (int z = 0; z <= def.grid_h; z++)
        {
            Vector3 p1 = offset + new Vector3(0, 0, z * def.grid_size);
            Vector3 p2 = offset + new Vector3(def.grid_w * def.grid_size, 0, z * def.grid_size);

            Handles.DrawLine(p1, p2);
        }
    }


}