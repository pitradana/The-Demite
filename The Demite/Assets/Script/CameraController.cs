using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {

    private GameObject camParent;
    private GameObject plane;
    private Quaternion rotationQua;
    private WebCamTexture webcamTexture;
    public float maxRayDistance = 100;
    public bool canHover = false;
    public Text ahh;

    //public AspectRatioFitter fit;

    // Use this for initialization
    void Start ()
    {
        camParent = new GameObject("CameraParent");
        camParent.transform.position = this.transform.position;
        this.transform.parent = camParent.transform;
        camParent.transform.rotation = Quaternion.Euler(90f, 180, 0f);
        rotationQua = new Quaternion(0, 0, 1, 0);
        Input.gyro.enabled = true;

        plane = GameObject.Find("Plane");

        float pos = (Camera.main.nearClipPlane + 500f);
        plane.transform.position = Camera.main.transform.position + Camera.main.transform.forward * pos;
        float h = (Mathf.Tan(Camera.main.fieldOfView * Mathf.Deg2Rad * 0.5f) * pos * 2f) / 10.0f;
        plane.transform.localScale = new Vector3(h * Camera.main.aspect, 1.0f, h);

        //float ratio = (float)webcamTexture.width / (float)webcamTexture.height;
        //fit.aspectRatio = ratio;

        if (webcamTexture == null)
        {
            webcamTexture = new WebCamTexture();
            plane.GetComponent<MeshRenderer>().material.mainTexture = webcamTexture;
        }

        webcamTexture.Play();
	}
	
	// Update is called once per frame
	void Update ()
    {
        this.transform.localRotation = Quaternion.Slerp(this.transform.localRotation, Input.gyro.attitude * rotationQua, Time.deltaTime * 2.0f);

        Ray ray = new Ray(transform.position, Vector3.forward);
        RaycastHit hit;

        Debug.DrawLine(transform.position, transform.position + Vector3.fwd * maxRayDistance, Color.red);

        if (Physics.Raycast(ray, out hit, maxRayDistance))
        {
            //if (hit.distance <= 10.0 && hit.collider.gameObject.tag == "pocong" )
            //{
            //    Debug.Log("YES");
            //}
            //if(hit.collider.)
            Debug.DrawLine(hit.point, hit.point + Vector3.up * 5, Color.green);
            Debug.Log("jancuk");
            ahh.text = "AH yes";
            //if (hit.collider == )
            //canHover = true;

           
        }

        else
        {
            Debug.Log("tidak kena");
            ahh.text = "tidak dapat";
        }
    }
}
