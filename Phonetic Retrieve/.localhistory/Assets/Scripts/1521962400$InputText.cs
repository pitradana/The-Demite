using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SQLite4Unity3d;
using System.IO;
using System;

using Mono.Data.Sqlite;
using System.Data;


public class InputText : MonoBehaviour {

    public InputField inputField;
    public Text phonetic;
    public IDbConnection dbconn;

    void Start() {
        string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/words.db";
        dbconn = (IDbConnection)new SqliteConnection(conn);
        
    }

    void Awake() {
        inputField.onEndEdit.AddListener(AcceptStringInput);
    }

	void AcceptStringInput(string userInput) {
        userInput = userInput.ToLower();
        string phoneticResult = findPhonetic(userInput);
        phonetic.text = phoneticResult;
        InputComplete();
    }

    public static IEnumerable<Noun> QueryNoun(SQLiteConnection db, string userInput) {
        return db.Query<Noun>("select * from noun where word = ?", userInput);
    }

    string findPhonetic(string userInput) {
        dbconn.Open();
        string phoneticResult = "";
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlquery = string.Format("SELECT * FROM Noun WHERE Word=\"{0}\"", userInput);
        dbcmd.CommandText = sqlquery;
        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read()) {
            phoneticResult = reader.GetString(2);
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

    void InputComplete() {
        inputField.ActivateInputField();
        inputField.text = null;
    }
}

public class Noun {
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Word { get; set; }
    public string Phonetic { get; set; }
    public string Indo_Phonetic { get; set; }
    public int WordPhoneticDistance { get; set; }
    public int IndoEnPhoneticDistance { get; set; }
    public int WordIndoPhoneticDistance { get; set; }
    public int TotalSyllables { get; set; }
    public int AmbiguityRate { get; set; }
    public int SubKategori0 { get; set; }
    public int SubKategori1 { get; set; }
    public int SubKategori2 { get; set; }
}