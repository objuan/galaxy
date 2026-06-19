
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.PackageManager;
using UnityEngine;

[CreateAssetMenu(
    fileName = "EnemyPath",
    menuName = "Game Data/Enemy Path"
)]
public class EnemyPath : ScriptableObject
{
    public int lastFixedPoint = 0;
    public string desc = "";
    public List<Vector2> points = new();

    public SplinePath GetPath()
    {

        SplinePath path = new SplinePath();
        path.points = points;
        path.BuildLengthTable();
        return path;
    }
    public SplinePath GetSubPath(int from, int to)
    {
        SplinePath path = new SplinePath();
        from = Mathf.Clamp(from, 0, points.Count - 1);
        to = Mathf.Clamp(to, from, points.Count - 1);
        path.points = points.GetRange(from, to - from + 1);

        path.BuildLengthTable();
        return path;
    }
}

[Serializable]
public class SplinePath 
{
    public List<Vector2> points = new();

    private List<float> cumulativeLengths;
    private float totalLength;
    private List<float> sampleTs;

    private int samplesPerSegmentUsed;



    public Vector2? EvaluateSinglePathDistance(float distance)
    {
        if (distance <= 0)
            return EvaluateRaw(0);

        if (distance >= totalLength)
            return null;

        int index = cumulativeLengths.BinarySearch(distance);

        if (index < 0)
            index = ~index;

        if (index == 0)
            return EvaluateRaw(0);

        float d0 = cumulativeLengths[index - 1];
        float d1 = cumulativeLengths[index];

        float alpha = (distance - d0) / (d1 - d0);

        float t0 = sampleTs[index - 1];
        float t1 = sampleTs[index];

        float t = Mathf.Lerp(t0, t1, alpha);

        return EvaluateRaw(t);
    }

    public Vector2? EvaluateSinglePath(float t)
    {
        if (points == null || points.Count < 4)
            return null;

        int segments = points.Count - 3;

        if (t < 0)
            t = 0;

        // fine percorso
        if (t >= segments)
            return null;

        int seg = Mathf.FloorToInt(t);
        float localT = t - seg;

        return CatmullRom(
            points[seg],
            points[seg + 1],
            points[seg + 2],
            points[seg + 3],
            localT);
    }

    public Vector2 EvaluatePath(float t)
    {
        int segments =
            points.Count - 3;

        float total = t * segments;

        int seg =
            Mathf.Min(
                Mathf.FloorToInt(total),
                segments - 1);

        float localT =
            total - seg;

        return CatmullRom(
            points[seg],
            points[seg + 1],
            points[seg + 2],
            points[seg + 3],
            localT);
    }

    static Vector2 CatmullRom(
       Vector2 p0,
       Vector2 p1,
       Vector2 p2,
       Vector2 p3,
       float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f *
        (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }

    public void BuildLengthTable(int samplesPerSegment = 50)
    {
        samplesPerSegmentUsed = samplesPerSegment;
        cumulativeLengths = new List<float>();

        int segments = points.Count - 3;

        Vector2 prev = EvaluateRaw(0f);

        sampleTs = new List<float>();

        sampleTs.Add(0);
        cumulativeLengths.Add(0);

        float length = 0;

        for (int s = 0; s < segments; s++)
        {
            int maxI = (s == segments - 1)
                ? samplesPerSegment - 1
                : samplesPerSegment;

            for (int i = 1; i <= maxI; i++)
            {
                float t = s + i / (float)samplesPerSegment;


                Vector2 p = EvaluateRaw(t);

                length += Vector2.Distance(prev, p);

                cumulativeLengths.Add(length);
                sampleTs.Add(t);
                
                prev = p;
            }
        }

        totalLength = length;
    }
    private Vector2 EvaluateRaw(float t)
    {
        int segments = points.Count - 3;

        if (t <= 0)
            t = 0;

        if (t >= segments)
        {
            return CatmullRom(
                points[segments - 1],
                points[segments],
                points[segments + 1],
                points[segments + 2],
                1f);
        }

        int seg = Mathf.FloorToInt(t);
        float localT = t - seg;

        return CatmullRom(
            points[seg],
            points[seg + 1],
            points[seg + 2],
            points[seg + 3],
            localT);
    }
}