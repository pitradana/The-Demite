using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Mono.Data.Sqlite;
using System.Data;

public class CopyText : MonoBehaviour {
    public InputField Field;
    public Text TextBox;
    public IDbConnection dbconn;

    void Start() {
        string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/words.db";
        dbconn = (IDbConnection)new SqliteConnection(conn);
    }

    public void phoneticResult() {
        string PhoneticValue = findPhonetic(Field.text);
        TextBox.text = PhoneticValue;
    }

    string findPhonetic(string userInput) {
        dbconn.Open();
        string phoneticResult = "";
        string indoPhonetic = "";
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlquery = string.Format("SELECT * FROM Noun WHERE Word=\"{0}\"", userInput);
        dbcmd.CommandText = sqlquery;
        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read()) {
            phoneticResult = reader.GetString(2);
            indoPhonetic = reader.GetString(3);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        //var conn = new SQLiteConnection(Path.GetFileName(filename));
        //var query = conn.Table<Noun>().Where(a => a.Word.Equals(userInput)).FirstOrDefault();
        ////var query = conn.Query<Noun>("select phonetics from noun where word = ?", userInput);
        ////var query = conn.Query<Noun>("select phonetics from noun where word = ?", userInput);
        //return query.ToString();
        return phoneticResult;
    }
}
