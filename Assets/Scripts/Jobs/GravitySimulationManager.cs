using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities.UniversalDelegates;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;



public class GravitySimulationManager : MonoBehaviour
{
    public static GravitySimulationManager Instance;

    public float gravitationalConstant = 0.001f;

    public float speedOfLight = 100f;

    public bool useBurst = true;

    List<CelestialBody1> registeredBodies =
        new List<CelestialBody1>();

    NativeArray<GravityBodyData> inputBodies;

    NativeArray<GravityBodyData> outputBodies;

    void Awake()
    {
        Instance = this;
    }

    public void Register(CelestialBody1 body)
    {
        if (!registeredBodies.Contains(body))
        {
            registeredBodies.Add(body);
        }
    }

    public void Unregister(CelestialBody1 body)
    {
        registeredBodies.Remove(body);

       // inputBodies.Dispose();

      //  outputBodies.Dispose();
    }

    void FixedUpdate()
    {
        int count = registeredBodies.Count;

        if (count == 0)
            return;

        inputBodies =
             new NativeArray<GravityBodyData>(
          count,
          Allocator.TempJob);

        outputBodies =
            new NativeArray<GravityBodyData>(
                count,
                Allocator.TempJob);

        //---------------------------------
        // COPY TO NATIVE ARRAY
        //---------------------------------

        for (int i = 0; i < count; i++)
        {
            CelestialBody1 body = registeredBodies[i];

            inputBodies[i] = new GravityBodyData
            {
                centerOfMass = body.parent!=null ? body.parent.position: new float3(0,0,0),
                mass = body.mass,
                radius = body.radius,
                position = body.position,
                velocity = body.velocity,
                isBlackHole = body.isBlackHole ? 1 : 0
            };
        }

        //---------------------------------
        // SCHEDULE JOB
        //---------------------------------

        GravitySimulationJob job =
            new GravitySimulationJob
            {
                inputBodies = inputBodies,
                outputBodies = outputBodies,
                deltaTime = Time.fixedDeltaTime,
                gravitationalConstant =
                    gravitationalConstant,
                //speedOfLight = speedOfLight
            };

        JobHandle handle =
            job.Schedule(count, 64);

     

            handle.Complete();

        for (int i = 0; i < count; i++)
        {
            registeredBodies[i].position = outputBodies[i].position;
            registeredBodies[i].velocity = outputBodies[i].velocity;
            // GravityBodyData data =
            //       outputBodies[i];
        }

        //---------------------------------
    }
}