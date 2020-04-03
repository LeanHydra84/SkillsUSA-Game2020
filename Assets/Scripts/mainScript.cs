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

public delegate void DarkAction_Bool(bool x);
public delegate void DarkAction();

public static class PlayerState
{
    static IEnumerator healthDelay()
    {
        canLose = false;
        yield return new WaitForSeconds(0.5f);
        canLose = true;
    }

    public const int MaxHealth = 6;
    
    static bool alive;
    static int pianoKeys;
    static int health;
    static float time;
    static int seconds;
    static bool canLose;
    static int ammo;

    public static bool hasMask;

    public static bool[] Victories { get; set; }

    public static float x;
    public static float y;
    public static float z;

    public static int[] Bosses { get; set; }

    public static int[] PuzzlePositions { get; set; }

    public static void Reset()
    {
        hasMask = false;
        Keys = new int[4];
        pianoKeys = 0;
        health = MaxHealth;
        time = 0f;
        seconds = 0;
        canLose = true;
        ammo = 0;
        isInBossFight = false; //SHOULD NOT BE TRUE
        alive = true;
        Victories = new bool[4];
        System.Random rnd = new System.Random();
        Bosses = Enumerable.Range(1, 8).OrderBy(r => rnd.Next()).ToArray();
        
    }

    static PlayerState()
    {
        Reset();
    }

    public static int Ammo
    {
        get => ammo;
        set { if (value < 8) ammo = value; }
    }

    public static bool IsAlive { get => alive; set => alive = value; }

    private static bool isInBossFight;
    public static bool IsInBossFight
    {
        get => isInBossFight;

        set
        {
            int cursorTrype = Convert.ToInt32(!isInBossFight);
            Texture2D image = mainScript.instance.Cursors[cursorTrype];
            Cursor.SetCursor(image, new Vector2(image.width / 2, image.height / 2), CursorMode.ForceSoftware);

            isInBossFight = value;
            mainScript.aud.enabled = !isInBossFight;
        }
    }


    public static int Health
    {
        get => health;
        set
        {
            if (canLose && (value >= 0 && value <= MaxHealth))
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
        Keys = trans.keys;
        time = trans.time;
        seconds = (int)time;
        hasMask = trans.hasMask;

        x = trans.x;
        y = trans.y;
        z = trans.z;

        Bosses = trans.Bosses;
        PuzzlePositions = trans.PuzzlePositions;
        Victories = trans.Victories;
    }

    public static void Save()
    {

        transDat trans = new transDat
        {
            keys = Keys,
            health = health,
            time = seconds,
            Bosses = Bosses,
            Victories = Victories,
            PuzzlePositions = PuzzlePositions,
            hasMask = hasMask,

            x = mainScript.instance.transform.position.x,
            y = mainScript.instance.transform.position.y,
            z = mainScript.instance.transform.position.z
        };

        BinaryFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(Application.persistentDataPath + "//save.txt", FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, trans);
        stream.Close();

        Debug.Log("Saved");

    }

}


public class mainScript : MonoBehaviour
{
    
    public static PlayerControls controls;
    public static AudioSource aud;

    //Misc
    private Light flashlight;
    private GameObject directionalLight;
    private Vector3 flashDirection;
    public float timeScale;
    public Text time;
    public bool showSeconds;
    private RaycastHit flashHit;
    public GUIStyle g;
    public Text lockText;
    public Vector3 bossFightPos;
    public static Vector3 startingPos;
    Animator FadeAnimator;
    [SerializeField] private Text timeText;
    [SerializeField] private Image MaskCheck;
    [SerializeField] private GameObject PauseScreen;
    

    public Texture2D[] Cursors;

    //Interaction Booleans
    private bool LeftClick;
    private bool Interact;

    //Mask
    public bool maskOn;
    private bool CR_mask;
    int lmask;
    public float intensityMult = 2f;

    //GUI
    public Texture2D healthImage;
    public Texture2D[] ammo_counter;
    private bool canPause;
    private Image fadeBlack;

    //Array of lights for the mask
    public Light[] lightArray;
    public static mainScript instance;
    public static List<GameObject> AllRooms;
    Dictionary<string, Rect> rectPositions = new Dictionary<string, Rect>();
    public static DialogScript dg;
    public static List<GameObject> dontCheck;
    GUIStyle style;

    public static GameObject[] handler_bosses;


