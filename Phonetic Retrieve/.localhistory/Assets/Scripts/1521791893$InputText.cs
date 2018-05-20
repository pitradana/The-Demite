using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputText : MonoBehaviour {

    public InputField inputField;
    public Text phonetic;

    void Awake() {
        inputField.onEndEdit.AddListener(AcceptStringInput);
    }

	void AcceptStringInput(string userInput) {
        userInput = userInput.ToLower();
        phonetic.text = userInput;
    }

    void InputComplete() {
        inputField.ActivateInputField();
        inputField.text = null;
    }
}
