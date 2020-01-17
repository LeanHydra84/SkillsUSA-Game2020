using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

public static class PlayerState
{
    static IEnumerator healthDelay()
    {
        canLose = false;
        yield return new WaitForSeconds(0.5f);
        canLose = true;
    }

    static int keys;
    static int health;
    static float time;
    static int seconds;
    static bool canLose;
    static int ammo;


    public static float[] position;
    public static float x;
    public static float y;
    public static float z;

    static PlayerState()
    {
        position = new float[2];
        keys = 0;
        health = 4;
        time = 0f;
        seconds = 0;
        canLose = true;
        ammo = 0;
        IsInBossFight = true; //SHOULD NOT BE TRUE
    }

    public static int Ammo
    {
        get => ammo;
        set { if (value < 8) ammo = value; }
    }

    public static bool IsInBossFight { get; set; }

    public static int Health
    {
        get => health;
        set
        {
            if (canLose && (value >= 0 && value < 5))
            {
                health = value;
                mainScript.instance.StartCoroutine(healthDelay());
            }
        }
    }

    public static int Keys
    {
        get => keys;
        set { if (value > keys || value == 0) keys = value; }
    }

    public static float Time
    {
        get => time;
        set
        {
            seconds = (int)time;
            if (value > time) time = value;
        }
    }

    public static int Seconds { get => seconds; }

    public static void Load()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(Application.persistentDataPath + "//save.txt", FileMode.Open, FileAccess.Read);
        object saveObj = formatter.Deserialize(stream);
        stream.Close();
        transDat trans = (transDat)saveObj;

        health = trans.health;
        keys = trans.keys;
        time = trans.time;
        seconds = (int)time;

        for (int i = 0; i < 3; i++)
        {
            position[i] = trans.HoldPosition[i];
        }
    }

    public static void Save()
    {
        transDat trans = new transDat
        {
            keys = keys,
            health = health,
            time = time,

            //HoldPosition = {mainScript.instance.transform.position.y,  }, Why can't I assign to an array in here
            y = mainScript.instance.transform.position.y,
            z = mainScript.instance.transform.position.z
        };
        trans.HoldPosition[0] = mainScript.instance.transform.position.x; //But here works? 

        BinaryFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(Application.persistentDataPath + "//save.txt", FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, trans);
        stream.Close();
    }

}


public class mainScript : MonoBehaviour
{

    //Misc
    private Light flashlight;
    public float timeScale;
    public Text time;
    public bool showSeconds;
    private RaycastHit flashHit;
    public GUIStyle g;
    //Mask
    public bool maskOn;
    private bool CR_mask;
    int lmask;
    public float intensityMult = 2f;

    //GUI
    public Texture2D[] heart;
    public Texture2D[] ammo_counter;
    public Texture2D timeImage;
    private bool canPause;
    public int goo;

    //Array of lights for the mask
    public Light[] lightArray;
    public static mainScript instance;
    private static List<GameObject> AllRooms;
    //public static Dictionary<Material, Color> defaultColors;
    Dictionary<string, Rect> rectPositions = new Dictionary<string, Rect>();
    DialogScript dg;

    private void GetAllMapObjects()
    {
        GameObject map_parent = GameObject.Find("Map.Furniture.V4");
        AllRooms = new List<GameObject>();
        for (int i = 0; i < map_parent.transform.childCount; i++)
        {
            AllRooms.Add(map_parent.transform.GetChild(i).gameObject);
        }

    }
	/*
	private void GetMaterialColors()
	{
        defaultColors = new Dictionary<Material, Color>();

		foreach(GameObject x in AllRooms)
		{
			Renderer rend = x.GetComponent<Renderer>();
			foreach(Material y in rend.materials) defaultColors.Add(y, y.color);
		}
	}
	*/
    private void Awake()
    {
		instance = this;

        //if (AllRooms == null)
        //GetAllMapObjects();


        if (!menu.newGame)
        {
            PlayerState.Load();
            transform.position = new Vector3(PlayerState.x, PlayerState.y + 1, PlayerState.z);
        }
    }
	


    void Start()
    {
        //if (defaultColors == null)
        //GetMaterialColors();
        dg = gameObject.AddComponent<DialogScript>();


        //Debug.Log(defaultColors.Keys);
        Debug.Log(Screen.width + " x " + Screen.height);
        GameObject[] gos = GameObject.FindGameObjectsWithTag("room_lights");
        lightArray = new Light[gos.Length];
        for (int i = 0; i < gos.Length; i++) lightArray[i] = gos[i].GetComponent<Light>();
        flashlight = GameObject.FindWithTag("flashlight").GetComponent<Light>();
        lmask = ~((
            1 << LayerMask.NameToLayer("triggers")) |
            1 << LayerMask.NameToLayer("Player") |
            1 << LayerMask.NameToLayer("bullet")
        );
        maskOn = false;
        CR_mask = true;

        //Dictionary Rects:
        rectPositions.Add("Heart", new Rect(0, 0, Screen.width / 9.16f, Screen.width / 9.16f));
        rectPositions.Add("Ammo", new Rect(Screen.width / 128.26f, Screen.height - (Screen.height / 3.79f), Screen.width / 8.75f, Screen.width / 8.75f));
        rectPositions.Add("Time", new Rect(Screen.width - (Screen.width / 6.41f), -(Screen.height / 18.22f), Screen.width / 6.41f, Screen.height / 3.04f));
        rectPositions.Add("TimeText", new Rect(Screen.width / 1.08f, Screen.height / 10.65f, Screen.width / 3.9f, Screen.height / 5.37f));

    }

