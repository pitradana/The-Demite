using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class GameWordManager : MonoBehaviour {

    public Word[] word;
    private static List<Word> unansweredWord;

    private Word currentWord;
    public Text wordFromSpeech;
    GameObject pocong;
    
    [SerializeField]
    private Text wordToSpeech;

    // Use this for initialization
    void Start ()
    {
	    if (unansweredWord == null || unansweredWord.Count == 0)
        {
            unansweredWord = word.ToList<Word>();
        }

        pocong = GameObject.FindGameObjectWithTag("pocong");

        SetRandomQuestion();
	}

    public void SetRandomQuestion()
    {
        int randomQuestionIndex = Random.Range(0, unansweredWord.Count);
        currentWord = unansweredWord[randomQuestionIndex];

        wordToSpeech.text = currentWord.fact;

        unansweredWord.RemoveAt(randomQuestionIndex);
    }

    void Update()
    {
        
        if (wordToSpeech.text == wordFromSpeech.text)
        {
            //for(int i = 0; i<= pocong.Length; i++)
            //{
            //    Destroy(pocong[i]);

            //}

            Destroy(pocong);
            Debug.Log("BENAR COY!");
        }
    }
}
