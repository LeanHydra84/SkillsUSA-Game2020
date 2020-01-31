using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Switch;

public static class PlayerState
{
    static IEnumerator healthDelay()
    {
        canLose = false;
        yield return new WaitForSeconds(0.5f);
        canLose = true;
    }

    static int pianoKeys;
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
        Keys = new int[4];
        pianoKeys = 0;
        health = 4;
        time = 0f;
        seconds = 0;
        canLose = true;
        ammo = 0;
        isInBossFight = false; //SHOULD NOT BE TRUE

        System.Random rnd = new System.Random();
        Bosses = Enumerable.Range(1, 4).OrderBy(r => rnd.Next()).ToArray();

    }

    public static int Ammo
    {
        get => ammo;
        set { if (value < 8) ammo = value; }
    }

    private static bool isInBossFight;
    public static bool IsInBossFight
    {
        get => isInBossFight;
        set
        {
            isInBossFight = value;
            mainScript.aud.enabled = !isInBossFight;
        }
    }

    public static int[] Bosses { get; }

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

    public static int PianoKeys { get => pianoKeys; set => pianoKeys = value; }

    public static int[] Keys { get; set; }

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
        Stream stream = new FileStream(Application.persistentDataPath + "/save.txt", FileMode.Open, FileAccess.Read);
        object saveObj = formatter.Deserialize(stream);
        stream.Close();
        transDat trans = (transDat)saveObj;

        health = trans.health;
        PianoKeys = trans.keys;
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
            keys = PianoKeys,
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

    public static PlayerControls controls;
    public static AudioSource aud;

    //Misc
    private Light flashlight;
    private Vector3 flashDirection;
    public float timeScale;
    public Text time;
    public bool showSeconds;
    private RaycastHit flashHit;
    public GUIStyle g;
    public Text lockText;
    public Vector3 bossFightPos;
    public static Vector3 startingPos;

    public bossFight bf_instance;

    //Interaction Booleans
    private bool LeftClick;
    private bool Interact;

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
    private Image fadeBlack;

    //Array of lights for the mask
    public Light[] lightArray;
    public static mainScript instance;
    private static List<GameObject> AllRooms;
    Dictionary<string, Rect> rectPositions = new Dictionary<string, Rect>();
    public static DialogScript dg;
    GUIStyle style;

    private void GetAllMapObjects()
    {
        GameObject map_parent = GameObject.Find("TheMap");
        AllRooms = new List<GameObject>();
        for (int i = 0; i < map_parent.transform.childCount; i++)
        {
            AllRooms.Add(map_parent.transform.GetChild(i).gameObject);
        }

    }

    void FlashLightAim(InputAction.CallbackContext ctx)
    {
        Vector2 temp = -ctx.ReadValue<Vector2>();
        flashDirection = new Vector3(temp.x, 0, temp.y).normalized;
    }


    private void Awake()
    {
		instance = this;
        aud = GetComponent<AudioSource>();
        if (AllRooms == null)
            GetAllMapObjects();

        controls = new PlayerControls();
        controls.Controller.Enable();

        controls.Controller.FlashlightAim.performed += ctx => FlashLightAim(ctx);
        controls.Controller.FlashlightAim.canceled += ctx => flashDirection = Vector2.zero;
        controls.Controller.ShootFlashlight.started += ctx => LeftClick = true;
        controls.Controller.Interact.started += ctx => Interact = true;
        controls.Controller.QuitGame.performed += ctx => Application.Quit();

        if (!menu.newGame)
        {
            PlayerState.Load();
            transform.position = new Vector3(PlayerState.x, PlayerState.y + 1, PlayerState.z);
        }
    }

    public void FinishedBoss()
    {
        StartCoroutine(teleportToBoss(startingPos));
    }

    public IEnumerator teleportToBoss(Vector3 tele)
    {
        fadeBlack.GetComponent<Animator>().Play("ScreenFadeOut");
        yield return new WaitForSeconds(1.5f);
        transform.position = tele;
        yield return new WaitForSeconds(2.5f);
        fadeBlack.GetComponent<Animator>().Play("ScreenFadeIn");
    }

    public void StartBossFight(string bossFight1)
    {
        PlayerState.IsInBossFight = true;
        bossFight.fightName = bossFight1;
        bf_instance.Initialize();
        StartCoroutine(teleportToBoss(bossFightPos));
        Debug.Log("Start Battle");
    }

    void Start()
    {

        dg = gameObject.AddComponent<DialogScript>();
        fadeBlack = charCont.mainCam.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        fadeBlack.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
        fadeBlack.GetComponent<Animator>().Play("ScreenFadeIn");
        //Debug.Log(Screen.width + " x " + Screen.height + ", " + Screen.dpi);
        Debug.Log($"{Screen.width} x {Screen.height}, at {Screen.dpi} dpi");
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

        style = new GUIStyle();
        style.normal.textColor = Color.black;
        style.fontSize = (int)((100f / 1617f) * Screen.height / Screen.dpi * 72);

        startingPos = transform.position + new Vector3(0, 1, 0);

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
        }
            
    }

    IEnumerator mask(bool a)
    {
        CR_mask = false;
        yield return new WaitForSeconds(0.5f);

        if (a) charCont.mainCam.cullingMask &= ~(1 << LayerMask.NameToLayer("ghosts"));//Enables culling mask for ghosts
        else charCont.mainCam.cullingMask |= 1 << LayerMask.NameToLayer("ghosts"); //Enables culling mask for the ghosts
        
        maskOn = !a;

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

        GUI.Label(rectPositions["TimeText"], convertTime(PlayerState.Seconds), style);
        //Flashlight

        if(flashDirection == Vector3.zero)
        {
            Event current = Event.current;
            Vector2 mousePos = new Vector2();

            mousePos.x = current.mousePosition.x;
            mousePos.y = charCont.mainCam.pixelHeight - current.mousePosition.y;

            Ray ray = charCont.mainCam.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, charCont.mainCam.nearClipPlane));
            if (Physics.Raycast(ray, out flashHit, Mathf.Infinity, lmask)) flashlight.transform.LookAt(flashHit.point);
        }
        else
        {
            flashlight.transform.rotation = Quaternion.LookRotation(flashDirection);
        }
        

        //flashlight.transform.eulerAngles = new Vector3(Mathf.Clamp(flashlight.transform.eulerAngles.x, -20f, 15f), flashlight.transform.eulerAngles.y, 0f);
        // ^ Clamps vertical rotation between two constants. Issue: Currently locks to one constant, doesn't go negative



        //MAIN MENU
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
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
                PlayerState.PianoKeys += 1;
                Destroy(col.gameObject);
            }
        }

    }

    void TryOpenDoors(Collider[] cols)
    {
        foreach(Collider hit in cols)
        {
            if(hit.tag == "Puzzle")
            {
                hit.gameObject.AddComponent<Minigame_puzzle>();
                StartCoroutine(charCont.focusCamera(hit.transform.position));
                return;
            }

            if(hit.tag == "ghostDiag")
            {
                GhostDialogHandler g = hit.gameObject.GetComponent<GhostDialogHandler>();
                dg.Initialize(g.name, g.lines, "");
                return;
            }

            if(hit.tag == "bossHandler")
            {
                hit.gameObject.GetComponent<BossHandler>().Interact();
                return;
            }

            if(hit.tag == "door")
            {
                DoorHandler dh = hit.transform.parent.parent.gameObject.GetComponent<DoorHandler>();
                dh.OpenCloseDoor();
                if (dh.locked) lockText.GetComponent<Animator>().Play("fadeInOut");
                return;
            }
        }
    }

    void Update()
    {
        PlayerState.Time = PlayerState.Time + Time.deltaTime;
        //if (time != null) time.text = convertTime(PlayerState.Seconds);

        if (Input.GetKeyDown(KeyCode.L)) fadeBlack.GetComponent<Animator>().Play("ScreenFadeOut");
        if (Input.GetKeyDown(KeyCode.J)) fadeBlack.GetComponent<Animator>().Play("ScreenFadeIn");


        //Lose-Death condition
        if (PlayerState.Health <= 0)
        {
            //SceneManager.LoadScene("loseCondition"); //Very tenuous

        }

        if(Input.GetKeyDown(KeyCode.K)) dg.Initialize("GLADOS", "I think we can put our differences behind us\nFor science.\nYou monster.\nPlease place the Weighted Storage Cube on the Fifteen Hundred Megawatt Aperture Science Heavy Duty Super-Colliding Super Button", "");

        //Note firing / Flashlight toggle
        if (Input.GetKeyDown(KeyCode.Mouse0)) LeftClick = true;

        if(LeftClick)
        {
            if (PlayerState.IsInBossFight)
            {
                if (PlayerState.Ammo > 0)
                {

                    Quaternion startRot = (flashDirection == Vector3.zero) ? Quaternion.LookRotation(flashHit.point - transform.position) : Quaternion.LookRotation(flashDirection);
                    Rigidbody a = Instantiate(bossFight.projectile, transform.position, startRot);

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
        LeftClick = false;
        if (Input.GetKeyDown(KeyCode.M) && CR_mask)
            StartCoroutine(mask(maskOn));
        /*
        if (Input.GetKeyDown(KeyCode.Escape))
		{
			PlayerState.Save();
           		Application.Quit();
		}
        */

        if (Input.GetKeyDown(KeyCode.E)) Interact = true;
        if(Interact)
        {
            //RaycastHit[] hits = Physics.CapsuleCastAll();
            Collider[] hits = Physics.OverlapSphere(transform.position, 2f);
            TryOpenDoors(hits);
        }
        Interact = false;
    }

}
