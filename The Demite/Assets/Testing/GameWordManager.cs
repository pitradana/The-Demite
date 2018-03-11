using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class GameWordManager : MonoBehaviour {

    public Question[] word;
    private static List<Question> unansweredQuestion;

    public Question currentQuestion;

    GameObject pocong;
    public Text kataVoice;
    
    [SerializeField]
    private Text fact;

	// Use this for initialization
	void Start ()
    {
	    if (unansweredQuestion == null || unansweredQuestion.Count == 0)
        {
            unansweredQuestion = word.ToList<Question>();
        }

        pocong = GameObject.FindGameObjectWithTag("pocong");
        //kataVoice = GameObject.Find("Textv").GetComponent<Text>();

        SetRandomQuestion();
        //Debug.Log(currentQuestion.fact + " = " + currentQuestion.isTrue);
	}

    public void SetRandomQuestion()
    {
        int randomQuestionIndex = Random.Range(0, unansweredQuestion.Count);
        currentQuestion = unansweredQuestion[randomQuestionIndex];

        fact.text = currentQuestion.fact;

        unansweredQuestion.RemoveAt(randomQuestionIndex);
    }

    public void UserSelectTrue()
    {
        //string voice = kataVoice.text.ToString();

        if(fact.text == kataVoice.text)
        {
            Destroy(pocong);
            Debug.Log("BENAR COY!");
        }
        else
        {
            Debug.Log("SALAH BRO!");
        }
    }

    public void UserSelectFalse()
    {
        
        //kataVoice.text = "catch";
        Debug.Log("catch");

        pocong.SetActive(true);
    }

    void Update()
    {
        if (fact.text == kataVoice.text)
        {
            Destroy(pocong);
            Debug.Log("BENAR COY!");
        }
    }
}
