using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;


// ==================================================================

public class SwarmManager : MonoBehaviour
{
    public int entityCount = 100;
    public GameObject enemyPrefab;
    public float Y=5;
    public Transform player;

    private Transform[] transforms;
    private NativeArray<Vector3> positions;
    private NativeArray<Vector3> velocities;
    private NativeArray<Vector3> accelerations;

    private NativeArray<EnemyData> enemyData;

    private List<EnemyLogic> enemyList = new List<EnemyLogic>();

    void Start()
    {
        /*
        transforms = new Transform[entityCount];
        positions = new NativeArray<Vector3>(entityCount, Allocator.Persistent);
        velocities = new NativeArray<Vector3>(entityCount, Allocator.Persistent);
        accelerations = new NativeArray<Vector3>(entityCount, Allocator.Persistent);

        for (int i = 0; i < entityCount; i++)
        {
            GameObject go = Instantiate(enemyPrefab, Random.insideUnitSphere * 10, Quaternion.identity);
            go.transform.position = new Vector3(go.transform.position.x, Y, go.transform.position.z);
            transforms[i] = go.transform;
            // Varietà estetica: scala casuale
            transforms[i].localScale = Vector3.one;// * Random.Range(0.8f, 1.2f);
        }
        */

        //var logic = new EnemyLogic_Progressive()
    }

    public void Spawn(EnemyLogic logic)
    {
        enemyList.Add(logic);
        //logic.Init();
    }

    void Update()
    {
        /*
        // Aggiorna posizioni correnti per il Job
        for (int i = 0; i < entityCount; i++) positions[i] = transforms[i].position;

        JobParams pars = new JobParams
        {
            neighborRadius = 3.0f,
            separationDistance = 1.5f,
            playerPosition = player.position,
            deltaTime = Time.deltaTime,
            time =Time.time
        };
        BoidJob job = new BoidJob
        {
            positions = positions,
            velocities = velocities,
            accelerations = accelerations,
            pars= pars
        };

        JobHandle handle = job.Schedule(entityCount, 64);
        handle.Complete();

        // Applica i risultati
        for (int i = 0; i < entityCount; i++)
        {
            velocities[i] += accelerations[i] * Time.deltaTime;
            velocities[i] = Vector3.ClampMagnitude(velocities[i], 5f); // Limite velocità
            transforms[i].position += velocities[i] * Time.deltaTime;
            if (velocities[i] != Vector3.zero)
                transforms[i].rotation = Quaternion.LookRotation(velocities[i]);
        }
        */
    }

    void OnDestroy()
    {
        if (positions.IsCreated) positions.Dispose();
        if (velocities.IsCreated) velocities.Dispose();
        if (accelerations.IsCreated) accelerations.Dispose();
    }
}