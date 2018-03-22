﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CymaticLabs.Unity3D.Amqp;

public class MapController : MonoBehaviour {


    //untuk mengecek koneksi dan respon
    private bool mapAcquiredAndProcessed;

    public string uniqueId;

    //properti untuk player
    private string playerName;

    private float latitude;
    private float longitude;
    private float lastLatitude;
    private float lastLongitude;

    //properti untuk hantu
    private string petName;

    private float petPosX;
    private float petPosY;

    private GameObject mainCam;
    private GameObject petObject;
    private GameObject GhostObject;

    public float centerPosX;
    public float centerPosY;

    public int tileX;
    public int tileY;

    List<OtherPlayerData> otherPlayerDataList;

    private bool firstStart = false;
    private bool firstPet = true;
    public bool okToSentPetPos = false;

    private GameObject routeObject;

    Text posText;

	// Use this for initialization
	void Start ()
    {
        AmqpControllerScript.amqpControl.exchangeSubscription.Handler = CheckAndProcessResponse;

        otherPlayerDataList = new List<OtherPlayerData>();

        mainCam = GameObject.FindGameObjectWithTag("MainCamera");
        petObject = GameObject.Find("Cube");

        //routeObject = GameObject.Find("RouteButton");
        //routeObject.GetComponent<Button>().onClick.AddListener(StartRouting);

        //posText = GameObject.Find("PosText").GetComponent<Text>();

        this.InitDefaultProperties();
        //this.UpdateGpsAndSendRequest();
        StartCoroutine(this.StartGPS());
        
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (mapAcquiredAndProcessed)
        {
            this.UpdateGpsAndSendRequest();
        }

        //OtherPetMovement();
    }

    void InitDefaultProperties()
    {
        this.uniqueId = Guid.NewGuid().ToString();
        this.playerName = PlayerPrefs.GetString("username");
        this.latitude = -6.890439f; //-6.915108f; //-6.890439, 107.611256
        this.longitude = 107.611256f; //107.607206f;
        this.lastLatitude = float.MinValue;
        this.lastLongitude = float.MinValue;
        this.petName = "pocong"; // PlayerPrefs.GetString("petName");
        this.petPosX = 0f;
        this.petPosY = 0f;

        this.centerPosX = 0f;
        this.centerPosY = 0f;
        this.tileX = 0;
        this.tileY = 0;

        TextMesh textMest = petObject.GetComponentInChildren<TextMesh>();
        textMest.text = "pocong"; //PlayerPrefs.GetString("petName");

        this.mapAcquiredAndProcessed = false;
        this.firstStart = true;
        this.firstPet = true;
    }

    IEnumerator StartGPS()
    {
        if (!Input.location.isEnabledByUser)
        {
            print("GPS not enabled");
            yield break;
        }

        Input.location.Start(1f, .1f);

        int maxWait = 20;

        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait < 1)
        {
            print("Timed Out");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            print("Unable to determine location");
            yield break;
        }