    private void GetAllMapObjects()
    {
        GameObject map_parent = GameObject.Find("TheMap");
        handler_bosses = new GameObject[4];

        //Getting boss handlers
        for (int i = 0; i < 4; i++)
        {
            handler_bosses[i] = map_parent.transform.GetChild(i).gameObject;
        }



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

    void StaticReset_Master()
    {
        Minigame_puzzle.StaticReset();
        Key_Handler.update = null;
    }

    private void OnLevelWasLoaded(int level)
    {
        StaticReset_Master();
    }

    private void Awake()
    {
        instance = this;
        aud = GetComponent<AudioSource>();
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
            transform.position = new Vector3(PlayerState.x, PlayerState.y + 2, PlayerState.z);
            Key_Handler.update(0);
        }



    }

    public void FinishedBoss()
    {
        StartCoroutine(fadeTeleport(startingPos));
        PlayerState.Health = PlayerState.MaxHealth;
    }

    public IEnumerator fadeTeleport(Vector3 tele)
    {
        fadeBlack.GetComponent<Animator>().Play("ScreenFadeOut");
        charCont.instance.enabled = false;
        yield return new WaitForSeconds(1.5f);
        transform.position = tele;
        yield return new WaitForSeconds(2.0f);
        charCont.instance.enabled = true;
        yield return new WaitForSeconds(0.5f);
        fadeBlack.GetComponent<Animator>().Play("ScreenFadeIn");
        charCont.isInEndBossFight = false;
    }

    public void StartBossFight(string bossFight1)
    {
        PlayerState.IsInBossFight = true;
        bossFight.fightName = bossFight1;
        bossFight.instance.gameObject.SetActive(true);
        bossFight.instance.Initialize();
        StartCoroutine(fadeTeleport(bossFightPos));
        Debug.Log("Start Battle");
    }


    void Start()
    {
        directionalLight = GameObject.Find("D1");
        dg = gameObject.AddComponent<DialogScript>();
        fadeBlack = charCont.mainCam.transform.GetChild(0).Find("FadeBlack").GetComponent<Image>();
        fadeBlack.GetComponent<Animator>().Play("ScreenFadeIn");
        System.Random rnd = new System.Random();
        PlayerState.PuzzlePositions = Enumerable.Range(0, mapCreator.instance.Rooms.Length - 1).OrderBy(r => rnd.Next()).ToArray();
        bossFight.instance = GameObject.Find("Boss").GetComponent<bossFight>();
        Debug.Log($"{Screen.width} x {Screen.height}, at {Screen.dpi} dpi");
        GameObject[] gos = GameObject.FindGameObjectsWithTag("room_lights");
        FadeAnimator = fadeBlack.GetComponent<Animator>();

        lightArray = new Light[gos.Length];
        for (int i = 0; i < gos.Length; i++) lightArray[i] = gos[i].GetComponent<Light>();

        for (int i = 0; i < lightArray.Length; i++)
        {
            lightArray[i].gameObject.SetActive(false);
            lightArray[i].enabled = true;
        }
        flashlight = GameObject.FindWithTag("flashlight").GetComponent<Light>();
        lmask = ~((
            1 << LayerMask.NameToLayer("triggers")) |
            1 << LayerMask.NameToLayer("Player") |
            1 << LayerMask.NameToLayer("bullet")
        );
        maskOn = false;
        CR_mask = true;
        MaskHolder(true);
        MaskCheck.gameObject.SetActive(false);
        style = new GUIStyle();
        style.normal.textColor = Color.black;
        style.fontSize = (int)((100f / 1617f) * Screen.height / Screen.dpi * 72);

        dontCheck = new List<GameObject>();

        startingPos = transform.position + new Vector3(0, 1, 0);
        dontCheck.Clear();

        //Dictionary Rects:
        rectPositions.Clear();
        rectPositions.Add("Ammo", new Rect(Screen.width / 96.2f, Screen.height / 1.34f, Screen.width / 4.81f, Screen.height / 3.96f));
        rectPositions.Add("HeartPosition", new Rect(Screen.width - Screen.width / 12, Screen.height - Screen.width / 12, Screen.width / 10, Screen.width / 10));

        Cursor.SetCursor(Cursors[0], Vector2.zero, CursorMode.ForceSoftware);

        mapCreator.instance.Initialize();

    }

