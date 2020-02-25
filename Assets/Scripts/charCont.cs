using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class charCont : MonoBehaviour
{
    public static charCont instance;
    PlayerControls controls;

    //Values
    public bool debug = false;
    public bool cameraFollow;
    public float walkSpeed = 3;
    public float runMult = 1.6f;
    public float jumpForce = 8f;
    private float runSpeed;
    const float gravity = 20f;

    public static readonly Vector3 ScaleOutY = new Vector3(1, 0, 1);

    private bool sprint;
    private Transform Mesh;
    private Animator anim;
    private bool iswalking;
    private int camDirection;
    private int direction;
    private int turnDirection;
    private static Transform roomCamPosition;
    public static bool shouldBeFocused;
    public static bool isInDialog;
    public static bool isInEndBossFight;

    //Movement booleans
    public static bool isInPuzzle;
    private bool canMoveOnTransition;

    //Camera Values
    public Vector3 offset; //Math will be needed to fix the offset when the camera turns into rooms
    public float smoothness = 10f;

    //References
    public static Camera mainCam;
    private Camera moveableCamera;
    private CharacterController cc;
    public static Vector3 startingPoint = new Vector3(0f, 3.579f, -1.23f);

    //Vector Creation
    public static Vector3 moveDir = Vector3.zero;
    float mvY;
    float mvX;
    float mvZ;
    public Vector4 walkDirection;
    private Camera[] allCams;
    public bool airStrafing = false;
    public static bool TeleportBuffer;
    private Collider c1;

    Vector2 controllermover;

    static List<Coroutine> CurrentCoroutines;

    //Trigger booleans
    bool setJump;

    public int Direction
    {
        get => direction;
        set
        {
            if (value != 0) direction = value;
        }
    }

    public bool IsWalking
    {
        get => iswalking;
        set
        {
            if (iswalking != value)
            {
                //anim.SetTrigger(value ? "StartWalk" : "EndWalk");
                anim.enabled = value;
            }
            iswalking = value;
        }
    }

    void EnableCamera(Camera c)
    {
        for (int i = 0; i < allCams.Length; i++)
        {
            allCams[i].enabled = false;
        }
        c.enabled = true;
    }

    public void EndLerpCoroutines()
    {
        if(CurrentCoroutines.Count > 0)
        {
            foreach (Coroutine c in CurrentCoroutines)
            {
                StopCoroutine(c);
            }
            CurrentCoroutines.Clear();
        }
        
    }

    private void Awake()
    {
        instance = this;
        controls = new PlayerControls();
        controls.Controller.Enable();

        controls.Controller.JumpAction.performed += ctx => setJump = true;
        controls.Controller.Move.performed += ctx => controllermover = ctx.ReadValue<Vector2>();
        controls.Controller.Move.canceled += ctx => controllermover = Vector2.zero;
        controls.Controller.Sprint.performed += ctx => sprint = true;
        controls.Controller.Sprint.canceled += ctx => sprint = false;
        CurrentCoroutines = new List<Coroutine>();
        allCams = Camera.allCameras;
        EnableCamera(Camera.main);
    }

    void Start()
    {
        canMoveOnTransition = true;
        mainCam = Camera.main;
        runSpeed = walkSpeed * runMult;
        cc = GetComponent<CharacterController>();
        walkDirection = ScaleOutY;

        Mesh = transform.GetChild(0);
        anim = Mesh.GetChild(0).GetComponent<Animator>();

        anim.SetTrigger("StartWalk");
        anim.enabled = false;
    }

    public void ChangeCamera(roomClass rc) //Allows camera change to be called by any script with the same method
    {
        walkDirection = rc.md;
        //EnableCamera(rc.roomCam);
        Debug.Log(rc.roomName);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "triggerRoom" && other != c1)
        {

            if (Time.time > 1f && c1.GetComponent<roomClass>().isHallway) //Emables camera after exiting hallway, sets position to exiting position
            {
                EnableCamera(mainCam);
                //mainCam.transform.position = c1.GetComponent<roomClass>().roomCam.transform.position;
            }
            roomClass rc = other.gameObject.GetComponent<roomClass>();
            walkDirection = rc.md;
            roomCamPosition = rc.roomCam.transform;
            EndLerpCoroutines();
            CurrentCoroutines.Add(StartCoroutine(lerpCamera(roomCamPosition, rc.isHallway)));
            CurrentCoroutines.Add(StartCoroutine(mainScript.EnableRoom(rc.assets)));

            c1 = other;
        }
    }

    void OnGUI() // INFO
    {
        if (debug)
        {
            string editString =
            "AIRSTRAFE = " + airStrafing.ToString().ToUpper() +
            "\nCAM_OFFSET = " + offset +
            "\nCAM_SMOOTH = " + smoothness +
            "\nCONST_GRAVITY = " + gravity +
            "\nMOVE_SPEED = " + walkSpeed +
            "\nRUN_SPEED = " + runSpeed +
            "\nPLAYER_ISRUNNING = " + Input.GetKey(KeyCode.LeftShift).ToString().ToUpper() +
            "\nPLAYER_ISGROUNDED = " + cc.isGrounded.ToString().ToUpper() +
            "\nMOVE_VECTOR = " + moveDir +
            "\nTIME = " + PlayerState.Seconds +
            "\nFramerate = " + 1.0f / Time.deltaTime;
            GUI.skin.textArea.active.background =
            GUI.skin.textArea.normal.background =
            GUI.skin.textArea.onHover.background =
            GUI.skin.textArea.hover.background =
            GUI.skin.textArea.onFocused.background =
            GUI.skin.textArea.focused.background = null;
            GUI.TextArea(new Rect(10, 50, 400, 400), editString);
        }
    }

    int GetCamDirection(Vector4 m)
    {
        int cd = (int)((m.x * 10) + 20);
        cd += (int)(m.z + 2);
        cd *= (m.w < float.Epsilon) ? 1 : -1;
        return cd;
    }

    public static bool ShouldFocus() { return shouldBeFocused; }

    public static IEnumerator focusCamera(Vector3 input)
    {
        isInPuzzle = true;
        float StartTime = Time.time;
        float EndTime = 0.2f + StartTime;
        Vector3 startPoint = mainCam.transform.position;
        Vector3 focusPoint = input + new Vector3(0, 2, 2);
        Quaternion toRotation = Quaternion.LookRotation(input - focusPoint);
        Quaternion fromRotation = mainCam.transform.rotation;

        while (Time.time < EndTime)
        {
            float timeProg = (Time.time - StartTime) / 0.2f;
            mainCam.transform.rotation = Quaternion.Lerp(fromRotation, toRotation, timeProg);
            mainCam.transform.position = Vector3.Lerp(startPoint, focusPoint, timeProg);
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitUntil(ShouldFocus);

        StartTime = Time.time;
        EndTime = 0.2f + StartTime;

        while (Time.time < EndTime)
        {
            float timeProg = (Time.time - StartTime) / 0.2f;
            mainCam.transform.rotation = Quaternion.Lerp(toRotation, roomCamPosition.rotation, timeProg);
            mainCam.transform.position = Vector3.Lerp(focusPoint, roomCamPosition.position, timeProg);
            yield return new WaitForFixedUpdate();
        }

        isInPuzzle =
        shouldBeFocused = false;

    }

    bool CanMove()
    {
        return (canMoveOnTransition && !isInPuzzle && !isInDialog && !isInEndBossFight);
    }

    bool isSprint() => Input.GetKey(KeyCode.LeftShift) || sprint;

    IEnumerator lerpCamera(Transform t, bool isHall)
    {


        if (Time.time > 0.1f && camDirection != GetCamDirection(walkDirection)) canMoveOnTransition = false;
        camDirection = GetCamDirection(walkDirection);

        //Vector3 mePosition = mainCam.transform.position;

        if (isHall)
        {
            //moveableCamera = t.GetComponent<Camera>();
            //EnableCamera(moveableCamera);
            cameraFollow = true;
            yield return new WaitForSeconds(0.4f);
        }
        else
        {
            cameraFollow = false;

            float startTime = Time.time;
            //float journeyDistance = Vector3.Distance(t.position, transform.position);
            float speed = .1f;



            while (Time.time - startTime < 2f)
            {
                mainCam.transform.rotation = Quaternion.Lerp(mainCam.transform.rotation, t.rotation, (Time.time - startTime) * speed);
                mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, t.position, (Time.time - startTime) * speed);
                if (Time.time - startTime > 1.1f) canMoveOnTransition = true;
                yield return 0;
            }
            //yield return new WaitForSeconds(0.5f);

        }
        canMoveOnTransition = true;
        //EndLerpCoroutines();
    }

    void Update()
    {

        float speed = isSprint() ? runSpeed : walkSpeed; //Sprinting
        anim.speed = isSprint() ? 2 : 1.2f;

        if (airStrafing || cc.isGrounded) mvX = mvZ = mvY = 0;

        if (Input.GetKeyDown(KeyCode.Space)) setJump = true;

        if (setJump && cc.isGrounded) mvY = jumpForce; //Jumping
        setJump = false;

        //Movement
        float zSpeed = walkDirection.z * speed;
        float xSpeed = walkDirection.x * speed;

        if ((cc.isGrounded || airStrafing) && CanMove())
        {
            if (Input.GetKey(KeyCode.W)) mvZ -= zSpeed; //Forwards
            if (Input.GetKey(KeyCode.S)) mvZ += zSpeed; //Back
            if (Input.GetKey(KeyCode.D)) mvX -= xSpeed; //Right
            if (Input.GetKey(KeyCode.A)) mvX += xSpeed; //Left
        }

        if (controllermover != Vector2.zero && CanMove())
        {
            mvX = -controllermover.x * speed;
            mvZ = -controllermover.y * speed;
        }

        //Handling Gravity
        mvY -= gravity * Time.deltaTime;

        //Making the move
        if (walkDirection.w < float.Epsilon) moveDir = new Vector3(mvX, mvY, mvZ);
        else moveDir = new Vector3(mvZ, mvY, mvX);

        Direction = (int)((mvX / speed) * 10 + (mvZ / speed));
        
        if(moveDir != Vector3.zero) cc.Move(moveDir * Time.deltaTime);
    }



    IEnumerator MakeLerp(Quaternion one, Quaternion two, float time)
    {
        float StartTime = Time.time;
        float EndTime = StartTime + time;

        while (Time.time < EndTime)
        {
            float timeProg = (Time.time - StartTime) / time;
            Mesh.rotation = Quaternion.Lerp(one, two, timeProg);
            yield return new WaitForFixedUpdate();
        }


    }

    void LateUpdate() //Camera movement stuff
    {
        //Could save this transform as a variable rather than just saying transform
        //This would let me pass in another object for the camera to target if necessary
        if (cameraFollow)
        {
            Vector3 toPosition = transform.position + offset;
            Vector3 smoothPos = Vector3.Lerp(mainCam.transform.position, toPosition, smoothness * Time.deltaTime);
            mainCam.transform.position = smoothPos;
            //mainCam.transform.LookAt(transform);
        }

        //if (cameraFollow)
        //{
        //    moveableCamera.transform.position = new Vector3(transform.position.x - 150, moveableCamera.transform.position.y, transform.position.z);
        //}

        if (!moveDir.x.Equals(0) || !moveDir.z.Equals(0))
        {

            IsWalking = true;

            Quaternion smoothedTurn = Quaternion.LookRotation(new Vector3(mvX, 0, mvZ).normalized);

            if (turnDirection != Direction)
            {
                StartCoroutine(MakeLerp(Mesh.rotation, smoothedTurn, 0.3f));
                turnDirection = Direction;
            }

            //Mesh.rotation = smoothedTurn;//Quaternion.Lerp(transform.rotation, smoothedTurn, turnSpeed * Time.deltaTime);
        }

        else IsWalking = false;

    }

}
