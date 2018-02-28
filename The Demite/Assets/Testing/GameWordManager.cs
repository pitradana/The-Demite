using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class GameWordManager : MonoBehaviour {

    public Question[] question;
    private static List<Question> unansweredQuestion;

    public Question currentQuestion;

    [SerializeField]
    private Text fact;

	// Use this for initialization
	void Start ()
    {
	    if (unansweredQuestion == null || unansweredQuestion.Count == 0)
        {
            unansweredQuestion = question.ToList<Question>();
        }

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
        if(currentQuestion.isTrue)
        {
            Debug.Log("BENAR COY!");
        }
        else
        {
            Debug.Log("SALAH BRO!");
        }
    }

    public void UserSelectFalse()
    {
        if (!currentQuestion.isTrue)
        {
            Debug.Log("BENAR COY!");
        }
        else
        {
            Debug.Log("SALAH BRO!");
        }
    }
}
