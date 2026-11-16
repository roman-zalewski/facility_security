using UnityEngine;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

public class FacilitySelector : MonoBehaviour
{
    [SerializeField] private GameObject FacilityPrefab;
    [SerializeField] private Transform FacilityVLG;
    private Stack<GameObject> facilityObjects;

    private void Start()
    {
        facilityObjects = new Stack<GameObject>();
        OnRefresh();
    }

    public void OnRefresh()
    {
        while (facilityObjects.Count > 0)
        {
            Destroy(facilityObjects.Pop());
        }

        //insert facility get query here
        Stack<string> facilityNames = new Stack<string>();
        Stack<int> facilityIDs = new Stack<int>();
        Stack<int> facilityCount = new Stack<int>();
        try
        {
            MySqlConnection conn = new MySqlConnection("Server = 127.0.0.1; " +
                "port = 3307; " +
                "username = manager; " +
                "password = P@ssword" +
                "database =super_secret_labs; ");
            //get facility name and facility ID
            string query = "SELECT facility_name, facility_id FROM facilities";
            MySqlCommand myCommand = new MySqlCommand(query, conn);


            using var myReader = myCommand.ExecuteReader();
            {
                while (myReader.Read())
                {
                    facilityNames.Push(myReader.GetString("facility_name"));
                    facilityIDs.Push(myReader.GetInt32("facility_id"));
                }
            }
            Stack<int> facilityIdsForQueries = facilityIDs;
            //this only does specific facilities, it needs to be all facilities
            query = "SELECT COUNT(*) AS people_inside" +
"FROM(" +
    "SELECT l.employee_id, l.in_or_out, " +
           "ROW_NUMBER() OVER(PARTITION BY l.employee_id" +
                              "ORDER BY l.access_date DESC, l.access_time DESC, l.entry_id DESC) AS rn" +
    "FROM access_log l" +
    "WHERE l.facility_id = @facility_id" +
      "AND l.access_status = 'Granted'" +
      "AND l.in_or_out IS NOT NULL" +
") latest" +
"WHERE rn = 1 AND in_or_out = 'In'; ";
            myCommand = new MySqlCommand(query, conn);

            while (facilityIdsForQueries.Count > 0)
            {
                myCommand.Parameters.AddWithValue("@facility_id", facilityIdsForQueries.Pop());

                using var myReader2 = myCommand.ExecuteReader();
                {
                    while (myReader2.Read())
                    {
                        facilityCount.Push(myReader2.GetInt32("people_inside"));
                    }
                }
            }
            conn.Close();

        }
        catch (MySqlException ex)
        {
            Debug.LogError("Error " + ex.Number + " has occurred: " + ex.Message);
        }


        GameObject temp;

        for (int i = 0; i < facilityNames.Count; i++)
        {
            temp = Instantiate(FacilityPrefab, FacilityVLG);
            facilityObjects.Push(temp);
            temp.GetComponentInChildren<FacilityGameobjects>().FacilityText.text = facilityNames.Pop();
            temp.GetComponentInChildren<FacilityGameobjects>().facilityID = facilityIDs.Pop();
            temp.GetComponentInChildren<FacilityGameobjects>().UserCountText.text = facilityCount.Pop().ToString();
        }
    }
}
