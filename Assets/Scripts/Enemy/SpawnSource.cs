#nullable enable

using UnityEngine;


public class StartPose
{
    public Vector3 position;
    public Vector3 direction;
}

// ====
public class SpawnSource: MonoBehaviour
{
    protected float runTime = 0;

    public void Begin()
    {
        runTime  =Time.time;
    }
    public virtual StartPose? Next()
    {
        return null;
    }
}
