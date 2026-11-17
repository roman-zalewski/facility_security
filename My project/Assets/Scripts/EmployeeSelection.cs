using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;

//adding and deleting employees should look through every toggle to see if it enabled or not, and handle accordingly
public class EmployeeSelection : MonoBehaviour
{
    [SerializeField] GameObject employeeSelectionPrefab;
    [SerializeField] Transform VLG;

    private Stack<EmployeeSelectionPrefab> employeePrefabs;

    int facilityId;
    bool add;

    public void OnStartAdd(int facility)
    {
        Debug.Log("add");
        add = true;
        facilityId = facility;

        foreach (Transform child in VLG.transform)
        {
            Destroy(child.gameObject);
        }

        employeePrefabs = new Stack<EmployeeSelectionPrefab>();

        StartCoroutine( OnStartCoroutine());
    }

    private IEnumerator OnStartCoroutine()
    {
        string query = "SELECT e.employee_id, e.first_name, e.last_name, e.job_title, e.employment_status " +
        "FROM employee e " +
        "WHERE NOT EXISTS( " +
            "SELECT 1 " +
            "FROM access a " +
            "WHERE a.employee_id = e.employee_id " +
              "AND a.facility_id = " + facilityId +
        ") " +
        "ORDER BY e.last_name, e.first_name;";
        StartCoroutine(PythonBackendConnector.Send(query));
        while (PythonBackendConnector.requestPending)
        {
            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log(PythonBackendConnector.responseText);
        Stack<Employee> employees = JsonSerializer.Deserialize<Stack<Employee>>(PythonBackendConnector.responseText, new JsonSerializerOptions { IncludeFields = true });

        GameObject temp;
        Employee empTemp;
        EmployeeSelectionPrefab nameTemp;
            while (employees.Count > 0)
            {
                temp = Instantiate(employeeSelectionPrefab, VLG);
                nameTemp = temp.GetComponent<EmployeeSelectionPrefab>();
            empTemp = employees.Pop();
                nameTemp.nameText.text = empTemp.first_name + " " + empTemp.last_name;
                nameTemp.employeeId = empTemp.employee_id;
                nameTemp.toggle.isOn = false;
                employeePrefabs.Push(nameTemp);
            }
    }

    public void OnStartRemove(int facility)
    {
        add = false;
        facilityId = facility;

        foreach (Transform child in VLG.transform)
        {
            Destroy(child.gameObject);
        }

        employeePrefabs = new Stack<EmployeeSelectionPrefab>();

        StartCoroutine( OnRemoveCoroutine());

    }

    private IEnumerator OnRemoveCoroutine()
    {
        string query = "SELECT e.employee_id, e.first_name, e.last_name, e.job_title, e.employment_status " +
        "FROM employee e " +
        "WHERE EXISTS( " +
            "SELECT 1 " +
            "FROM access a " +
            "WHERE a.employee_id = e.employee_id " +
              "AND a.facility_id = " + facilityId +
        ") " +
        "ORDER BY e.last_name, e.first_name;";
        Debug.Log(query);
        StartCoroutine(PythonBackendConnector.Send(query));
        while (PythonBackendConnector.requestPending)
        {
            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log(PythonBackendConnector.responseText);
        Stack<Employee> employees = JsonSerializer.Deserialize<Stack<Employee>>(PythonBackendConnector.responseText, new JsonSerializerOptions { IncludeFields = true });

        GameObject temp;
        Employee empTemp;
        EmployeeSelectionPrefab nameTemp;
        while (employees.Count > 0)
        {
            temp = Instantiate(employeeSelectionPrefab, VLG);
            nameTemp = temp.GetComponent<EmployeeSelectionPrefab>();
            empTemp = employees.Pop();
            Debug.Log(empTemp.full_name + " " + empTemp.first_name);
            nameTemp.nameText.text = empTemp.first_name + " " + empTemp.last_name;
            nameTemp.employeeId = empTemp.employee_id;
            nameTemp.toggle.isOn = true;
            employeePrefabs.Push(nameTemp);
        }
    }

    public void OnSubmit()
    {
            string query = "DELETE FROM access " +
            "WHERE facility_id = %s AND employee_id = %s;";
            if(add){
                query = "INSERT INTO access (facility_id, employee_id) " +
                "VALUES (%s, %s);";
            }
        /*
            MySqlTransaction tr = myCommand.Transaction;
            EmployeeSelectionPrefab temp;

            try 
            {
                while (employees.Count > 0)
                {
                    temp = employees.Pop(); 
                    if ((temp.toggle.isOn && !add) || (!temp.toggle.isOn && add))
                    {
                        continue;
                    }
                    myCommand.Parameters.Clear();
                    myCommand.Parameters.AddWithValue("@facilityID", facilityId);
                    myCommand.Parameters.AddWithValue("@employeeId", temp.employeeId);
                    //loop through command
                    myCommand.ExecuteNonQuery();
                }
            }
            catch(MySqlException ex)
            {
                Debug.LogError("Error " + ex.Number + " has occurred: " + ex.Message);
                tr.Rollback();
            }

            tr.Commit();
        */
        gameObject.SetActive(false);
    }

    public void OnCancel()
    {
        gameObject.SetActive(false);
    }
}
