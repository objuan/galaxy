using UnityEngine;
using System.IO;

public class PxFile
{
 
    public static bool winMode => Application.platform != RuntimePlatform.Android;

	public static bool IsNetworkPath(string full_path) {
		return full_path.Contains("://") || full_path.Contains(":///");
	}

    public static void Initialize()
    {
        //Debug.Log("Win Mode " + winMode);

    }

    public static string streamingAssetsPath
    {
		get
        {
        //    if (winMode)
                return Application.streamingAssetsPath;
            //else
            //    return "";
        }
    }

    public static bool Exists(string path)
    {
		if (!IsNetworkPath(path))
			return File.Exists(path);
		else
		{
			UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(path);
			var ret = www.SendWebRequest();

			Debug.Log("GET "+ path);

			while (ret.isDone == false || www.isDone == false)
			{
				System.Threading.Thread.Sleep(10);
			}
			if (www.isNetworkError || www.isHttpError)
			{
				Debug.Log("NOT FOUND");

				return false;
			}
			else
				return true;
		}
    }

    public static void Delete(string path)
    {
		if (!IsNetworkPath(path))
			File.Delete(path);
        else
        {
        }
    }
    public static string ReadAllText(string path)
    {
		if (!IsNetworkPath(path))
			return File.ReadAllText(path);
        else
        {
            return LoadStreamFile(path);
        }
    }
	public static byte[] ReadAllBytes(string path)
	{
		if (!IsNetworkPath(path))
			return File.ReadAllBytes(path);
		else
		{
			return LoadStreamBytes(path);
		}
	}
	public static string[] ReadAllLines(string path)
    {
		if (!IsNetworkPath(path))
			return File.ReadAllLines(path);
        else
        {
            return LoadStreamFile(path).Split('\n');
        }
    }
	public static void WriteAllText(string path,string content)
	{
		if (!IsNetworkPath(path))
			 File.WriteAllText(path, content);
		else
		{
			
		}
	}
	// ==================


	public static string LoadStreamFile(string full_path)
	{
		//var localizedText = new Dictionary<string, string>();
		//	string filePath; 
		//filePath = Path.Combine(FolderPath + "/", fileName); 
		string dataAsJson = "";

		//Debug.Log("LOAD :" + full_path);

		if (IsNetworkPath(full_path))
		{
			//	var full_path1 = "jar:file://" + Application.dataPath + "!/assets/" + "Roads/" + new FileInfo(full_path).Name;

			//	Debug.Log("<< " + full_path1);

			try
			{
				UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(full_path);
				var ret = www.SendWebRequest();
				while (ret.isDone == false || www.isDone == false)
				{
					System.Threading.Thread.Sleep(10);
				}
				if (www.isNetworkError || www.isHttpError)
				{
					Debug.LogWarning($"Network error whilst downloading [{full_path}] Error: [{www.error}]");
					return "";
				}

				//yield return www.SendWebRequest();
				//dataAsJson = www.downloadHandler.text;

				Debug.Log("Load OK");

				//yield return www.downloadHandler.text;
				return www.downloadHandler.text;

			}
			catch (System.Exception e)
			{
				Debug.LogError(e);
				return "";
			}
		}
		else
		{
			dataAsJson = File.ReadAllText(full_path);
			return dataAsJson;
		}
		//	return dataAsJson;
		//LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

		//for (int i = 0; i < loadedData.items.Length; i++)
		//{
		//	localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
		//	Debug.Log("KEYS:" +loadedData.items[i].key);
		//}

	}

	public static byte[] LoadStreamBytes(string full_path)
	{
		if (IsNetworkPath(full_path))
		{

			try
			{
				UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(full_path);
				var ret = www.SendWebRequest();
				while (ret.isDone == false || www.isDone == false)
				{
					System.Threading.Thread.Sleep(10);
				}
				if (www.isNetworkError || www.isHttpError)
				{
					Debug.LogWarning($"Network error whilst downloading [{full_path}] Error: [{www.error}]");
					return new byte[] { };
				}

				Debug.Log("Load OK");

				return www.downloadHandler.data;

			}
			catch (System.Exception e)
			{
				Debug.LogError(e);
				return new byte[] { };
			}
		}
		else
		{
			var b = File.ReadAllBytes(full_path);
			return b;
		}
		//	return dataAsJson;
		//LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

		//for (int i = 0; i < loadedData.items.Length; i++)
		//{
		//	localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
		//	Debug.Log("KEYS:" +loadedData.items[i].key);
		//}

	}
}