        else
        {
            this.UpdateGpsAndSendRequest();
        }
    }

    void UpdateGpsAndSendRequest()
    {
        this.latitude = Input.location.lastData.latitude;
        this.longitude = Input.location.lastData.longitude;
        //posText.text = "lat= " + this.latitude + " --- Long=" + this.longitude;

        if (this.lastLatitude != this.latitude || this.lastLongitude != this.longitude)
        {
            this.lastLatitude = this.latitude;
            this.lastLongitude = this.longitude;

            Debug.Log("publish map");

            string requestJSON = this.CreateJsonMassage("map");
            AmqpClient.Publish(AmqpControllerScript.amqpControl.requestExchange, AmqpControllerScript.amqpControl.requestRoutingKey, requestJSON);

            this.mapAcquiredAndProcessed = false;
        }
    }

    //void StartRouting()
    //{
    //    GameObject textObject = GameObject.Find("DestinationFIeld");
    //    string destination = textObject.GetComponent<InputField>().text;

    //    string routeRequestJson = this.CreateRouteJsonMessage("route", destination);
    //    AmqpClient.Publish(AmqpControllerScript.amqpControl.requestExchange, AmqpControllerScript.amqpControl.requestRoutingKey, routeRequestJson);
    //}

    void CheckAndProcessResponse(AmqpExchangeReceivedMessage received)
    {
        var receivedJson = System.Text.Encoding.UTF8.GetString(received.Message.Body);
        var msg = CymaticLabs.Unity3D.Amqp.SimpleJSON.JSON.Parse(receivedJson);

        Debug.Log(msg);

        if(msg != null)
        {
            Debug.Log("msg masuk = " + (string)msg["type"]);

            string id = (string)msg["id"];

             if (id == this.uniqueId) // check for guid or unique id
            {
                string responseType = (string)msg["type"];
                if (responseType == "map") // response for create map
                {
                    //this.DestroyGameObjectByTagName("MapObject");
                    bool createNewMap = (bool)msg["needCreateMap"];
                    if (createNewMap)
                    {
                        Debug.Log("CREATE MAP");
                        this.DestroyGameObjectByTagName("MapObject");

                        var buildingData = msg["mapData"]["listBuildingData"];
                        this.CreateBuilding(buildingData);
                        var roadData = msg["mapData"]["listRoadData"];
                        this.CreateRoad(roadData);
                    }else
                    {
                        if (firstStart)
                        {
                            Debug.Log("FIRST START CREATE MAP");
                            this.DestroyGameObjectByTagName("MapObject");

                            var buildingData = msg["mapData"]["listBuildingData"];
                            this.CreateBuilding(buildingData);
                            var roadData = msg["mapData"]["listRoadData"];
                            this.CreateRoad(roadData);

                            firstStart = false;
                        }
                    }

                    Vector3 tempCamPos = this.mainCam.transform.position;
                    this.mainCam.transform.parent.position = new Vector3((float)msg["playerPosX"], tempCamPos.y, (float)msg["playerPosY"]);

                    if (firstPet)
                    {
                        //Debug.Log(lastLatitude + " & " + lastLongitude);
                        petObject.transform.position = new Vector3((float)msg["playerPosX"], 0, (float)msg["playerPosY"]);
                        firstPet = false;
                    }

                    this.centerPosX = (float)msg["centerPosX"];
                    this.centerPosY = (float)msg["centerPosY"];
                    this.tileX = (int)msg["tileX"];
                    this.tileY = (int)msg["tileY"];

                    //Debug.Log(centerPosX +" -------  "+centerPosY);

                    this.okToSentPetPos = true;
                    this.mapAcquiredAndProcessed = true;

                    //AmqpController.amqpControl.msg = null;
                }

                if (responseType == "route") // response for create route
                {
                    Debug.Log("CREATE ROUTE");
                    this.DestroyGameObjectByTagName("RouteObject");
                    CreateRoute(msg["route"]);

                    //AmqpController.amqpControl.msg = null;
                }

                if (responseType == "maptohome")
                {
                    Debug.Log("MASUK MAP TO HOME");

                    if ((int)msg["result"] == 1)
                    {
                        Time.timeScale = 1;
                        SceneManager.LoadScene("MainScene");
                    }
                }
            }
            
            if ((string)msg["type"] == "listplayer")
            {
                //Debug.Log("listplayer");
                if ((int)msg["tileX"] == this.tileX && (int)msg["tileY"] == this.tileY)
                {
                    RemoveOtherFromList(msg["unityPlayerPos"]);
                    UpdateOthersPosition(msg["unityPlayerPos"]);
                }
            }
        } 
    }

    void RemoveOtherFromList(CymaticLabs.Unity3D.Amqp.SimpleJSON.JSONNode data )
    {
        bool found = false;
        List<OtherPlayerData> removeList = new List<OtherPlayerData>();

        for(int i=0; i <otherPlayerDataList.Count; i++)
        {
            OtherPlayerData other = otherPlayerDataList[i];

            for(int j=0; j<data.Count; j++)
            {
                string otherName = (string)data[j]["playerName"];

                Debug.Log("list name = " + other.playerName + " data list name = " + otherName);

                if(other.playerName == otherName)
                {
                    Debug.Log("sama");

                    found = true;
                    break;
                }
            }

            if (!found)
            {
                removeList.Add(other);
            }

            found = false;
        }

        Debug.Log("Remove Count = " + removeList.Count);

        for(int i = 0; i<removeList.Count; i++)
        {
            OtherPlayerData remove = removeList[i];

            GameObject otherPlayer = GameObject.Find(remove.playerName);
            GameObject otherGhostPlayer = GameObject.Find(remove.playerName + "_" + remove.petName);
            GameObject food = GameObject.Find("food_" + remove.playerName);

            if (otherPlayer != (GameObject)null)
            {
                Destroy(otherPlayer);
            }

            if(otherGhostPlayer != (GameObject)null)
            {
                Destroy(otherGhostPlayer);
            }

            if (food != (GameObject)null)
            {
                Destroy(food);
            }

            otherPlayerDataList.Remove(remove);
        }

        Debug.Log("Finished Check Delete");
    }

    void UpdateOthersPosition(CymaticLabs.Unity3D.Amqp.SimpleJSON.JSONNode data)
    {
        Debug.Log("list player responses");

        for (int i =0; i< data.Count; i++)
        {
            string otherUsername = (string)data[i]["playerName"];
            float otherPosX = (float)data[i]["posX"];
            float otherPosY = (float)data[i]["posY"];

            string otherGhostName = (string)data[i]["petName"];
            float otherGhostPosX = (float)data[i]["petPosX"];
            float otherGhostPosY = (float)data[i]["petPosY"];
            float otherGhostLastPosX = (float)data[i]["petLastPosX"];
            float otherGhostLastPosY = (float)data[i]["petLastPosY"];

            string otherStartTImeMoveString = (string)data[i]["timeStartMove"];
            long otherStartTimeMove = 0L;

            if (otherStartTImeMoveString != "")
            {
                otherStartTimeMove = Convert.ToInt64(otherStartTImeMoveString);
            }

            string otherGhostState = (string)data[i]["petState"];
            float otherPetSpeed = (float)data[i]["petSpeed"];

            Debug.Log("main player = " + this.playerName);
            Debug.Log("other player = " + otherUsername);

            if(otherUsername != this.playerName)
            {
                OtherPlayerData other = otherPlayerDataList.Find(x => x.playerName == otherUsername);

                if(other == null)
                {
                    OtherPlayerData newOtherPlayer = new OtherPlayerData();
                    newOtherPlayer.playerName = otherUsername;
                    newOtherPlayer.posX = otherPosX - this.centerPosX;
                    newOtherPlayer.posY = otherPosY - this.centerPosY;
                    newOtherPlayer.petName = otherGhostName;
                    newOtherPlayer.petState = otherGhostState;

                    float mapGhostLastPosX = otherGhostLastPosX - this.centerPosX;
                    float mapGhostLastPosY = otherGhostLastPosY - this.centerPosY;
                    float mapGhostPosX = otherGhostPosX - this.centerPosX;
                    float mapGhostPosY = otherGhostPosY - this.centerPosY;

                    Vector3 mapGhostLastPos = new Vector3(mapGhostLastPosX, 0.0f, mapGhostLastPosY);
                    Vector3 mapGhostPos = new Vector3(mapGhostPosX, 0.0f, mapGhostPosX);
                    float totalDistance = Vector3.Distance(mapGhostLastPos, mapGhostPos);
                    float distanceTraveled = 0.0f;

                    //double seconds = (DateTime.Now - new DateTime(otherStartTimeMove)).TotalSeconds;
                    //Debug.Log(seconds);
                    //float distanceTraveled = (float)(seconds * 1f);

                    if (otherStartTimeMove != 0L)
                    {
                        double seconds = (DateTime.Now - new DateTime(otherStartTimeMove)).TotalSeconds;
                        distanceTraveled = (float)(seconds * otherPetSpeed);
                    }

                    Debug.Log(distanceTraveled + " --- distance traveled");
                    Debug.Log(totalDistance + " --- total distance");
                    //Debug.Log(seconds + "  --- time seconds");

                    Vector3 predictedPos;

                    if (distanceTraveled >= totalDistance)
                    {
                        newOtherPlayer.petFromPosX = mapGhostLastPosX;
                        newOtherPlayer.petFromPosY = mapGhostLastPosY;
                        newOtherPlayer.petToPosX = mapGhostPosX;
                        newOtherPlayer.petToPosY = mapGhostPosY;

                        predictedPos = new Vector3(mapGhostPosX, 0.0f, mapGhostPosY);
                    }
                    else
                    {
                        float percentage = distanceTraveled / totalDistance;

                        predictedPos = Vector3.Lerp(mapGhostLastPos, mapGhostPos, percentage);
                        newOtherPlayer.petFromPosX = predictedPos.x;
                        newOtherPlayer.petFromPosY = predictedPos.z;
                        newOtherPlayer.petToPosX = mapGhostPosX;
                        newOtherPlayer.petToPosY = mapGhostPosY;
                    }

                    newOtherPlayer.petSpeed = otherPetSpeed;

                    otherPlayerDataList.Add(newOtherPlayer);

                    //jika belum ada nama pet
                    GameObject newGhostObject = Instantiate(Resources.Load("PetPrefab")) as GameObject;
                    newGhostObject.name = "pet_" + otherGhostName;
                    newGhostObject.transform.position = new Vector3(predictedPos.x, 0.0f, predictedPos.z);

                    GameObject namaGhost = newGhostObject.transform.Find("PetNameText").gameObject;
                    TextMesh ghostNameMesh = namaGhost.GetComponent<TextMesh>();

                    ghostNameMesh.text = "<pet>\n" + otherGhostName;
                    ghostNameMesh.characterSize = 0.05f;
                    ghostNameMesh.fontSize = 100;
                    ghostNameMesh.color = Color.green;

                    Font font = Resources.Load<Font>("Font/SHADSER");
                    ghostNameMesh.font = font;
                    var mr = ghostNameMesh.GetComponent<Renderer>();
                    mr.material = font.material;

                    GameObject otherPlayerNameObject = new GameObject();
                    otherPlayerNameObject.AddComponent<NameController>();
                    otherPlayerNameObject.name = otherUsername;
                    otherPlayerNameObject.transform.position = new Vector3(otherPosX - this.centerPosX, 10.0f, this.centerPosY);

                    var meshText = otherPlayerNameObject.AddComponent<TextMesh>() as TextMesh;
                    meshText.text = "<user>\n" + otherUsername;
                    meshText.characterSize = 0.05f;
                    meshText.fontSize = 100;
                    meshText.color = Color.green;

                    Font font1 = Resources.Load<Font>("Font/SHADSER");
                    meshText.font = font1;
                    var mr1 = meshText.GetComponent<Renderer>();
                    mr1.material = font1.material;
                }
                else
                {
                    other.petToPosX = otherGhostPosX - this.centerPosX;
                    other.petToPosY = otherGhostPosY - this.centerPosY;
                    other.petState = otherGhostState;
                    other.petSpeed = otherPetSpeed;

                    if(other.petState == "walkFood")
                    {
                        GameObject food = GameObject.CreatePrimitive(PrimitiveType.Cube);

                        food.name = "food_" + other.playerName;
                        food.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                        food.transform.position = new Vector3(other.petToPosX, 0, other.petToPosY);
                    }

                    GameObject otherPlayer = GameObject.Find(otherUsername);
                    otherPlayer.transform.position = new Vector3(otherPosX - this.centerPosX, 10.0f, otherPosY - centerPosY);
                }
            }
        }
    }

    void OtherPetMovement()
    {
        for( int i=0; i<otherPlayerDataList.Count; i++)
        {

            OtherPlayerData player = (OtherPlayerData)otherPlayerDataList[i];

            if(player.playerName != this.playerName)
            {
                if(player.petState =="walk" || player.petState == "walkFood")
                {
                    GameObject currentGhostObject = GameObject.Find(player.playerName + "_" + player.petName);
                    Animator animator = currentGhostObject.GetComponent<Animator>();
                    animator.runtimeAnimatorController = Resources.Load("AnimalController/PetWalkController") as RuntimeAnimatorController;

                    Vector3 lookpos = new Vector3(player.petToPosX, 0.0f, player.petToPosY) - currentGhostObject.transform.position;
                    lookpos.y = 0;
                    if (lookpos != Vector3.zero)
                    {
                        var rotation = Quaternion.LookRotation(lookpos);
                        currentGhostObject.transform.rotation = Quaternion.Slerp(currentGhostObject.transform.rotation, rotation, Time.deltaTime * 2.0f);
                    }

                    currentGhostObject.transform.position = Vector3.MoveTowards(currentGhostObject.transform.position, new Vector3(player.petToPosX, 0.0f, player.petToPosY), Time.deltaTime * 1.0f);
                }
                else if(player.petState == "eatfood")
                {
                    Animator animator = GameObject.Find(player.playerName + "_" + player.petName).GetComponent<Animator>();
                    animator.runtimeAnimatorController = Resources.Load("AnimatorController/GhostEatController") as RuntimeAnimatorController;

                    GameObject food = GameObject.Find("food_" + player.playerName);
                    if(food != (GameObject)null)
                    {
                        Destroy(food);
                    }
                }
            }
        }
    }

    void CreateRoute(CymaticLabs.Unity3D.Amqp.SimpleJSON.JSONNode routeData)
    {
        for (int i = 0; i < routeData.Count - 1; i++)
        {
            float startLatitude = (float)routeData[i]["latitude"];
            float startLongitude = (float)routeData[i]["longitude"];
            Vector3 start = new Vector3(startLatitude, 0.0f, startLongitude);

            float endLatitude = (float)routeData[i + 1]["latitude"];
            float endLongitude = (float)routeData[i + 1]["longitude"];
            Vector3 end = new Vector3(endLatitude, 0.0f, endLongitude);

            CreateRoadWaterMesh(start, end, 8.0f, "RouteObject", Color.green, "route");
        }
    }

    void CreateBuilding(CymaticLabs.Unity3D.Amqp.SimpleJSON.JSONNode buildingData)
    {
        List<Vector3> point = new List<Vector3>();
        List<Vector2> point2D = new List<Vector2>();

        for(int i=0; i<buildingData.Count; i++)
        {
            var tempData = buildingData[i];

            if(tempData["listCoordinate"].Count == 1)
            {
                string buildingName = (string)tempData["buildingName"];

                Debug.Log("nama gedung ini adalah  "+buildingName);

                if(buildingName != null)
                {
                    var coordinate = tempData["listCoordinate"][0];                  
                    this.ShowName(new Vector3((float)coordinate["latitude"], 12f, (float)coordinate["longitude"]), new Vector3(), buildingName, "MapObject", "buildingName", Color.green);

                    //this.ShowGhost(new Vector3((float)coordinate["latitude"], 10f, (float)coordinate["longitude"]), buildingName, "buildingName");
                    //Debug.Log();
                    //petObject.transform.position = new Vector3((float)coordinate["latitude"], 12f, (float)coordinate["longitude"]);
                    //Debug.Log("pocong = " + petObject.transform.position);
                }
            }
            else
            {
                for (int j=0; j< tempData["listCoordinate"].Count-1; j++)
                {
                    var coordinate = tempData["listCoordinate"][j];
                    float latitude = (float)coordinate["latitude"];
                    float longitude = (float)coordinate["longitude"];
                    point.Add(new Vector3(latitude, 0.0f, longitude));
                    point2D.Add(new Vector2(latitude, longitude));
                }

               // this.CreatePolygon(point2D.ToArray(), point.ToArray(), Color.green, "MapObject", "building");
                point.Clear();
                point2D.Clear();
            }
        }
    }

    void CreateRoad(CymaticLabs.Unity3D.Amqp.SimpleJSON.JSONNode roadData)
    {
        List<Vector3[]> point = new List<Vector3[]>();
        List<string> names = new List<string>();

        for (int i = 0; i < roadData.Count; i++)
        {
            var tempData = roadData[i];
            Vector3[] perPoint = new Vector3[tempData["listCoordinate"].Count];

            for (int k = 0; k < tempData["listCoordinate"].Count; k++)
            {
                var coordinate = tempData["listCoordinate"][k];
                float latitude = (float)coordinate["latitude"];
                float longitude = (float)coordinate["longitude"];
                Debug.Log(latitude + " & " + longitude);
                perPoint[k] = new Vector3(latitude, 8f, longitude);
            }

            point.Add(perPoint);
            names.Add((string)tempData["roadName"]);
        }

        float distance = float.MaxValue;
        Vector3 startNamePos = Vector3.zero;
        Vector3 endNamePos = Vector3.zero;

        for (int l = 0; l < point.Count; l++)
        {
            string roadName = names[l];
            Vector3[] tempVector = point[l];

            for (int m = 0; m < tempVector.Length - 1; m++)
            {
                //CreateRoadWaterMesh(tempVector[m], tempVector[m + 1], 2.0f, "MapObject", Color.red, "road");

                Vector3 tempNamePost = Vector3.Lerp(tempVector[m], tempVector[m + 1], 0.5f);
                float tempDistance = Vector3.Distance(this.mainCam.transform.parent.position, tempNamePost);

                if (tempDistance < distance)
                {
                    distance = tempDistance;
                    startNamePos = tempVector[m];
                    endNamePos = tempVector[m + 1];
                }
            }

            if (roadName != null)
            {
                Debug.Log(roadName);
                this.ShowName(startNamePos, endNamePos, roadName, "MapObject", "roadName", Color.green);
            }

            distance = float.MaxValue;
        }
    }

    void CreatePolygon(Vector2[] point2D, Vector3[] points, Color color, string tagName, string typeName)
    {
        Triangulator triangulator = new Triangulator(point2D);
        int[] indices = triangulator.Triangulate();

        GameObject gameObject = new GameObject(typeName + "_mesh");
        gameObject.tag = tagName;
        var mf = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        var mr = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        mr.material.color = color;
        mr.material.shader = Shader.Find("GUI/Text Shader");

        Mesh m = new Mesh();
        m.Clear();
        m.vertices = points;
        m.triangles = indices;
        mf.mesh = m;

        m.RecalculateBounds();
        m.RecalculateNormals();
    }

    void CreateRoadWaterMesh(Vector3 start, Vector3 end, float lineWidth, string tagName, Color color, string typeName)
    {
        Vector3 normal = Vector3.Cross(start, end);
        Vector3 side = Vector3.Cross(normal, end - start);

        side.Normalize();

        Vector3 a = start + side * (lineWidth / 2);
        Vector3 b = start + side * (lineWidth / -2);
        Vector3 c = end + side * (lineWidth / 2);
        Vector3 d = end + side * (lineWidth / -2);

        Vector3[] points = new Vector3[] { a, b, c, d };

        GameObject gameObject = new GameObject(typeName + "_mesh");
        gameObject.tag = tagName;
        var mf = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        var mr = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;

        mr.material.color = color;
        mr.material.shader = Shader.Find("GUI/Text Shader");

        Mesh m = new Mesh();
        m.Clear();
        m.vertices = points;
        m.triangles = new int[] { 0, 2, 1, 2, 3, 1 };

        if (normal.y > 0)
        {
            mf.mesh = RevertNormals(m);
        }
        else
        {
            mf.mesh = m;
        }

        m.RecalculateBounds();
        m.RecalculateNormals();
    }

    void ShowName(Vector3 textPosStart, Vector3 textPostEnd, string objectName, string tagName, string typeName, Color color)
    {
        GameObject gameObject = new GameObject(typeName + "_mesh");
        gameObject.tag = tagName;

        var text = gameObject.AddComponent<TextMesh>();
        text.color = color;
        text.characterSize = 0.05f;
        text.fontSize = 100;
        text.fontStyle = FontStyle.Italic;
        text.anchor = TextAnchor.MiddleCenter;
        text.alignment = TextAlignment.Center;
        Font font = Resources.Load<Font>("Font/SHADSER");
        text.font = font;
        var mr = text.GetComponent<Renderer>();
        mr.material = font.material;

        if (typeName == "roadName")
        {
            text.text = "<road>\n" + objectName;
            gameObject.transform.position = Vector3.Lerp(textPosStart, textPostEnd, 0.5f);
            gameObject.AddComponent<NameController>();
        }
        else if(typeName == "buildingName" || typeName == "poiName")
        {
            text.text = "<building/poi>\n" + objectName;
            gameObject.transform.position = textPosStart;
            gameObject.AddComponent<NameController>();
        }
    }

    void ShowGhost(Vector3 textPosStart, string objectName, string typeName)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        if(typeName == "buildingName" || typeName == "poiName")
        {
            cube.transform.position = textPosStart;
        }

        //var Ghost = gameObject.AddComponent<Mes>
    }

    Mesh RevertNormals(Mesh mesh)
    {
        Vector3[] normals = mesh.normals;
        for (int i = 0; i < normals.Length; i++)
            normals[i] = -normals[i];
        mesh.normals = normals;

        for(int j=0; j < mesh.subMeshCount; j++)
        {
            int[] triangles = mesh.GetTriangles(j);

            for(int k = 0; k < triangles.Length; k +=3)
            {
                int temp = triangles[k + 0];
                triangles[k + 0] = triangles[k + 1];
                triangles[k + 1] = temp;
            }

            mesh.SetTriangles(triangles, j);
        }

        return mesh;
    }

    void DestroyGameObjectByTagName(string tagName)
    {
        Debug.Log("Destroy Game Object");
        GameObject[] arrayGameObject = GameObject.FindGameObjectsWithTag(tagName);
        for(int i = 0 ; i < arrayGameObject.Length; i++)
        {
            Destroy(arrayGameObject[i]);
        }
    }

    public string CreateJsonMassage(string type)
    {
        RequestJSON requestJson = new RequestJSON();
        requestJson.id = this.uniqueId;
        requestJson.type = type;
        requestJson.playerName = this.playerName;
        requestJson.latitude = this.latitude;
        requestJson.longitude = this.longitude;
        requestJson.petName = this.petName;
        requestJson.petPosX = this.petPosX;
        requestJson.petPosY = this.petPosY;

        return JsonUtility.ToJson(requestJson);
    }

    public string CreateRouteJsonMessage(string type, string destination)
    {
        RouteRequestJson routeRequestJson = new RouteRequestJson();
        routeRequestJson.id = this.uniqueId;
        routeRequestJson.username = this.playerName;
        routeRequestJson.type = type;
        routeRequestJson.latitude = this.latitude;
        routeRequestJson.longitude = this.longitude;
        routeRequestJson.destination = destination;

        return JsonUtility.ToJson(routeRequestJson);
    }

    [Serializable]
    public class RequestJSON
    {
        public string id;
        public string type;
        public string playerName;
        public float latitude;
        public float longitude;
        public string petName;
        public float petPosX;
        public float petPosY;
    }

    [Serializable]
    public class RouteRequestJson
    {
        public string id;
        public string username;
        public string type;
        public float latitude;
        public float longitude;
        public string destination;
    }

    public class OtherPlayerData
    {
        public string playerName;
        public float posX;
        public float posY;
        public string petName;
        public float petToPosX;
        public float petToPosY;
        public float petFromPosX;
        public float petFromPosY;
        public string petState;
        public float petSpeed;
    }
}
