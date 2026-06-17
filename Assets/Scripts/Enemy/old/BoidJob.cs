using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public struct BoidJob : IJobParallelFor
{
    // Dati in lettura

    [ReadOnly] public NativeArray<Vector3> positions;
    [ReadOnly] public NativeArray<Vector3> velocities;

    [ReadOnly] public JobParams pars;
    // Dati in uscita
    public NativeArray<Vector3> accelerations;

    public void Execute(int index)
    {
        throw new NotImplementedException();
    }

    /*
    public Vector3 Split(int index,Vector3 pos, Vector3 swarmForce)
    {
        Vector3 toPlayer = pars.playerPosition - pos;
        float dist = toPlayer.magnitude;

        Vector3 forward = toPlayer.normalized;
        Vector3 side = Vector3.Cross(Vector3.up, forward).normalized;

        // scegli lato basato sull’indice (o random precomputato)
        float sideSign = (index % 2 == 0) ? 1f : -1f;

        float splitDistance = 6f;

        Vector3 splitForce;

        if (dist > splitDistance)
        {
            // fase 1: attacco diretto
            splitForce = forward * 2f;
        }
        else
        {
            // fase 2: separazione laterale
            splitForce = (forward * 0.5f) + (side * sideSign * 3f);
        }
        var acc = (swarmForce + splitForce);
        return acc;
    }

    public Vector3 OrbitaStringe(Vector3 pos, Vector3 swarmForce)
    {
        Vector3 toPlayer = playerPosition - pos;
        float dist = toPlayer.magnitude;

        Vector3 radialDir = toPlayer.normalized;
        Vector3 tangentDir = Vector3.Cross(Vector3.up, radialDir).normalized;

        // raggio target dinamico (si restringe nel tempo)
        float targetRadius = Mathf.Lerp(8f, 2f, deltaTime);// * 0.05f);

        
        // forza per mantenere il raggio
        float radiusError = dist - targetRadius;
        Vector3 radiusForce = radialDir * radiusError;

        // movimento orbitale
        float orbitSpeed = 3.0f;
        Vector3 orbitForce = tangentDir * orbitSpeed;

        Vector3 orbitBehavior = orbitForce - radiusForce;
        var acc = (swarmForce + orbitBehavior);
        return acc;
    }

    public Vector3 SpiraleToPlayer(Vector3 pos, Vector3 swarmForce)
    {
        Vector3 toPlayer = playerPosition - pos;
        Vector3 radial = toPlayer.normalized;

        // direzione tangenziale (perpendicolare sul piano XZ)
        Vector3 tangent = Vector3.Cross(Vector3.up, radial).normalized;

        // spirale = mix tra entrare e girare
        float spiralStrength = 2.0f;
        float inwardStrength = 1.5f;

        // più sei lontano → più spirale larga
        float dist = toPlayer.magnitude;
        float spiralFactor = Mathf.Clamp01(dist / 10f);

        Vector3 spiralForce = (radial * inwardStrength) + (tangent * spiralStrength * spiralFactor);

        var acc = (swarmForce + spiralForce);
        return acc;
    }

    public void Execute(int index)
    {
        Vector3 pos = positions[index];
        Vector3 vel = velocities[index];

        Vector3 separation = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;
        int neighborCount = 0;

        for (int i = 0; i < positions.Length; i++)
        {
            if (i == index) continue;

            float dist = Vector3.Distance(pos, positions[i]);
            if (dist < neighborRadius)
            {
                neighborCount++;
                alignment += velocities[i];
                cohesion += positions[i];

                if (dist < separationDistance)
                {
                    separation += (pos - positions[i]) / dist;
                }
            }
        }

        Vector3 swarmForce = Vector3.zero;
        if (neighborCount > 0)
        {
            alignment /= neighborCount;
            cohesion = (cohesion / neighborCount) - pos;

            float cohesionWeight = 0.5f;   // Aumenta per uno sciame più "stretto"
            float alignmentWeight = 1.0f;  // Aumenta per un movimento più parallelo
            float separationWeight = 3.0f; // Aumenta per evitare sovrapposizioni

            // Calcolo finale della forza dello sciame
            swarmForce = (separation * separationWeight) +
                                 (alignment * alignmentWeight) +
                                 (cohesion * cohesionWeight);
            //force = (separation * 2.5f) + (alignment * 1f) + (cohesion * 1f);
        }

        // Accerchiamento: punta a un raggio di 5 metri dal giocatore
        Vector3 toPlayer = playerPosition - pos;
        float distToPlayer = toPlayer.magnitude;
        Vector3 chaseForce = (distToPlayer > 5f) ? toPlayer.normalized : -toPlayer.normalized * 0.5f;

        Debug.Log(index+ " " +distToPlayer+" " + chaseForce);

        //var acc =   chaseForce * 20f;


        // All'interno del BoidJob


        var acc = SpiraleToPlayer(pos, swarmForce);
        // var acc = Split(index,pos, swarmForce);
        //var acc = swarmForce; // OrbitaStringe(pos, swarmForce, 1f);


        acc.y = 0;
        accelerations[index] = acc;

      

      
    }
    */
}
