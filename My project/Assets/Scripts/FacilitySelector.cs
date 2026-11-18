using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;

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
        StartCoroutine( RefreshCoroutine());
    }

    private IEnumerator RefreshCoroutine()
    {

        //insert facility get query here
        Stack<string> facilityNames = new Stack<string>();
        Stack<int> facilityIDs = new Stack<int>();
        Stack<int> facilityCount = new Stack<int>();

        string query = "SELECT facility_name, facility_id FROM facilities";

        StartCoroutine(PythonBackendConnector.Send(query));
        while (PythonBackendConnector.requestPending)
        {
            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log(PythonBackendConnector.responseText);
        Stack<Facility> facilities = JsonSerializer.Deserialize<Stack<Facility>>(PythonBackendConnector.responseText, new JsonSerializerOptions { IncludeFields = true });

        Stack<Facility> facilityIdsForQueries = new Stack<Facility>(facilities);

        //this only does specific facilities, it needs to be all facilities
        
        while (facilityIdsForQueries.Count > 0)
        {
            query = "SELECT COUNT(*) AS people_inside " +
   "FROM( " +
   "SELECT l.employee_id, l.in_or_out, " +
          "ROW_NUMBER() OVER(PARTITION BY l.employee_id " +
                             "ORDER BY l.access_date DESC, l.access_time DESC, l.entry_id DESC) AS rn " +
   "FROM access_log l " +
   "WHERE l.facility_id = " + facilityIdsForQueries.Pop().facility_id + " " +
     "AND l.access_status = 'Granted' " +
     "AND l.in_or_out IS NOT NULL " +
   ") latest " +
   "WHERE rn = 1 AND in_or_out = 'In'; ";
            StartCoroutine(PythonBackendConnector.Send(query));
            while (PythonBackendConnector.requestPending)
            {
                yield return new WaitForSeconds(0.5f);
            }
            Debug.Log(PythonBackendConnector.responseText);
            Stack<Facility> facility = JsonSerializer.Deserialize<Stack<Facility>>(PythonBackendConnector.responseText, new JsonSerializerOptions { IncludeFields = true });
            
                facilityCount.Push(facility.Pop().people_inside);
        }

        Debug.Log("Check 1");
        GameObject temp;
        int totalFacilities = facilities.Count;
        for (int i = 0; i < totalFacilities; i++)
        {
            Debug.Log("Check 2");
            temp = Instantiate(FacilityPrefab, FacilityVLG);
            facilityObjects.Push(temp);
            Facility tempFacility = facilities.Pop();
            temp.GetComponentInChildren<FacilityGameobjects>().FacilityText.text = tempFacility.facility_name;
            temp.GetComponentInChildren<FacilityGameobjects>().facilityID = tempFacility.facility_id;
            temp.GetComponentInChildren<FacilityGameobjects>().UserCountText.text = facilityCount.Pop().ToString();
        }
    }
}
