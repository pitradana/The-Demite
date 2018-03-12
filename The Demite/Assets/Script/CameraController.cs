using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {

    //private GameObject camParent;
    //private GameObject plane;
    private Quaternion rotationQua;
    //private WebCamTexture webcamTexture;
    public float maxRayDistance = 100f;
    //public bool canHover = false;
    public Text ahh;

    private Gyroscope gyro;
    private bool gyroSupported;
    private Quaternion rotFix;

    public bool canOver = false;
    
    
    

    //gameObject.GetComponent<move>().speed

    [SerializeField]
    private Transform worldObject;
    private float startY;
    

    [SerializeField]
    private Transform zoomObj;

    // Use this for initialization
    void Start ()
    {
        //camParent = new GameObject("CameraParent");
        //camParent.transform.position = this.transform.position;
        //this.transform.parent = camParent.transform;
        //camParent.transform.rotation = Quaternion.Euler(90f, 180f, 0f);
        //rotationQua = new Quaternion(0, 0, 1, 0);
        //Input.gyro.enabled = true;

        //plane = GameObject.Find("Plane");

        //float pos = (Camera.main.nearClipPlane + 500f);
        //plane.transform.position = Camera.main.transform.position + Camera.main.transform.forward * pos;
        //float h = (Mathf.Tan(Camera.main.fieldOfView * Mathf.Deg2Rad * 0.5f) * pos * 2f) / 10.0f;
        //plane.transform.localScale = new Vector3(h * Camera.main.aspect, 1.0f, h);

        ////float ratio = (float)webcamTexture.width / (float)webcamTexture.height;
        ////fit.aspectRatio = ratio;

        //if (webcamTexture == null)
        //{
        //    webcamTexture = new WebCamTexture();
        //    plane.GetComponent<MeshRenderer>().material.mainTexture = webcamTexture;
        //}

        //webcamTexture.Play();

        

        gyroSupported = SystemInfo.supportsGyroscope;

        GameObject camParent = new GameObject("camParent");
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
        //this.transform.localRotation = Quaternion.Slerp(this.transform.localRotation, Input.gyro.attitude * rotationQua, Time.deltaTime * 2.0f);
        
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        Debug.DrawLine(transform.position, transform.position + transform.forward * maxRayDistance, Color.red);

        if (Physics.Raycast(ray, out hit, maxRayDistance))
        {
<<<<<<< HEAD
            canOver = true;
            Debug.DrawLine(hit.point, hit.point + transform.up * 10, Color.green);
            Debug.Log("kena");
            ahh.text = "AH yes";
           
=======
            Debug.DrawLine(hit.point, hit.point + transform.up * 10, Color.green);
            ahh.text = "AH yes";
>>>>>>> 1c562862582d80634b41f51eb1086080078c4a82
        }
        else
        {
            canOver = false;
            Debug.Log("tidak kena");
            ahh.text = "tidak dapat";
        }

        if (gyroSupported && startY == 0)
        {
            ResetGyroRotation();
        }
        //transform.localRotation = gyro.attitude * rotFix;
    }

    

    //private void OnGUI()
    //{
    //    if (canOver==true)
    //    {
    //        GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 150, 20), "ilangin aku");
    //    }
    //}

    void ResetGyroRotation()
    {
        int x = Screen.width / 2;
        int y = Screen.height / 2;

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(x, y));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 500))
        {
            Vector3 hitPoint = hit.point;
            hitPoint.y = 0;

            float z = Vector3.Distance(Vector3.zero, hitPoint);
            zoomObj.localPosition = new Vector3(0f, zoomObj.localPosition.y, Mathf.Clamp(z, 2f, 10f));
        }

        startY = transform.eulerAngles.y;
        worldObject.rotation = Quaternion.Euler(0f, startY, 0f);
    }
    
}
