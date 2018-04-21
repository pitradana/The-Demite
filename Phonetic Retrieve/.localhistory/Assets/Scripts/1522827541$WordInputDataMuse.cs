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
        UnityWebRequest www = UnityWebRequest.Get("https://api.datamuse.com/words?sl="+Field.text+"&md=r&ipa=1");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        } else {
            Debug.Log(www.downloadHandler.text);
            string jsonString = fixJson(www.downloadHandler.text);
            DataMuse[] wordData = JsonHelper.FromJson<DataMuse>(jsonString);
            Debug.Log(wordData[0].tags);
            TextBox.text = wordData[0].word;
            
        }
    }

    public void CopyText() {
        StartCoroutine(getUnityWebRequest());
    }

    string fixJson(string value) {
        value = "{\"Items\":" + value + "}";
        return value;
    }
}

[Serializable]
public class DataMuse {
    public string word;
    public int score;
    public int numSyllables;
    public string tags;

    public static DataMuse CreateFromJSON(string jsonString) {
        return JsonUtility.FromJson<DataMuse>(jsonString);
    }

}
public class Tag {
    public string pron;
    public string ipa_pron;
}
