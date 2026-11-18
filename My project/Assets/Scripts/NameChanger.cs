using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;

public class NameChanger : MonoBehaviour
{
    [SerializeField] TMP_InputField firstName;
    [SerializeField] TMP_InputField lastName;
    int employeeID;
    public void OnStart(int employeeID)
    {
        this.employeeID = employeeID;
    }

    public void OnConfirm()
    {
        StartCoroutine( ConfirmCoroutine());

    }

    private IEnumerator ConfirmCoroutine()
    {
        string query = "UPDATE employee " +
"SET first_name = '" + firstName.text + "', last_name = '" + lastName.text + "' " +
"WHERE employee_id = " + employeeID.ToString() + ";";
        Debug.Log("NameChanger Query: " + query);
        StartCoroutine(PythonBackendConnector.Send(query));
        while (PythonBackendConnector.requestPending)
        {
            yield return new WaitForSeconds(0.5f);
        }

        gameObject.SetActive(false);
    }

    public void onCancel()
    {
        gameObject.SetActive(false);
    }
}