    public void DisableBossFigure(string bossLabel)
    {
        for (int i = 0; i < 4; i++)
        {
            if (handler_bosses[i] != null)
            {
                if (handler_bosses[i].GetComponent<BossHandler>().myName.Equals(bossLabel))
                {
                    dontCheck.Add(handler_bosses[i]);
                    Destroy(handler_bosses[i]);
                    handler_bosses[i] = null;
                }
            }
        }
    }

    public static IEnumerator EnableRoom(GameObject[] correct, GameObject puzzle)
    {

        List<GameObject> RemoveList = AllRooms.ToList();

        //List<GameObject> corr = correct.ToList();

        //foreach (GameObject j in dontCheck)
        //{
        //    corr.Remove(j);
        //}

        foreach (GameObject j in correct)
        {
            /*try { j.SetActive(true); }
            catch (MissingReferenceException) { Debug.Log("Failed to create object"); }*/
            if (j != null) j.SetActive(true);
            RemoveList.Remove(j);
        }

        if (puzzle != null)
        {
            puzzle.SetActive(true);
            RemoveList.Remove(puzzle);
        }

        yield return new WaitForSeconds(1.0f);

        foreach (GameObject k in RemoveList)
        {

            if (k != null)
            {
                k.SetActive(false);
            }
            /*try { k.SetActive(false); }
            catch (MissingReferenceException) { Debug.Log($"Caught {k}"); }*/
        }


    }



    public IEnumerator FadeInAndOut(float time, DarkAction_Bool meAction, bool a)
    {
        float _speed = FadeAnimator.speed;
        FadeAnimator.speed = 2f;
        FadeAnimator.Play("ScreenFadeOut");
        yield return new WaitForSeconds(time);
        meAction(a);
        FadeAnimator.Play("ScreenFadeIn");
        FadeAnimator.speed = _speed;
    }

    public IEnumerator FadeInAndOut(float time)
    {
        FadeAnimator.Play("ScreenFadeOut");
        yield return new WaitForSeconds(time);
        FadeAnimator.Play("ScreenFadeIn");
    }
    public IEnumerator FadeInAndOut(float time, DarkAction meAction)
    {
        FadeAnimator.Play("ScreenFadeOut");
        yield return new WaitForSeconds(time);
        meAction();
        FadeAnimator.Play("ScreenFadeIn");
    }

    void MaskAlpha(bool a)
    {
        Color Tc = MaskCheck.color;
        Tc.a = a ? 0.05f : 1;
        MaskCheck.color = Tc;
    }

    void MaskHolder(bool a)
    {
        CR_mask = false;
        MaskAlpha(a);
        if (a) charCont.mainCam.cullingMask &= ~(1 << LayerMask.NameToLayer("ghosts")); //Enables culling mask for ghosts
        else charCont.mainCam.cullingMask |= 1 << LayerMask.NameToLayer("ghosts"); //Enables culling mask for the ghosts

        maskOn = !a;
        directionalLight.SetActive(maskOn);
        for (int i = 0; i < lightArray.Length; i++)
        {
            lightArray[i].intensity = maskOn ? (lightArray[i].intensity * intensityMult) : (lightArray[i].intensity / intensityMult);
            lightArray[i].range *= (maskOn ? 1.5f : 0.666f);
        }

        CR_mask = true;
    }

    void mask(bool a) { if (PlayerState.hasMask) StartCoroutine(FadeInAndOut(1.4f, MaskHolder, a)); }

