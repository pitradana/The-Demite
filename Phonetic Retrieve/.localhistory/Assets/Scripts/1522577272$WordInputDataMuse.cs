﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordInputDataMuse : MonoBehaviour {
    public InputField Field;
    public Text TextBox;


    public void CopyText() {
        TextBox.text = Field.text;
    }
}
