using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;
using System;

public class InputTextDataMuse : MonoBehaviour {
    public InputField inputField;
    public Text phonetic;
    public Text ambiguityRate;

    // Use this for initialization
    void Start () {
        StartCoroutine(getUnityWebRequest());
    }

    IEnumerator getUnityWebRequest() {
        UnityWebRequest www = UnityWebRequest.Get("https://api.datamuse.com/words?sl=adventure");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        } else {
            Debug.Log(www.downloadHandler.text);
        }
    }
}
