using UnityEngine;
using TMPro;
using MySql.Data.MySqlClient;

public class ChangeNamePrefab : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI empText;
    
    public void OnChageName()
    {
        int empId = 0;
        FindAnyObjectByType<NameChanger>(FindObjectsInactive.Include).gameObject.SetActive(true);
        DBConnection dbConn = DBConnection.Instance();
        if (dbConn.IsConnect())
        {
            string query = "SELECT employee_id FROM employee WHERE first_name = @firstName";
            MySqlCommand myCommand = new MySqlCommand(query, dbConn.Connection);
            myCommand.Parameters.AddWithValue("@firstName", empText.text.Split(" ")[0]);


            using var myReader = myCommand.ExecuteReader();
            {
                while (myReader.Read())
                {
                    empId = myReader.GetInt32("employee_id");
                }
            }

            dbConn.Close();
        }
        else
        {
            Debug.LogError("NO CONNECTION!");
        }
        FindAnyObjectByType<NameChanger>().OnStart(empId);
    }
}
