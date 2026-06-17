using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyWaveEditor))]
public class EnemyWaveEditorScene : Editor
{
    private EnemyWaveEditor editor;

    private void OnSceneGUI()
    {
        editor = (EnemyWaveEditor)target;

        if (editor.waveDef == null)
            return;

        DrawGrid();
    }

    void DrawGrid()
    {
        var def = editor.waveDef;
        float grid_size = 1;
        Vector3 offset = -new Vector3(grid_size * def.grid_w / 2,0, grid_size * def.grid_h / 2);      

        Handles.color = Color.gray;

        for (int x = 0; x <= def.grid_w; x++)
        {
            Vector3 p1 = offset+new Vector3(x * grid_size, 0, 0);
            Vector3 p2 = offset + new Vector3(x * grid_size, 0, def.grid_h * grid_size);

            Handles.DrawLine(p1, p2);
        }

        for (int z = 0; z <= def.grid_h; z++)
        {
            Vector3 p1 = offset + new Vector3(0, 0, z * grid_size);
            Vector3 p2 = offset + new Vector3(def.grid_w * grid_size, 0, z * grid_size);

            Handles.DrawLine(p1, p2);
        }
    }


}