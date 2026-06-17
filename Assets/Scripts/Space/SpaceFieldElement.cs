using JetBrains.Annotations;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor.SpeedTree.Importer;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;

//Velocity Verlet
public class SpaceFieldElement : MonoBehaviour
{
    public float mass = 1f;
    public float softening = 0.5f;

    public float maxSpeed = 20f;

    public Vector2 velocity;
    private Vector2 acceleration;

    SpaceField sf;


    private Vector2 fieldAcc;
    private Vector2 engAcc;

    private void Awake()
    {
        sf = GO.Instance<SpaceField>();
    }
    private void Start()
    {
        acceleration = ComputeAcceleration(
                  (Vector2)transform.position);
    }

    protected virtual void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        Vector2 position =  new Vector2(transform.position.x, transform.position.z);

        Vector2 currentAcceleration =
            ComputeTotalAcceleration(position);

        Vector2 newPosition =
            position +
            velocity * dt +
            0.5f * currentAcceleration * dt * dt;

        Vector2 newAcceleration =
            ComputeTotalAcceleration(newPosition);

        velocity +=
            0.5f *
            (currentAcceleration + newAcceleration) *
            dt;

        // reset

        float speed = Mathf.Min(maxSpeed * Mathf.Max(1,fieldAcc.magnitude), velocity.magnitude);

        velocity = velocity.normalized * speed;

        //#if (engAcc.magnitude>0.01)
        //velocity =   engAcc.normalized * speed;

        transform.position = new Vector3(newPosition.x, sf.planeY, newPosition.y);
    }
    Vector2 ComputeTotalAcceleration(Vector2 position)
    {
        engAcc = ComputeEngineAcceleration();
        fieldAcc = ComputeAcceleration(position);
        return fieldAcc
             + engAcc;
    }

    Vector2 ComputeAcceleration(Vector2 shipPosition)
    {
        Vector2 totalAcceleration = Vector2.zero;

        foreach (Void v in sf.Voids)
        {
            Vector2 offset =
                new Vector2(v.transform.position.x, v.transform.position.z) -
                shipPosition;

            float distance =
                Mathf.Max(offset.magnitude, 0.5f);

            if (distance > v.eventHorizon)
                continue;
            if (distance < v.attractionRadius)
                continue;
            /*
            float accel =
                sf.G *
                v.mass /
                (distance * distance);

            totalAcceleration +=
                offset.normalized * accel;
            */


            float r2 = offset.sqrMagnitude + softening * softening;

            float invR = 1.0f / Mathf.Sqrt(r2);

            float invR3 = invR * invR * invR;

            totalAcceleration +=
                offset * (sf.G * v.mass * invR3);
            
        }

        return totalAcceleration;
    }

    protected virtual Vector2 ComputeEngineAcceleration()
    {
        return Vector2.zero;
    }

    private void OnDrawGizmos()
    {
        Vector3 dir = new Vector3(fieldAcc.x,0, fieldAcc.y);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + dir);

        dir = new Vector3(engAcc.x, 0, engAcc.y);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + dir);
    }
}