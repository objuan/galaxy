using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct GravitySimulationJob : IJobParallelFor
{
    public float3 speedOfLight;

    [ReadOnly]
    public NativeArray<GravityBodyData> inputBodies;

    public NativeArray<GravityBodyData> outputBodies;

    public float deltaTime;

    public float gravitationalConstant;

    public void Execute(int index)
    {
        GravityBodyData body =
            inputBodies[index];

        //---------------------------------
        // DIRECTION TO CENTER OF MASS
        //---------------------------------

        float3 dir =
            body.centerOfMass -
            body.position;

        dir.y = 0;

        float distSqr =
            math.lengthsq(dir) + 0.01f;

        float dist =
            math.sqrt(distSqr);

        //---------------------------------
        // GRAVITY ONLY TOWARD CENTER
        //---------------------------------

        float gravity =
            gravitationalConstant /
            distSqr;

        float3 acceleration =
            math.normalize(dir) *
            gravity;

        //---------------------------------
        // OPTIONAL ORBIT STABILIZATION
        //---------------------------------

        float3 tangent =
            new float3(-dir.z, 0, dir.x);

        tangent =
            math.normalize(tangent);

        // Keeps bodies orbiting instead of falling inward
        float orbitalSpeed =
            math.sqrt(gravitationalConstant / dist);

        body.velocity =
             tangent * orbitalSpeed * deltaTime * 0.1f;

        var newPos = body.position + body.velocity * deltaTime;
        dir = newPos - body.centerOfMass;


        body.position = body.centerOfMass + math.normalize(dir) * dist;


        /*
        body.velocity +=
            tangent * orbitalSpeed * deltaTime * 0.1f;

        //---------------------------------
        // UPDATE VELOCITY
        //---------------------------------

        body.velocity +=
            acceleration * deltaTime;

        //---------------------------------
        // UPDATE POSITION
        //---------------------------------
        var newPos = body.position + body.velocity * deltaTime;
        dir = newPos - body.centerOfMass;


        body.position = body.centerOfMass + math.normalize(dir) * dist;
        //body.position +=  body.velocity * deltaTime;

        body.position.y = 0;
        */
        //---------------------------------
        // WRITE RESULT
        //---------------------------------

        outputBodies[index] = body;
    }
}