using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CopyText : MonoBehaviour {
    public InputField Field;
    public Text TextBox;

    public void CopyTextFromInput() {
        TextBox.text = Field.text;
    }
}
