using UnityEngine;
using System.Collections.Generic;

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
        string[] facilityNames = {"facility A", "facility B"};
        int[] facilityCount = {2,4};

        GameObject temp;

        for(int i = 0; i < facilityNames.Length; i++)
        {
            temp = Instantiate(FacilityPrefab, FacilityVLG);
            facilityObjects.Push(temp);
            temp.GetComponentInChildren<FacilityGameobjects>().FacilityText.text = facilityNames[i];
            temp.GetComponentInChildren<FacilityGameobjects>().UserCountText.text = facilityCount[i].ToString();
        }
    }
}
