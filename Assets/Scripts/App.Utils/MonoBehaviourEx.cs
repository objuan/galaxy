
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class MonoBehaviourEx
{
    static List<MonoBehaviour> monoList = new List<MonoBehaviour>();

    public static void BroadcastMessageExt(this MonoBehaviour targetObj, string methodName, object value = null, SendMessageOptions options = SendMessageOptions.RequireReceiver)
    {
        targetObj.GetComponentsInChildren<MonoBehaviour>(true, monoList);
        for (int i = 0; i < monoList.Count; i++)
        {
            try
            {
                Type type = monoList[i].GetType();

                MethodInfo method = type.GetMethod(methodName, BindingFlags.Instance |
                                                BindingFlags.NonPublic |
                                                 BindingFlags.Public |
                                                 BindingFlags.Static);

                method.Invoke(monoList[i], new object[] { value });
            }
            catch (Exception e)
            {
                //Re-create the Error thrown by the original SendMessage function
                if (options == SendMessageOptions.RequireReceiver)
                    Debug.LogError("SendMessage " + methodName + " has no receiver!");

                //Debug.LogError(e.Message);
            }
        }
    }
}



