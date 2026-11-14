using UnityEngine;
using TMPro;
using MySql.Data.MySqlClient;

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
        DBConnection dbConn = DBConnection.Instance();
        if (dbConn.IsConnect())
        {
            try
            {
                //I actuall have no clue how this is supposed to work
                string query = "UPDATE employee" +
"SET first_name = @firstName, last_name = @lastName" +
"WHERE employee_id = @employeeId";

                MySqlCommand myCommand = new MySqlCommand();
                myCommand.Connection = dbConn.Connection;
                myCommand.CommandText = query;
                myCommand.Parameters.AddWithValue("@firstName", lastName.text);
                myCommand.Parameters.AddWithValue("@lastName", firstName.text);

                Debug.Log(myCommand.ExecuteNonQuery());

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Debug.LogError("Error " + ex.Number + " has occurred: " + ex.Message);
            }
        }
        gameObject.SetActive(false);
    }

    public void onCancel()
    {
        gameObject.SetActive(false);
    }
}
