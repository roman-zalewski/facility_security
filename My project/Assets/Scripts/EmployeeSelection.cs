using UnityEngine;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

//adding and deleting employees should look through every toggle to see if it enabled or not, and handle accordingly
public class EmployeeSelection : MonoBehaviour
{
    [SerializeField] GameObject employeeSelectionPrefab;
    [SerializeField] Transform VLG;

    private Stack<EmployeeSelectionPrefab> employees;

    int facilityId;
    bool add;

    public void OnStartAdd(int facility)
    {
        add = true;
        facilityId = facility;
        //a SQL query that gets every employee that does not have access to that facility
        try
        {
            employees = new Stack<EmployeeSelectionPrefab>();
            MySqlConnection conn = new MySqlConnection("Server = 127.0.0.1; " +
                "port = 3307; " +
                "username = manager; " +
                "password = P@ssword" +
                "database =super_secret_labs; ");
            string query = "[[[INSERT QUEREY HERE]]]";
            MySqlCommand myCommand = new MySqlCommand(query, conn);

            GameObject temp;
            EmployeeSelectionPrefab nameTemp;

            using var myReader = myCommand.ExecuteReader();
            {
                while (myReader.Read())
                {
                    temp = Instantiate(employeeSelectionPrefab, VLG);
                    nameTemp = temp.GetComponent<EmployeeSelectionPrefab>();
                    nameTemp.nameText.text = myReader.GetString("full_name");
                    nameTemp.employeeId = myReader.GetInt32("employee_id");
                    nameTemp.toggle.isOn = false;
                    employees.Push(nameTemp);
                }
            }
            conn.Close();

        }
        catch (MySqlException ex)
        {
            Debug.LogError("Error " + ex.Number + " has occurred: " + ex.Message);
        }
    }

    public void OnStartRemove(int facility)
    {
        add = false;
        facilityId = facility;
        //a SQL query that gets every employee that has access to that facility
        try
        {
            employees = new Stack<EmployeeSelectionPrefab>();
            MySqlConnection conn = new MySqlConnection("Server = 127.0.0.1; " +
                "port = 3307; " +
                "username = manager; " +
                "password = P@ssword" +
                "database =super_secret_labs; ");
            string query = "[[[INSERT QUEREY HERE]]]";
            MySqlCommand myCommand = new MySqlCommand(query, conn);

            GameObject temp;
            EmployeeSelectionPrefab nameTemp;

            using var myReader = myCommand.ExecuteReader();
            {
                while (myReader.Read())
                {
                    temp = Instantiate(employeeSelectionPrefab, VLG);
                    nameTemp = temp.GetComponent<EmployeeSelectionPrefab>();
                    nameTemp.nameText.text = myReader.GetString("full_name");
                    nameTemp.employeeId = myReader.GetInt32("employee_id");
                    nameTemp.toggle.isOn = true;
                    employees.Push(nameTemp);
                }
            }
            conn.Close();

        }
        catch (MySqlException ex)
        {
            Debug.LogError("Error " + ex.Number + " has occurred: " + ex.Message);
        }
    }

    public void OnSubmit()
    {
        //a sql update for bulk adding or removing people
        try
        {
            MySqlConnection conn = new MySqlConnection("Server = 127.0.0.1; " +
                "port = 3307; " +
                "username = manager; " +
                "password = P@ssword" +
                "database =super_secret_labs; ");
            string query = "DELETE FROM access" +
            "WHERE facility_id = @facilityId AND employee_id = @employeeId;";
            if(add){
                query = "INSERT INTO access (facility_id, employee_id)" +
                "VALUES (@facilityId, @employeeId);";
            }
            MySqlCommand myCommand = new MySqlCommand(query, conn);
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
            conn.Close();

        }
        catch (MySqlException ex)
        {
            Debug.LogError("Error " + ex.Number + " has occurred: " + ex.Message);
        }
        gameObject.SetActive(false);
    }

    public void OnCancel()
    {
        gameObject.SetActive(false);
    }
}
