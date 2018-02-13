using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CymaticLabs.Unity3D.Amqp;

public class AmqpControllerScript : MonoBehaviour {

    public static AmqpControllerScript amqpControl;

    public bool serverTerhubung;

    //properti untuk koneksi
    public string memintaExchangeName;
    public string memintaRoutingKey;

    public string responExchangeName;
    public string responRoutingKey;

    public AmqpExchangeTypes responExchangeType;

    public AmqpExchangeSubscription exchangeSubscription;

    void Awake()
    {
        if(amqpControl == null)
        {
            DontDestroyOnLoad(this.gameObject);
            amqpControl = this;
        }
        else if(amqpControl != this)
        {
            Destroy(this.gameObject);
        }
    }

    // Use this for initialization
    void Start ()
    {
        serverTerhubung = false;

        memintaExchangeName = "";
        memintaRoutingKey = "";

        responExchangeName = "";
        responRoutingKey = "";
        responExchangeType = AmqpExchangeTypes.Direct;

        AmqpClient.Instance.Connection = "ITB";
        AmqpClient.Connect();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
