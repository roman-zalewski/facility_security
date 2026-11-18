using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using System.Text.Json;

public class AccessRequester : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_Dropdown facilityDropdown;
    [SerializeField] private TextMeshProUGUI resultText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(FacilityCoroutine());
    }

    private IEnumerator testFacilityCoroutine()
    {
        string query = "SELECT * FROM facilities WHERE facility_id = 1";
        StartCoroutine(PythonBackendConnector.Send(query));
        while (PythonBackendConnector.requestPending)
        {
            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log(PythonBackendConnector.responseText);
        List<Facility> facility = JsonSerializer.Deserialize<List<Facility>>(PythonBackendConnector.responseText, new JsonSerializerOptions { IncludeFields = true });
        Debug.Log(facility[0].facility_name);
    }

    private IEnumerator FacilityCoroutine()
    {
        string query = "SELECT facility_name FROM facilities";
        StartCoroutine(PythonBackendConnector.Send(query));
        while (PythonBackendConnector.requestPending)
        {
            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log(PythonBackendConnector.responseText);
        Stack<Facility> facilities = JsonSerializer.Deserialize<Stack<Facility>>(PythonBackendConnector.responseText, new JsonSerializerOptions { IncludeFields = true });
        List<string> facilityNames = new List<string>();
        Debug.Log(facilities.Count);
        while (facilities.Count > 0)
        {
            facilityNames.Add(facilities.Pop().facility_name);
        }
        facilityNames.Reverse();
        facilityDropdown.ClearOptions();
        facilityDropdown.AddOptions(facilityNames);
    }

    public void OnAccess()
    {
        StartCoroutine(AccessCoroutine());

    }

    private IEnumerator AccessCoroutine()
    {
        int empID;
        int facID;
        string query = "SELECT * FROM employee WHERE first_name = '" + nameInput.text + "';";
        StartCoroutine(PythonBackendConnector.Send(query));
        resultText.text = "Request Sent";
        while (PythonBackendConnector.requestPending)
        {
            yield return new WaitForSeconds(0.5f);
        }

        empID = JsonSerializer.Deserialize<List<Employee>>(PythonBackendConnector.responseText, new JsonSerializerOptions { IncludeFields = true })[0].employee_id;

        query = "SELECT * FROM facilities WHERE facility_name = '" + facilityDropdown.options[facilityDropdown.value].text + "';";
        StartCoroutine(PythonBackendConnector.Send(query));
        resultText.text = "Request Sent";
        while (PythonBackendConnector.requestPending)
        {
            yield return new WaitForSeconds(0.5f);
        }

        facID = JsonSerializer.Deserialize<List<Facility>>(PythonBackendConnector.responseText, new JsonSerializerOptions { IncludeFields = true })[0].facility_id;

        query = "INSERT INTO access_log " +
"(employee_id, facility_id, access_status, access_time, access_date, in_or_out) " +
"SELECT " + empID+ ", "+ facID + ", " +
   "IF(d.granted, 'Granted', 'Denied'), " +
   "CURTIME(), CURDATE(), " +
   "IF(d.granted, 'In', NULL) " +
"FROM( " +
"SELECT IF('In' = 'In', " +
    "EXISTS( " +
        "SELECT 1 " +
        "FROM access a " +
        "JOIN employee e ON e.employee_id = a.employee_id " +
        "WHERE a.employee_id = " + empID + " " +
          "AND a.facility_id = " + facID + " " +
          "AND e.employment_status = 'Active' " +
    "), " +
    "EXISTS( " +
        "SELECT 1 " +
        "FROM( " +
            "SELECT in_or_out " +
            "FROM access_log " +
            "WHERE employee_id = " + empID + " " +
              "AND facility_id = " + facID + " " +
              "AND access_status = 'Granted' " +
              "AND in_or_out IS NOT NULL " +
            "ORDER BY access_date DESC, access_time DESC, entry_id DESC " +
            "LIMIT 1 " +
        ") last_event " +
        "WHERE last_event.in_or_out = 'In' " +
    ") " +
") AS granted " +
") d; ";

        Debug.Log(query);
        StartCoroutine(PythonBackendConnector.SendGUI(query));
        while (PythonBackendConnector.requestPending)
        {
            yield return new WaitForSeconds(0.5f);
        }

        query = "SELECT entry_id, access_status, in_or_out, access_date, access_time " +
        "FROM access_log " +
        "ORDER BY access_date DESC, access_time DESC, entry_id DESC " +
        "LIMIT 1;";
        StartCoroutine(PythonBackendConnector.Send(query));
        resultText.text = "Request Sent";
        while (PythonBackendConnector.requestPending)
        {
            yield return new WaitForSeconds(0.5f);
        }
        resultText.text = JsonSerializer.Deserialize<List<AccessLog>>(PythonBackendConnector.responseText, new JsonSerializerOptions { IncludeFields = true })[0].access_status;
    }
}
