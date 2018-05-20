﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CymaticLabs.Unity3D.Amqp;

public class LoginScript : MonoBehaviour {

    Button login;
    Text warning;
    string id;

	// Use this for initialization
	void Start () {
        id = Guid.NewGuid().ToString();

        warning = GameObject.Find("Warning_Text").GetComponent<Text>();

        login = this.GetComponent<Button>();
        login.onClick.AddListener(LoginGame);
	}
	
	// Update is called once per frame
	void Update () {
		if( Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
	}

    void ProcessLogin(AmqpExchangeReceivedMessage received)
    {
        var receivedJson = System.Text.Encoding.UTF8.GetString(received.Message.Body);
        var msg = CymaticLabs.Unity3D.Amqp.SimpleJSON.JSON.Parse(receivedJson);
        

        if(msg != null)
        {
            string msgId = (string)msg["id"];
            if(msgId == id)
            {
                string type = (string)msg["type"];
                if(type == "login")
                {
                    int count = (int)msg["count"];
                    if(count == 1)
                    {
                        LoadStatus(msg);
                        Time.timeScale = 1;
                        SceneManager.LoadScene("MainScene");
                    }
                    else
                    {
                        warning.text = "username or password not found";
                    }
                }
            }
        }
    }

    void LoginGame()
    {
        warning.text = "";

        InputField usernameField = GameObject.Find("InputField_username").GetComponent<InputField>();
        InputField passwordField = GameObject.Find("InputField_password").GetComponent<InputField>();

        string username = usernameField.text;
        string password = passwordField.text;

        if(username == "")
        {
            warning.text = "username cannot be empty";
        }

        if(password == "")
        {
            warning.text = "password cannot be empty";
        }

        Debug.Log(username + " & " + password);

        if(username != "" && password != "")
        {
            AmqpControllerScript.amqpControl.exchangeSubscription.Handler = ProcessLogin;

            LoginRequestJson request = new LoginRequestJson();
            request.id = id;
            request.type = "login";
            request.username = username;
            request.password = password;

            string requestToJson = JsonUtility.ToJson(request);
            AmqpClient.Publish(AmqpControllerScript.amqpControl.requestExchange, AmqpControllerScript.amqpControl.requestRoutingKey, requestToJson);
        }
    }

    void LoadStatus(CymaticLabs.Unity3D.Amqp.SimpleJSON.JSONNode msg)
    {

        PlayerPrefs.SetString("username", (string)msg["username"]);
        //PlayerPrefs.SetInt("Health", (int)msg["data"][0]);
        //PlayerPrefs.SetInt("Bones", (int)msg["data"][1]);
        //PlayerPrefs.SetInt("XP", (int)msg["data"][2]);
        //PlayerPrefs.SetInt("Sleep", (int)msg["data"][3]);
        //PlayerPrefs.SetInt("Food", (int)msg["data"][4]);
        //PlayerPrefs.SetInt("Walk", (int)msg["data"][5]);
        //PlayerPrefs.SetInt("Fun", (int)msg["data"][6]);
        PlayerPrefs.SetString("petName", (string)msg["username"]);
    }

    [Serializable]
    public class LoginRequestJson
    {
        public string id;
        public string type;
        public string username;
        public string password;
    }
}
