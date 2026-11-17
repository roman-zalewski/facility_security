using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using System.Text.Json;

public class AccessRequester : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_Dropdown facilityDropdown;
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
        string query = "INSERT INTO access_log " +
"(employee_id, facility_id, access_status, access_time, access_date, in_or_out) " +
"SELECT @employeeId, @facilityId, " +
   "IF(d.granted, 'Granted', 'Denied'), " +
   "CURTIME(), CURDATE(), " +
   "IF(d.granted, @direction, NULL) " +
"FROM( " +
"SELECT IF(@direction = 'In', " +
    "EXISTS( " +
        "SELECT 1 " +
        "FROM access a " +
        "JOIN employee e ON e.employee_id = a.employee_id " +
        "WHERE a.employee_id = @employeeId " +
          "AND a.facility_id = @facilityId " +
          "AND e.employment_status = 'Active' " +
    "), " +
    "EXISTS( " +
        "SELECT 1 " +
        "FROM( " +
            "SELECT in_or_out " +
            "FROM access_log " +
            "WHERE employee_id = @employeeId " +
              "AND facility_id = @facilityId " +
              "AND access_status = 'Granted' " +
              "AND in_or_out IS NOT NULL" +
            "ORDER BY access_date DESC, access_time DESC, entry_id DESC " +
            "LIMIT 1 " +
        ") last_event " +
        "WHERE last_event.in_or_out = 'In' " +
    ") " +
") AS granted " +
") d; + " +
"SET @facilityId = SELECT facilityId FROM facilities WHERE facility_name = " + facilityDropdown.options[facilityDropdown.value].text +
"SET @empoyeeId = SELECT emploayeeId FROM employee WHERE first_name = " + nameInput.text;

        PythonBackendConnector.Send(query);
        while (PythonBackendConnector.requestPending)
        {
            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log("Send Complete: " + PythonBackendConnector.responseText); //make a tmpro thing that actually makes it clear to the user.
    }
}
