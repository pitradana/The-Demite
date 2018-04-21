using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Networking;
using System.Text;
using System;

public class WordInputDataMuse : MonoBehaviour {
    public InputField Field;
    public Text TextBox;

    IEnumerator getUnityWebRequest() {
        UnityWebRequest www = UnityWebRequest.Get("https://api.datamuse.com/words?sl=adventure");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        } else {
            Debug.Log(www.downloadHandler.text);
        }
    }

    public void CopyText() {
        StartCoroutine(getUnityWebRequest());
        TextBox.text = Field.text;
    }
}
