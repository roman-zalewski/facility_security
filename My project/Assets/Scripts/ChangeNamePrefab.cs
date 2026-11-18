using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System.Text.Json;

public class ChangeNamePrefab : MonoBehaviour
{
    public TextMeshProUGUI empName;

    public void OnChageName()
    {
        StartCoroutine(OnChangeNameCoroutine());
    }

    private IEnumerator OnChangeNameCoroutine()
    {
        FindAnyObjectByType<NameChanger>(FindObjectsInactive.Include).gameObject.SetActive(true);
        string query = "SELECT employee_id FROM employee WHERE first_name = '" + empName.text.Split(" ")[0] + "';";
        Debug.Log("ChangeNamePrefab Query: " + query);
        StartCoroutine(PythonBackendConnector.Send(query));
        while (PythonBackendConnector.requestPending)
        {
            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log(PythonBackendConnector.responseText);
        Stack<Employee> employee = JsonSerializer.Deserialize<Stack<Employee>>(PythonBackendConnector.responseText, new JsonSerializerOptions { IncludeFields = true });
        FindAnyObjectByType<NameChanger>().OnStart(employee.Pop().employee_id);
    }
}
