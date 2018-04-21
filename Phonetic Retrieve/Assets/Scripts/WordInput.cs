using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Mono.Data.Sqlite;
using System.Data;
using UnityEngine.Networking;

public class WordInput : MonoBehaviour {
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
        string word = "";
        string phoneticResult = "";
        string indoPhonetic = "";
        string totalSyllables = "";
        string ambiguityRate = "";
        string subKategori0 = "";
        string subKategori1 = "";
        string subKategori2 = "";
        string similarWord = "";
        string type = "";
        string result = "";
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlquery = string.Format("SELECT * FROM Word WHERE Word=\"{0}\"", userInput);
        dbcmd.CommandText = sqlquery;
        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read()) {
            word = reader.GetString(1);
            phoneticResult = reader.GetString(2);
            indoPhonetic = reader.GetString(3);
            totalSyllables = reader.GetInt32(7).ToString();
            ambiguityRate = reader.GetInt32(8).ToString();
            subKategori0 = reader.GetInt32(9).ToString();
            subKategori1 = reader.GetInt32(10).ToString();
            subKategori2 = reader.GetInt32(11).ToString();
            similarWord = reader.GetString(12);
            type = reader.GetString(13);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();

        result += "Word: "+word + "\n";
        result += "Phonetics: "+phoneticResult+"\n";
        result += "Syllables: " + totalSyllables + "\n";
        result += "Ambiguity: " + ambiguityRate + "\n";
        result += "Sub Kategori 0: " + subKategori0 + "\n";
        result += "Sub Kategori 1: " + subKategori1 + "\n";
        result += "Sub Kategori 2: " + subKategori2 + "\n";
        result += "Tipe: " + type + "\n";
        result += "Similar Word: " + similarWord + "\n";
        //var conn = new SQLiteConnection(Path.GetFileName(filename));
        //var query = conn.Table<Noun>().Where(a => a.Word.Equals(userInput)).FirstOrDefault();
        ////var query = conn.Query<Noun>("select phonetics from noun where word = ?", userInput);
        ////var query = conn.Query<Noun>("select phonetics from noun where word = ?", userInput);
        //return query.ToString();
        return result;
    }
}
