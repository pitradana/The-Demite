﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{

    private Gyroscope gyro;
    private Quaternion rotFix;
    private bool gyroSupported;

    private float maxRayDistance = 100f;
    public Text debugText;
    public bool canOver = false;

    private GameObject healthBar;

    // Use this for initialization
    void Start ()
    {
        gyroSupported = SystemInfo.supportsGyroscope;

        GameObject camParent = new GameObject("camParent");
        healthBar = GameObject.Find("HealthBar");
        healthBar.SetActive(false);

        camParent.transform.position = transform.position;
        transform.parent = camParent.transform;

        if (gyroSupported)
        {
            gyro = Input.gyro;
            gyro.enabled = true;

            camParent.transform.rotation = Quaternion.Euler(90f, 180f, 0f);
            rotFix = new Quaternion(0, 0, 1, 0);
        }

    }

    // Update is called once per frame
    void Update()
    {

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        Debug.DrawLine(transform.position, transform.position + transform.forward * maxRayDistance, Color.red);

        if (Physics.Raycast(ray, out hit, maxRayDistance))
        {
<<<<<<< HEAD

=======
>>>>>>> f50e1c8f08de09701cd6dae65fe0c796193c2128
            canOver = true;

            healthBar.SetActive(true);
            //Debug.DrawLine(hit.point, hit.point + transform.up * 10, Color.green);
            Debug.Log("kena");
<<<<<<< HEAD
            ahh.text = "AH yes";
           

            Debug.DrawLine(hit.point, hit.point + transform.up * 10, Color.green);
            ahh.text = "AH yes";

=======
            debugText.text = "kena";
>>>>>>> f50e1c8f08de09701cd6dae65fe0c796193c2128
        }
        else
        {
            canOver = false;
            Debug.Log("tidak kena");
            debugText.text = "tidak kena";
            healthBar.SetActive(false);
        }

        transform.localRotation = gyro.attitude * rotFix;

    }
}
