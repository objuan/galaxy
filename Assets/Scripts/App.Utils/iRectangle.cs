
using Newtonsoft.Json;
using System;
using UnityEngine;

[System.Serializable]
public struct iRectangle
{
    public iVector3 start;
    public iVector3 end;


    public iRectangle(iVector3 start, iVector3 end)
    {
        this.start = start;
        this.end = end;
    }

}



