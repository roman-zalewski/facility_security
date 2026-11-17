using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;

//this script is responsible for spawning the correct prefab for changing the name of employees
//change name just needs to be spawned and despawned.
public class Employees : MonoBehaviour
{
    [SerializeField] private GameObject employeeNamePrefab;
    [SerializeField] private Transform VLG;
    private void OnEnable()
    {
        //destroy all existing names
        foreach(Transform child in VLG.transform)
        {
            Destroy(child.gameObject);
        }
        StartCoroutine(SpawnEmployeesCoroutine());
    }

    private IEnumerator SpawnEmployeesCoroutine()
    {
        string query = "SELECT CONCAT(first_name, ' ', last_name) AS full_name FROM employee";
        StartCoroutine(PythonBackendConnector.Send(query));
        while (PythonBackendConnector.requestPending)
        {
            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log(PythonBackendConnector.responseText);
        Stack<Employee> employees = JsonSerializer.Deserialize<Stack<Employee>>(PythonBackendConnector.responseText, new JsonSerializerOptions { IncludeFields = true });

        GameObject temp;
        ChangeNamePrefab nameTemp;

        while (employees.Count > 0)
        {
            temp = Instantiate(employeeNamePrefab, VLG);
            nameTemp = temp.GetComponent<ChangeNamePrefab>();
            nameTemp.empName.text = employees.Pop().full_name;
        }
    }
}
