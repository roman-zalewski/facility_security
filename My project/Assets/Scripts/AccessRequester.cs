using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using TMPro;

public class AccessRequester : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_Dropdown facilityDropdown;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //some code to fetch the facilities in the database
        try{
            MySqlConnection conn = new MySqlConnection("Server = 127.0.0.1; " +
            "port = 3307; " +
            "username = manager; " +
            "password = P@ssword" +
            "database =super_secret_labs; ");
            string query = "SELECT facility_name FROM facilities";
            MySqlCommand myCommand = new MySqlCommand(query, conn);
            Debug.Log("Send Command");
            List<string> facilityNames = new List<string>();
            Debug.Log(2.5f);
            using var myReader = myCommand.ExecuteReader();
            {
                while (myReader.Read())
                {
                    facilityNames.Add(myReader.GetString("facility_name"));
                }
            }
            Debug.Log(3);
            conn.Close();
            Debug.Log(4);

            facilityDropdown.ClearOptions();
            facilityDropdown.AddOptions(facilityNames);
        }
        catch (MySqlException ex)
        {
            Debug.LogError("Error " + ex.Number + " has occurred: " + ex.Message);
        }
    }

    public void OnAccess()
    {
        MySqlConnection conn = new MySqlConnection("Server = 127.0.0.1; " +
        "port = 3307; " +
        "username = GUI_user; " +
        "password = password" +
        "database =super_secret_labs; ");
        try {
            //I actuall have no clue how this is supposed to work
            string query = "INSERT INTO access_log" +
    "(employee_id, facility_id, access_status, access_time, access_date, in_or_out)" +
"SELECT @employeeId, @facilityId," +
       "IF(d.granted, 'Granted', 'Denied')," +
       "CURTIME(), CURDATE()," +
       "IF(d.granted, @direction, NULL)" +
"FROM(" +
    "SELECT IF(@direction = 'In', " +
        "EXISTS(" +
            "SELECT 1" +
            "FROM access a" +
            "JOIN employee e ON e.employee_id = a.employee_id" +
            "WHERE a.employee_id = @employeeId" +
              "AND a.facility_id = @facilityId" +
              "AND e.employment_status = 'Active'" +
        "), " +
        "EXISTS(" +
            "SELECT 1" +
            "FROM(" +
                "SELECT in_or_out" +
                "FROM access_log" +
                "WHERE employee_id = @employeeId" +
                  "AND facility_id = @facilityId" +
                  "AND access_status = 'Granted'" +
                  "AND in_or_out IS NOT NULL" +
                "ORDER BY access_date DESC, access_time DESC, entry_id DESC" +
                "LIMIT 1" +
            ") last_event" +
            "WHERE last_event.in_or_out = 'In'" +
        ")" +
    ") AS granted" +
") d; +" +
"SET @facilityId = SELECT facilityId FROM facilities WHERE facility_name = @facilityName" +
"SET @empoyeeId = SELECT emploayeeId FROM employee WHERE first_name = @employeeName";

            MySqlCommand myCommand = new MySqlCommand(query, conn);
            myCommand.Parameters.AddWithValue("@facilityName", facilityDropdown.options[facilityDropdown.value].text);
            myCommand.Parameters.AddWithValue("@employeeName", nameInput.text);

            Debug.Log(myCommand.ExecuteNonQuery());

            conn.Close();
        
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Debug.LogError("Error " + ex.Number + " has occurred: " + ex.Message);
            }
    }
}
