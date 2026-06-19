
using System.Linq;
using UnityEngine;

// ====

public class Enemy: MonoBehaviour
{
    public enum EnemyState
    {
        Entering,
        JoiningFormation,
        InFormation,
        Diving
    }

    public EnemyWave wave;
    public EnemyWaveDefCell cell;

    //public EnemyPath enter_path;
    //public EnemySpawnSourcePoint enter_point;

    public EnemyState state = EnemyState.Entering;

    private void OnEnable()
    {
        if (cell != null) {

            //var cfg = GO.Instance<EnemyPathConfig>();
          //  enter_path = cell.enter_path;// PathPresets.Get(cell.pathId);

          //  var list = GameObject.FindObjectsByType<EnemySpawnSourcePoint>(FindObjectsSortMode.None);

          //  enter_point = list.Where( X=> X.source ==cell.enter_source).FirstOrDefault(); 

        }
    }
  

    public void OnBeginEnter()
    {
        GetComponent<ShipBuilder>().SetVisible(false);
    }

    public void OnEnter(EnemySpawnSourcePoint startPos, EnemyPath path)
    {
   
        GetComponent<ShipBuilder>().SetVisible(true);

        transform.position = startPos.transform.position;
        transform.rotation = startPos.transform.rotation;

        gameObject.AddComponent<EnemyWavePathAnimator>().cell = cell;
        gameObject.GetComponent<EnemyWavePathAnimator>().path = path;
        gameObject.GetComponent<EnemyWavePathAnimator>().wave = wave;


    }

    //=======================================
    public void Kill()
    {
        gameObject.AddComponent<ShipExplosion>();
    }
}

