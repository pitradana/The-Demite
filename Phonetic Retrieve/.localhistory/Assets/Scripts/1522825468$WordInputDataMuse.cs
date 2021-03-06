﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Networking;
using System.Text;
using System;

using SimpleJSON;

public class WordInputDataMuse : MonoBehaviour {
    public InputField Field;
    public Text TextBox;

    IEnumerator getUnityWebRequest() {
        UnityWebRequest www = UnityWebRequest.Get("https://api.datamuse.com/words?sl=adventure");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        } else {
            //Debug.Log(www.downloadHandler.text);
            var jsonStr = www.downloadHandler.text;
            DataMuse wordData = JsonUtility.FromJson<DataMuse>(jsonStr);
            Debug.Log(wordData.word);
            
        }
    }

    public void CopyText() {
        StartCoroutine(getUnityWebRequest());
    }
}

public class DataMuse {
    public string word;
    public int numSyllables;
    public string pron;
    public string ipa_pron;

}
