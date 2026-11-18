
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
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
    public int people_inside;
}

public class AccessLog
{
    public int entry_id;
    public string access_status;
    public string in_or_out;
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

    public static IEnumerator SendGUI(string sqlRequest)
    {
        requestPending = true;
        string json = "{\"sql\":\""+ sqlRequest +"\"}";
        UnityWebRequest req = new UnityWebRequest("http://127.0.0.1:49170/data/keycard", "POST");
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

    public static IEnumerator SendBatch(Stack<Employee> employees, int facilityId, string query)
    {
        requestPending = true;
        List<object> queries = new List<object>();
        foreach (Employee employee in employees)
        {
            queries.Add(new { sql = query, @params = new object[] { facilityId, employee.employee_id } });
        }

        string json = JsonSerializer.Serialize(new { statements = queries });
        UnityWebRequest req = new UnityWebRequest("http://127.0.0.1:49170/data/manager/batch", "POST");
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
