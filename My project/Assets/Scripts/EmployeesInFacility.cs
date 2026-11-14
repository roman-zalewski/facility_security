using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class EmployeesInFacility : MonoBehaviour
{
    [SerializeField] private GameObject tmproPrefab;
    [SerializeField] private Transform textParent;
    private Stack<GameObject> names;
    private string facilityName;

    private void Start()
    {
        names = new Stack<GameObject>();
        gameObject.SetActive(false);
    }

    public void OnOpen(string facility)
    {
        facilityName = facility;
        OnRefresh();
    }
    
    public void OnRefresh()
    {
        if (names != null)
        {
            while(names.Count > 0)
            {
                Destroy(names.Pop());
            }
        }
        //insert sql Query
        string[] newNames = { "John Doe", "Mary Vasques" };
        GameObject temp;
        foreach( string name in newNames)
        {
            temp = Instantiate(tmproPrefab, textParent);
            names.Push(temp);
            temp.GetComponent<TextMeshProUGUI>().text = name;
        }
    }
    
    public void OnBack()
    {
        gameObject.SetActive(false);
    }

    public void OnUpdateAccess()
    {

    }
}
