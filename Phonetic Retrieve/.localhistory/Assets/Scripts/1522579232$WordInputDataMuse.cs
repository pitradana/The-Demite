using System.Collections;
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
            var Json = JSON.Parse(www.downloadHandler.text);
            //foreach(var aaaa in Json) {
            //    TextBox.text += aaaa.Key;
            //    TextBox.text += "\n";
            //}
            TextBox.text = Json["word"].Value;
        }
    }

    public void CopyText() {
        StartCoroutine(getUnityWebRequest());
    }
}
