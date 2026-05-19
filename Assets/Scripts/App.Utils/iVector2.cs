
using Newtonsoft.Json;
using System;
using UnityEngine;

[System.Serializable]
public struct iVector2
{
    public static iVector2 zero = new iVector2(0,0);

    public int x;
    public int y;

    [JsonIgnore]
    public float Length => Mathf.Sqrt( x*x + y*y);
    [JsonIgnore]
    public float LengthSquared => (x * x + y * y );

    public iVector2(float[] v)
    {
        this.x = (int)v[0];
        this.y = (int)v[1];

    }
    public iVector2(int[] v)
    {
        this.x = (int)v[0];
        this.y = (int)v[1];
    }
    public iVector2(int _x,int _y)
    {
        this.x = _x;
        this.y = _y;
    }
    public iVector2(Vector3 v)
    {
        this.x = (int)v.x;
        this.y = (int)v.y;
    }
    public iVector2(iVector2 v)
    {
        this.x = (int)v.x;
        this.y = (int)v.y;
    }
    public static iVector2 operator +(iVector2 a, iVector2 b)
    {
        return new iVector2(a.x + b.x, a.y + b.y);
    }
    public static iVector2 operator -(iVector2 a, iVector2 b)
    {
        return new iVector2(a.x - b.x, a.y - b.y);
    }
    public static iVector2 Sub(iVector2 a, iVector3 b)
    {
        return new iVector2(a.x - b.x, a.y - b.y);
    }
    public static iVector2 Add(iVector2 a, iVector2 b)
    {
        return new iVector2(a.x + b.x, a.y + b.y);
    }
    public static float Distance(iVector2 a, iVector2 b)
    {
        return (a-b).Length;
    }
    public static float DistanceSquared(iVector2 a, iVector2 b)
    {
        return (a-b).LengthSquared;
    }

    public override bool Equals(object obj)
    {
        iVector2 a = (iVector2)obj;
        return x == a.x && y == a.y ;
    }
    public override string ToString()
    {
        return "" + x + "," + y ;
    }

    public override int GetHashCode()
    {
        return x + y << 8;
    }

    public  Vector3 ToVector3()
    {
        return new Vector3(x, y,0);
    }
}



