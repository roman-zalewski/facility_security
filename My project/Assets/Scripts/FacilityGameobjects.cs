using UnityEngine;
using TMPro;

public class FacilityGameobjects : MonoBehaviour
{
    public TextMeshProUGUI FacilityText;
    public TextMeshProUGUI UserCountText;

    public void OnFacilitySelect()
    {
        var temp = FindAnyObjectByType<EmployeesInFacility>(FindObjectsInactive.Include);
        temp.gameObject.SetActive(true);
        temp.OnOpen(FacilityText.text);
        
    }
}
