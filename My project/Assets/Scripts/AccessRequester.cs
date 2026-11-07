using UnityEngine;
using UnityEngine.UI;

public class AccessRequester : MonoBehaviour
{
    DBConnection dbConn = DBConnection.Instance();
    [SerializeField] private InputField nameInput;
    [SerializeField] private Dropdown facilityDropdown;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dbConn.Server = "zalwe.ski:22";
        dbConn.DatabaseName = "super_secret_labs";
        dbConn.UserName = "[FILL LATER]";
        dbConn.Password = "[FILL LATER]";

        //some code to fetch the facilities in the database
    }

    public void OnAccess()
    {
        if (dbConn.IsConnect())
        {
            //I actuall have no clue how this is supposed to work
            string query = "CREATE ";
        }
    }
}
