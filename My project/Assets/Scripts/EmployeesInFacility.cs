using UnityEngine;
using TMPro;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

public class EmployeesInFacility : MonoBehaviour
{
    [SerializeField] private GameObject tmproPrefab;
    [SerializeField] private Transform textParent;
    private Stack<GameObject> names;
    private string facilityName;

    private void Start()
    {
        names = new Stack<GameObject>();
        gameObject.SetActive(false);
    }

    public void OnOpen(string facility)
    {
        facilityName = facility;
        OnRefresh();
    }
    
    public void OnRefresh()
    {
        if (names != null)
        {
            while(names.Count > 0)
            {
                Destroy(names.Pop());
            }
        }
        //insert sql Query
        Stack<string> newNames = new Stack<string>();

        try
        {
            MySqlConnection conn = new MySqlConnection("Server = 127.0.0.1; " +
                "port = 3307; " +
                "username = manager; " +
                "password = P@ssword" +
                "database =super_secret_labs; ");
            string query = "SELECT CONCAT(first_name, ' ', last_name) AS full_name FROM employee";
            MySqlCommand myCommand = new MySqlCommand(query, conn);


            using var myReader = myCommand.ExecuteReader();
            {
                while (myReader.Read())
                {
                    newNames.Push(myReader.GetString("full_name"));
                }
            }
            conn.Close();

        }
        catch (MySqlException ex)
        {
            Debug.LogError("Error " + ex.Number + " has occurred: " + ex.Message);
        }

        GameObject temp;
        while(newNames.Count > 0)
        {
            temp = Instantiate(tmproPrefab, textParent);
            names.Push(temp);
            temp.GetComponent<TextMeshProUGUI>().text = newNames.Pop();
        }
    }
    
    public void OnBack()
    {
        gameObject.SetActive(false);
    }

    public void OnAddAccess()
    {
        FindAnyObjectByType<EmployeeSelection>(FindObjectsInactive.Include).gameObject.SetActive(true);
        FindAnyObjectByType<EmployeeSelection>().OnStartAdd(facilityName);
    }
    public void OnRemoveAccess()
    {
        FindAnyObjectByType<EmployeeSelection>(FindObjectsInactive.Include).gameObject.SetActive(true);
        FindAnyObjectByType<EmployeeSelection>().OnStartRemove(facilityName);
    }
}
