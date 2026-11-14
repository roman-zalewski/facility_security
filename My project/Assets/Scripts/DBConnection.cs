/* I have read that doing this for actual applicaitons is really bad, but who cares?*/
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using UnityEngine;

public class DBConnection
{
    public string Server { get; set; }
    public string DatabaseName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }

    public MySqlConnection Connection { get; set; }

    private static DBConnection _instance = null;
    public static DBConnection Instance()
    {
        if (_instance == null)
            _instance = new DBConnection();
        return _instance;
    }

    public bool IsConnect()
    {
        if (Connection == null)
        {
            if (String.IsNullOrEmpty(DatabaseName))
                return false;
            string connstring = string.Format("Server={0};port=3307;database={1};username={2};password={3}", Server, DatabaseName, UserName, Password);
            Connection = new MySqlConnection(connstring);
            Connection.Open();
            Debug.Log("Connection Open");
        }

        return true;
    }

    public void Close()
    {
        Debug.Log("Connection Closing");
        Connection.Close();
    }
}
