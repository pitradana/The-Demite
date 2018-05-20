using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Networking;
using System.Text;
using System;
using System.Linq;

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
            string word = wordData[0].word;
            int score = wordData[0].score;
            int syllables = wordData[0].numSyllables;
            string[] pronFull = wordData[0].tags[0].Split(':');
            string pron = pronFull[0];

            string[] ipaFull = wordData[0].tags[1].Split(':');
            string ipa = ipaFull[1];
            int subKategori0Result = subKategori0(Field.text);

            TextBox.text = "Word: "+word+"\nIPA: "+ipa+"\nSyllables: "+syllables+"\nSub Kategori 0: "+subKategori0Result;


        }
    }

    public void CopyText() {
        StartCoroutine(getUnityWebRequest());
    }

    string fixJson(string value) {
        value = "{\"Items\":" + value + "}";
        return value;
    }

    int subKategori0(string word) {
        var count = word.Count(x => x == 'f' || x == 'l' || x == 'm' || x == 'n' || x == 'o' || x == 'q' || x == 's' || x == 'x');
        return count;
    }
}

[Serializable]
public class DataMuse {
    public string word;
    public int score;
    public int numSyllables;
    public string[] tags;

    public static DataMuse CreateFromJSON(string jsonString) {
        return JsonUtility.FromJson<DataMuse>(jsonString);
    }

}
