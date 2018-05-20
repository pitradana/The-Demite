using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputText : MonoBehaviour {

    public InputField inputField;

    void Awake() {
        inputField.onEndEdit.AddListener(AcceptStringInput);
    }

	void AcceptStringInput(string userInput) {
        userInput = userInput.ToLower();
    }
}
