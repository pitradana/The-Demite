using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SQLite4Unity3d;
using System.IO;
using System;

public class InputText : MonoBehaviour {

    public InputField inputField;
    public Text phonetic;
    public string filename = @"E:\projects\unity\Phonetic Retrieve\Assets\StreamingAssets\words.db";



    void Awake() {
        inputField.onEndEdit.AddListener(AcceptStringInput);
        
    }

	void AcceptStringInput(string userInput) {
        userInput = userInput.ToLower();
        string phoneticResult = findPhonetic(userInput);
        phonetic.text = phoneticResult;
        InputComplete();
    }

    string findPhonetic(string userInput) {
        var conn = new SQLiteConnection(Path.GetFileName(filename));
        var query = conn.Query<Noun>("select phonetics from Noun where word = ?", userInput);
        //var query = conn.Query<Noun>("select phonetics from noun where word = ?", userInput);
        return query.ToString();
        
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