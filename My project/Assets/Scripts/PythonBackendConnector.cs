
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class Employee
{
    public int employee_id;
    public string first_name;
    public string last_name;
    public string full_name;
}

public class Facility
{
    public string facility_city;
    public int facility_id;
    public int facility_manager;
    public string facility_name;
    public long facility_phone_number;
    public string facility_state;
}

public class PythonBackendConnector
{

    public static bool requestPending = false;
    public static string responseText;
    public static IEnumerator Send(string sqlRequest)
    {
        requestPending = true;
        string json = "{\"sql\":\""+ sqlRequest +"\"}";
        UnityWebRequest req = new UnityWebRequest("http://127.0.0.1:49170/data/manager", "POST");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        yield return req.SendWebRequest();
        if (req.result == UnityWebRequest.Result.Success)
        {
            Debug.Log(req.downloadHandler.text);
            responseText = req.downloadHandler.text;
        }
        else
            Debug.LogError(req.error + " | " + req.downloadHandler.text);
        requestPending = false;
    }
}
