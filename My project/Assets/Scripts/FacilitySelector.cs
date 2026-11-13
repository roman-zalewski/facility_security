using UnityEngine;
using TMPro;

public class FacilitySelector : MonoBehaviour
{
    [SerializeField] private GameObject FacilityPrefab;
    [SerializeField] private Transform FacilityVLG;
    public void OnRefresh()
    {
            //insert facility get query here
        string[] facilities = {"facility A", "facility B"};
        int[] facilityCount = 2,4};

        GameObject temp;

        for(int i = 0; i < facilities.count; i++)
        {
            temp = instatiate(FacilityPrefab, FacilityVLG);
            temp.GetComponentInChildren<FacilityGameobject>().FacilityText.text = facilities[i];
            temp.GetComponentInChildren<FacilityGameobject>().UserCountText.text = facilityCount[i];
        }
    }
}