    public static IEnumerator EnableRoom(GameObject[] correct)
    {

        List<GameObject> RemoveList = AllRooms;

        foreach (GameObject j in correct)
        {
            j.SetActive(true);
            RemoveList.Remove(j);
        }
            

        yield return new WaitForSeconds(1.0f);

        foreach (GameObject k in RemoveList)
        {
            k.SetActive(false);
            Debug.Log(k);
        }
            
    }

    IEnumerator mask(bool a)
    {
        CR_mask = false;
        yield return new WaitForSeconds(0.5f);

        if (a)
        {
            charCont.mainCam.cullingMask &= ~(1 << LayerMask.NameToLayer("ghosts"));//Enables culling mask for ghosts
            maskOn = false;
        }
        else
        {
            charCont.mainCam.cullingMask |= 1 << LayerMask.NameToLayer("ghosts"); //Enables culling mask for the ghosts
            maskOn = true;
        }

        for (int i = 0; i < lightArray.Length; i++)
            lightArray[i].intensity = maskOn ? (lightArray[i].intensity * intensityMult) : (lightArray[i].intensity / intensityMult);
        CR_mask = true;

    }

    void OnGUI()
    {

        //Draw Health
        GUI.DrawTexture(rectPositions["Heart"], heart[PlayerState.Health], ScaleMode.ScaleToFit, true);

        //Draw Ammo
        GUI.DrawTexture(rectPositions["Ammo"], ammo_counter[PlayerState.Ammo], ScaleMode.ScaleToFit, true);

        //Draw Time
        GUI.DrawTexture(rectPositions["Time"], timeImage, ScaleMode.ScaleToFit, true);
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.black;
        style.fontSize = goo;
        GUI.Label(rectPositions["TimeText"], convertTime(PlayerState.Seconds), style);
        //Flashlight
        Event current = Event.current;
        Vector2 mousePos = new Vector2();

        mousePos.x = current.mousePosition.x;
        mousePos.y = charCont.mainCam.pixelHeight - current.mousePosition.y;

        Ray ray = charCont.mainCam.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, charCont.mainCam.nearClipPlane));
        if (Physics.Raycast(ray, out flashHit, Mathf.Infinity, lmask)) flashlight.transform.LookAt(flashHit.point);

        //flashlight.transform.eulerAngles = new Vector3(Mathf.Clamp(flashlight.transform.eulerAngles.x, -20f, 15f), flashlight.transform.eulerAngles.y, 0f);
        // ^ Clamps vertical rotation between two constants. Issue: Currently locks to one constant, doesn't go negative



        //MAIN MENU
        if (Input.GetKeyDown(KeyCode.Escape))
        {

        }

    }

    string convertTime(int seconds)
    {
        float secs = seconds * timeScale;
        secs %= 24 * 3600;
        int hour = (int)(secs / 3600);
        secs %= 3600;
        int minutes = (int)(secs / 60);
        secs %= 60;

        string returnString = hour + 6 + ":" + minutes.ToString().PadLeft(2, '0');
        if (showSeconds) returnString += ':' + secs.ToString().PadLeft(2, '0');

        return returnString;
    }

    void OnTriggerStay(Collider col) //Has to be OnTriggerStay, OnTriggerEnter only gets called once
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (col.gameObject.tag == "healthPickup")
            {
                PlayerState.Health += 1; //Picking up hearts
                Destroy(col.gameObject);
            }
            else if (col.gameObject.tag == "pianoKey")
            {
                PlayerState.Keys += 1;
                Destroy(col.gameObject);
            }
        }

    }

    void Update()
    {
        PlayerState.Time = PlayerState.Time + Time.deltaTime;
        //if (time != null) time.text = convertTime(PlayerState.Seconds);

        //Lose-Death condition
        if (PlayerState.Health <= 0)
        {
            //SceneManager.LoadScene("loseCondition"); //Very tenuous

        }

        if(Input.GetKeyDown(KeyCode.K)) dg.Initialize("TestName", "I think we can put our differences behind us\nFor science.\nYou monster.", false);

        //Note firing / Flashlight toggle
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (PlayerState.IsInBossFight)
            {
                if (PlayerState.Ammo > 0)
                {
                    Rigidbody a = Instantiate(bossFight.projectile, transform.position, Quaternion.LookRotation(flashHit.point - transform.position));

                    a.gameObject.layer = LayerMask.NameToLayer("returnFire");
                    Bullet bul = new Bullet
                    {
                        ReturnFire = true,
                        InitSprite = bossFight.spriteNames["BlueSingle"],
                    };
                    bul.push(a.GetComponent<projectileScript>());
                    a.AddForce(a.transform.forward * bossFight.fire_speed, ForceMode.Impulse);
                    PlayerState.Ammo--;
                }
            }
            else
            {
                flashlight.enabled = !flashlight.enabled;
                flashlight.gameObject.GetComponent<AudioSource>().Play();
            }
        }

        if (Input.GetKeyDown(KeyCode.M) && CR_mask)
            StartCoroutine(mask(maskOn));
        /*
        if (Input.GetKeyDown(KeyCode.Escape))
		{
			PlayerState.Save();
           		Application.Quit();
		}
        */
    }

}
