
using System.Linq;
using UnityEngine;

// ====

public class Enemy: MonoBehaviour
{
    public EnemyWave wave;
    public EnemyWaveDefCell cell;

    public EnemyPath enter_path;
    public EnemySpawnSourcePoint enter_point;

    private void OnEnable()
    {
        if (cell != null) {

            //var cfg = GO.Instance<EnemyPathConfig>();
            enter_path = cell.enter_path;// PathPresets.Get(cell.pathId);

            var list = GameObject.FindObjectsByType<EnemySpawnSourcePoint>(FindObjectsSortMode.None);

            enter_point = list.Where( X=> X.source ==cell.enter_source).FirstOrDefault(); 

        }
    }
    /*
    public EnemyCommand cmd;
    float cmdTime;

    public EnemyCommandStack commandStack;

    public float speed;

    private void Start()
    {
        cmd = commandStack.FirstCommand;
    }

    private void Update()
    {
        
        if (cmd == null)
            return;

        float targetDistance = 9999999;

        if (cmd.mode == EnemyCommandMode.SPEED)
        {
            speed = cmd.arg_speed;
            cmd = commandStack.NextCommand(cmd);
            cmdTime = Time.time;
        }
        else if(cmd.mode == EnemyCommandMode.GOTO)
        {
            var dir = (cmd.target.transform.position - transform.position);
            targetDistance = dir.magnitude;
            var newPos = transform.position  + dir.normalized * speed * Time.deltaTime;
            transform.position = newPos;

        }
        else if(cmd.mode == EnemyCommandMode.SPIRAL)
        {
            var acc  = SpiraleToPlayer(cmd.target.transform.position, transform.position,speed,  cmd.radial_speed);
            transform.position += acc *  Time.deltaTime;

            var dir = (cmd.target.transform.position - transform.position);
            targetDistance = dir.magnitude;
        }


        if (cmd.condType == CommandCondictionType.Distance && Mathf.Abs(targetDistance - cmd.condValue) < 0.5f)
        {
            cmd = commandStack.NextCommand(cmd);
            cmdTime = Time.time;
        }
        else if (cmd.condType == CommandCondictionType.Time && Time.time- cmdTime > cmd.condValue)
        {
            cmd = commandStack.NextCommand(cmd);
            cmdTime = Time.time;
        }
    }

    public Vector3 SpiraleToPlayer(Vector3 targetPosition,Vector3 pos,float speed,  float radial_speed)
    {
        Vector3 toPlayer = targetPosition - pos;
        Vector3 radial = toPlayer.normalized;

        // direzione tangenziale (perpendicolare sul piano XZ)
        Vector3 tangent = Vector3.Cross(Vector3.up, radial).normalized;

        // spirale = mix tra entrare e girare
        //float spiralStrength = 2.0f;
        //float inwardStrength = 0.0f;

        // più sei lontano → più spirale larga
        float dist = toPlayer.magnitude;
        float spiralFactor = Mathf.Clamp01(dist / 10f);

        Vector3 spiralForce = (radial * radial_speed) + (tangent * speed);

        var acc = ( spiralForce);
        return acc;
    }
    */

    //=======================================
    public void Kill()
    {
        gameObject.AddComponent<ShipExplosion>();
    }
}

