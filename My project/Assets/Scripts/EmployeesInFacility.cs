using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;

public class EmployeesInFacility : MonoBehaviour
{
    [SerializeField] private GameObject tmproPrefab;
    [SerializeField] private Transform textParent;
    private Stack<GameObject> names;
    private int facilityId;

    private void Start()
    {
        names = new Stack<GameObject>();
    }

    public void OnOpen(int facilityId)
    {
        this.facilityId = facilityId;
        OnRefresh();
    }
    
    public void OnRefresh()
    {
        if (names != null)
        {
            while (names.Count > 0)
            {
                Destroy(names.Pop());
            }
        }
        //insert sql Query
        StartCoroutine(OnRefreshCoroutine());
    }

    private IEnumerator OnRefreshCoroutine()
    {
        string query = "SELECT e.employee_id, e.first_name, e.last_name, e.job_title, latest.access_time AS entered_at " +
        "FROM ( " +
            "SELECT l.employee_id, l.in_or_out, l.access_time, " +
            "ROW_NUMBER() OVER (PARTITION BY l.employee_id " +
            "ORDER BY l.access_date DESC, l.access_time DESC, l.entry_id DESC) AS rn " +
            "FROM access_log l " +
            "WHERE l.facility_id = " + facilityId + " " +
            "AND l.access_status = 'Granted' " +
        "AND l.in_or_out IS NOT NULL " +
        ") latest " +
        "JOIN employee e ON e.employee_id = latest.employee_id " +
        "WHERE latest.rn = 1 AND latest.in_or_out = 'In' " +
        "ORDER BY e.last_name, e.first_name;";

        Debug.Log("EmployeesInFacility Query: " + query);
        
        StartCoroutine(PythonBackendConnector.Send(query));
        while (PythonBackendConnector.requestPending)
        {
            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log(PythonBackendConnector.responseText);
        Stack<Employee> employees = JsonSerializer.Deserialize<Stack<Employee>>(PythonBackendConnector.responseText, new JsonSerializerOptions { IncludeFields = true });


        GameObject temp;
        Employee tempEmp;
        while (employees.Count > 0)
        {
            temp = Instantiate(tmproPrefab, textParent);
            names.Push(temp);
            tempEmp = employees.Pop();
            temp.GetComponent<TextMeshProUGUI>().text = tempEmp.first_name + " " + tempEmp.last_name;
        }
    }

    public void OnBack()
    {
        gameObject.SetActive(false);
    }

    public void OnAddAccess()
    {
        FindAnyObjectByType<EmployeeSelection>(FindObjectsInactive.Include).gameObject.SetActive(true);
        FindAnyObjectByType<EmployeeSelection>().OnStartAdd(facilityId);
    }
    public void OnRemoveAccess()
    {
        FindAnyObjectByType<EmployeeSelection>(FindObjectsInactive.Include).gameObject.SetActive(true);
        FindAnyObjectByType<EmployeeSelection>().OnStartRemove(facilityId);
    }
}