    void OnGUI()
    {

        //Draw Health
        if (!charCont.isInPuzzle && !charCont.isInDialog)
        {
            Rect heartPosition;
            for (int i = 0; i < PlayerState.Health; i++)
            {
                heartPosition = rectPositions["HeartPosition"];
                heartPosition.x -= i * (Screen.width / 18);
                GUI.DrawTexture(heartPosition, healthImage, ScaleMode.ScaleToFit, true);
            }
        }

        if (PlayerState.IsInBossFight)
        {
            GUI.DrawTexture(rectPositions["Ammo"], ammo_counter[PlayerState.Ammo], ScaleMode.ScaleToFit, true);
        }

        //Flashlight
        if (flashDirection == Vector3.zero)
        {
            Event current = Event.current;
            Vector2 mousePos = new Vector2();

            mousePos.x = current.mousePosition.x;
            mousePos.y = charCont.mainCam.pixelHeight - current.mousePosition.y;

            Ray ray = charCont.mainCam.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, charCont.mainCam.nearClipPlane));
            if (Physics.Raycast(ray, out flashHit, Mathf.Infinity, lmask)) flashlight.transform.LookAt(flashHit.point);
            flashlight.transform.position = transform.position + (flashHit.point - transform.position).normalized;
        }
        else
        {
            flashlight.transform.rotation = Quaternion.LookRotation(flashDirection);
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

    void TryInteract(Collider[] cols)
    {
        foreach (Collider hit in cols)
        {
            if (hit.tag == "item" && !charCont.isInPuzzle)
            {
                if (hit.name == "Mask")
                {
                    PlayerState.hasMask = true;
                    MaskCheck.gameObject.SetActive(true);
                    Destroy(hit.gameObject);
                }
                return;
            }

            if (hit.tag == "Puzzle" && !charCont.isInPuzzle)
            {
                hit.gameObject.AddComponent<Minigame_puzzle>();
                charCont.instance.EndLerpCoroutines();
                StartCoroutine(charCont.focusCamera(hit.transform.position));
                return;
            }

            if (hit.tag == "ghostDiag" && !charCont.isInDialog && !charCont.isInPuzzle && maskOn)
            {

                GhostDialogHandler g = hit.gameObject.GetComponent<GhostDialogHandler>();
                dg.Initialize(g.ghostName, g.lines, "");
                g.LineNumber++;
                return;
            }

            if (hit.tag == "bossHandler" && !charCont.isInDialog && !charCont.isInPuzzle && maskOn)
            {
                hit.gameObject.GetComponent<BossHandler>().Interact();
                return;
            }

            if (hit.tag == "door")
            {
                DoorHandler dh = hit.transform.parent.parent.gameObject.GetComponent<DoorHandler>();
                dh.OpenCloseDoor();
                if (dh.locked) lockText.GetComponent<Animator>().Play("fadeInOut");
                return;
            }
        }
    }

    IEnumerator DeathMethod()
    {
        Animator _fadeAnimator = fadeBlack.GetComponent<Animator>();
        _fadeAnimator.Play("ScreenFadeOut");
        charCont.instance.enabled = false;
        yield return new WaitForSeconds(1.25f);
        SceneManager.LoadScene("GameOver");
    }

    void Update()
    {

        PlayerState.Time = PlayerState.Time + Time.deltaTime;
        //if (time != null) time.text = convertTime(PlayerState.Seconds);

        if (timeText != null) timeText.text = convertTime(PlayerState.Seconds);

        //Lose-Death condition
        if (PlayerState.Health <= 0)
        {
            if (PlayerState.IsAlive) StartCoroutine(DeathMethod());
        }


        //DEBUG KEYS
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerState.Health--;
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            PlayerState.Health++;
        }


        if (Input.GetKeyDown(KeyCode.K)) dg.Initialize("GLADOS", "I think we can put our differences behind us:=For science.:=You monster.:=Please place the Weighted Storage Cube on the Fifteen Hundred Megawatt Aperture Science Heavy Duty Super-Colliding Super Button", "");

        if (Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log("Taking you back");
            StartCoroutine(fadeTeleport(startingPos));
        }

        //Note firing / Flashlight toggle
        if (Input.GetKeyDown(KeyCode.Mouse0)) LeftClick = true;

        if (LeftClick && !charCont.isInDialog)
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
        if (Input.GetKeyDown(KeyCode.Q) && CR_mask)
            mask(maskOn);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseMenu();
        }


        if (Input.GetKeyDown(KeyCode.E)) Interact = true;
        if (Interact)
        {
            //RaycastHit[] hits = Physics.CapsuleCastAll();
            Collider[] hits = Physics.OverlapSphere(transform.position, 2f);
            TryInteract(hits);
        }
        Interact = false;
    }
    public static bool isInPauseMenu;

    IEnumerator timeScaler(bool a)
    {
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 1;//a ? 1 : 0;
    }

    private void PauseMenu()
    {
        isInPauseMenu = !isInPauseMenu;

        if (isInPauseMenu) StartCoroutine(timeScaler(false));
        else Time.timeScale = 1;

        FadeAnimator.Play(isInPauseMenu ? "StartPause" : "EndPause");
        PauseScreen.SetActive(isInPauseMenu);

    }
}
