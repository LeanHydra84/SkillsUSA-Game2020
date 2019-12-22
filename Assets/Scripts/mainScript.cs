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

    //Mask
    public bool maskOn;
    private bool CR_mask;
    int lmask;
    public float intensityMult = 2f;

    //GUI
    public Texture2D[] heart;
    public Texture2D[] ammo_counter;

    //Array of lights for the mask
    public Light[] lightArray;
    public static mainScript instance;
	
    private void Awake()
    {
        instance = this;
		if(!menu.newGame) 
		{
			PlayerState.Load();
            transform.position = new Vector3(PlayerState.x, PlayerState.y + 1, PlayerState.z);
		}		
    }
    
    void Start()
    {
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
        Rect heartPos = new Rect(0, 0, 210, 210);
        GUI.DrawTexture(heartPos, heart[PlayerState.Health], ScaleMode.ScaleToFit, true);

        //Draw Ammo
        Rect ammoPos = new Rect(15, Screen.height - 240, 220, 220);
        GUI.DrawTexture(ammoPos, ammo_counter[PlayerState.Ammo], ScaleMode.ScaleToFit, true);

        //Flashlight
        Event current = Event.current;
        Vector2 mousePos = new Vector2();

        mousePos.x = current.mousePosition.x;
        mousePos.y = charCont.mainCam.pixelHeight - current.mousePosition.y;

        Ray ray = charCont.mainCam.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, charCont.mainCam.nearClipPlane));
        if (Physics.Raycast(ray, out flashHit, Mathf.Infinity, lmask)) flashlight.transform.LookAt(flashHit.point);

        //flashlight.transform.eulerAngles = new Vector3(Mathf.Clamp(flashlight.transform.eulerAngles.x, -20f, 15f), flashlight.transform.eulerAngles.y, 0f);
        // ^ Clamps vertical rotation between two constants. Issue: Currently locks to one constant, doesn't go negative
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
        if(time != null) time.text = convertTime(PlayerState.Seconds);

        //Lose-Death condition
        if (PlayerState.Health <= 0)
        {
            //SceneManager.LoadScene("loseCondition"); //Very tenuous

        }

        //Note firing / Flashlight toggle
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(PlayerState.IsInBossFight)
            {
                if(PlayerState.Ammo > 0)
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

        if (Input.GetKeyDown(KeyCode.Escape))
		{
			PlayerState.Save();
           		Application.Quit();
		}
    }

}
