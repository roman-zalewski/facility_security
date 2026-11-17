
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class PythonBackendConnector : MonoBehaviour
{
    void Start() 
    { 
        StartCoroutine(Send()); 
    }
    IEnumerator Send()
    {
        string json = "{\"sql\":\"SELECT employee_id, first_name, last_name FROM employee\"}";
        var req = new UnityWebRequest("http://127.0.0.1:49170/data/manager", "POST");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        yield return req.SendWebRequest();
        if (req.result == UnityWebRequest.Result.Success)
            Debug.Log(req.downloadHandler.text);
        else
            Debug.LogError(req.error + " | " + req.downloadHandler.text);
    }
}
