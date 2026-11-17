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
        gameObject.SetActive(false);
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
        string query = "SELECT CONCAT(first_name, ' ', last_name) AS full_name FROM employee";
        
        StartCoroutine(PythonBackendConnector.Send(query));
        while (PythonBackendConnector.requestPending)
        {
            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log(PythonBackendConnector.responseText);
        Stack<Employee> employees = JsonSerializer.Deserialize<Stack<Employee>>(PythonBackendConnector.responseText, new JsonSerializerOptions { IncludeFields = true });


        GameObject temp;
        while (employees.Count > 0)
        {
            temp = Instantiate(tmproPrefab, textParent);
            names.Push(temp);
            temp.GetComponent<TextMeshProUGUI>().text = employees.Pop().full_name;
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
