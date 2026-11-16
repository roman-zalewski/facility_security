using UnityEngine;
using MySql.Data.MySqlClient;

//this script is responsible for spawning the correct prefab for changing the name of employees
//change name just needs to be spawned and despawned.
public class Employees : MonoBehaviour
{
    [SerializeField] private GameObject employeeNamePrefab;
    [SerializeField] private Transform VLG;
    private void OnEnable()
    {
        //a sql query that gets all employees and instantiates a gameobject for each of them
        try
        {
            MySqlConnection conn = new MySqlConnection("Server = 127.0.0.1; " +
                "port = 3307; " +
                "username = manager; " +
                "password = P@ssword" +
                "database =super_secret_labs; ");
            string query = "SELECT CONCAT(first_name, ' ', last_name) AS full_name FROM employee";
            MySqlCommand myCommand = new MySqlCommand(query, conn);

            GameObject temp;
            ChangeNamePrefab nameTemp;

            using var myReader = myCommand.ExecuteReader();
            {
                while (myReader.Read())
                {
                    temp = Instantiate(employeeNamePrefab, VLG);
                    nameTemp = temp.GetComponent<ChangeNamePrefab>();
                    nameTemp.empName.text = myReader.GetString("full_name");
                }
            }
            conn.Close();

        }
        catch (MySqlException ex)
        {
            Debug.LogError("Error " + ex.Number + " has occurred: " + ex.Message);
        }
    }
}
