using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class DDRPosition
{
    public static Vector2 Size = new Vector2(200, 200);
    public static Texture2D[] Arrow;

    private float speed = 15f;
    public int Value { get; set; }
    private int DisplayValue;
    Rect Rpos;
    Rect Apos;

    public float XPosition { get => Rpos.x; }

    public void Move()
    {
        Rpos.x -= speed;
        Apos.x -= speed;
    }

    static DDRPosition()
    {
        object[] x = Resources.LoadAll("Arrow", typeof(Texture2D));
        Arrow = new Texture2D[4];
        for (int i = 0; i < 4; i++)
        {
            Arrow[i] = (Texture2D)x[i];
        }

    }

    public DDRPosition(Vector2 ScreenPosition, int Value)
    {
        this.Value = Value - 1;
        DisplayValue = this.Value;
        Rpos = new Rect(ScreenPosition, Size);
        Apos = new Rect(ScreenPosition + new Vector2(Size.x / 3, 140), Size / 6);
    }

    public void DrawPosition()
    {
        GUI.DrawTexture(Rpos, Minigame_puzzle.showSprites[DisplayValue], ScaleMode.StretchToFill);
        GUI.DrawTexture(Apos, Arrow[Value], ScaleMode.StretchToFill);
        if (DisplayValue < 4 && Rpos.x + Size.x / 2 < Minigame_puzzle.CheckPoint)
        {
            DisplayValue += 4;
        }
    }

    public bool isLegal()
    {
        return Rpos.x + Size.x > 0;
    }


    public bool CheckPosition(out float g)
    {
        float _distance = Mathf.Abs(Rpos.x - Minigame_puzzle.CheckPoint);
        g = XPosition;
        if (_distance < 150) //THIS IS IN PIXELS, WHICH CHAAAANGES - FFFUUUUCCCCCKKKK
        {
            return true;
        }
        return false;
    }

}

public class Minigame_puzzle : MonoBehaviour
{
    public static float CheckPoint = Screen.width / 3;
    public int PuzzleType = 1;
    int Guys_Index;
    int missed;
    List<DDRPosition> ddr = new List<DDRPosition>();
    Coroutine DanceMovement;
    
    public string hashString;
    public string checkString;
    public int length;
    public int counter;

    private int[] addCurrent;

    bool canAdd;

    //public GameObject[] showSprites;
    public static Texture2D[] showSprites;
    public static AudioClip[] clips;
    private static Animator[] TheBoys;
    private AudioSource aud;

    private void OnLevelWasLoaded(int level)
    {
        
    }

    void SetAll(int a) { for (int i = 0; i < 4; i++) { addCurrent[i] = a; } }

    void Start()
    {

        mainScript.controls.Controller.MG_Up.started += ctx => addString(4);
        mainScript.controls.Controller.MG_Down.started += ctx => addString(2);
        mainScript.controls.Controller.MG_Left.started += ctx => addString(1);
        mainScript.controls.Controller.MG_Right.started += ctx => addString(3);

        if (clips == null)
        {
            object[] sprites = Resources.LoadAll("Minigame_Sprites", typeof(Texture2D));
            object[] trumpet_sounds = Resources.LoadAll("Trumpet_Sounds", typeof(AudioClip));

            showSprites = new Texture2D[8];
            clips = new AudioClip[5];

            int counter = 0;
            foreach (UnityEngine.Object a in sprites)
            {
                showSprites[counter] = (Texture2D)a;
                counter++;
            }

            counter = 0;
            foreach (UnityEngine.Object x in trumpet_sounds)
            {
                clips[counter] = (AudioClip)x;
                counter++;
            }

            TheBoys = new Animator[4];
            int j = 0;
            Transform temporary = charCont.mainCam.transform.GetChild(0);
            for (int i = 2; i < 6; i++)
            {
                TheBoys[j++] = temporary.GetChild(i).GetComponent<Animator>();
            }

        }

        if (PuzzleType == 0)
        {
            addCurrent = new int[4];
            SetAll(0);

            //Randomization goes here
            StartCoroutine(EndAttempt(0.2f));
            aud = GetComponent<AudioSource>();


            SetAll(4);
        }

        else if (PuzzleType == 1)
        {
            randomize(10);
            Guys_Index = UnityEngine.Random.Range(0, 4); // "The GUYS INDEX is looking a little low" - Darkk Mane
            TheBoys[Guys_Index].gameObject.SetActive(true);
            TheBoys[Guys_Index].SetTrigger("FadeIn");
            DanceMovement = StartCoroutine(DanceDance());
        }


    }

    IEnumerator GameWin()
    {
        Debug.Log("You win");
        float CompleteWait;
        switch (PuzzleType)
        {
            case 0:
                CompleteWait = aud.clip.length;
                break;
            case 1:
                CompleteWait = 0;
                TheBoys[Guys_Index].SetTrigger("FadeOut");
                yield return new WaitForSeconds(2f);
                TheBoys[Guys_Index].gameObject.SetActive(false);
                break;
            default:
                CompleteWait = 1f;
                break;
        }
        yield return new WaitForSeconds(CompleteWait);
        mainScript.AllRooms.Remove(gameObject);
        mainScript.dontCheck.Add(gameObject);
        Destroy(gameObject);
        GetComponent<Key_Handler>().win();
    }

    void randomize(int n)
    {
        hashString = "";
        int rand = UnityEngine.Random.Range(n, n + 1);
        for (int i = 0; i < rand; i++)
        {
            hashString += UnityEngine.Random.Range(1, 5).ToString();
        }
        length = hashString.Length;
    }

    IEnumerator DanceDance()
    {


        for (int i = 0; i < hashString.Length; i++)
        {
            CreateMember((int)char.GetNumericValue(hashString[i]));

            yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.8f));
        }

        yield return new WaitUntil(GameFinished);

        StartCoroutine(GameWin());

    }

    IEnumerator EndAttempt(float time)
    {
        counter = 0;
        checkString = "";
        randomize(4);
        canAdd = false;

        for (int j = 0; j < 4; j++)
        {
            SetAll(0);
            yield return new WaitForSeconds(0.1f);
            SetAll(4);
            yield return new WaitForSeconds(0.1f);
        }
        for (int i = 0; i < hashString.Length; i++)
        {
            if (Input.GetKey(KeyCode.Return)) break;
            int index = int.Parse(hashString[i].ToString()) - 1;
            addCurrent[index] = 0;
            aud.clip = clips[index];
            aud.Play();
            yield return new WaitForSeconds(aud.clip.length);
            addCurrent[index] = 4;
            yield return new WaitForSeconds(time);
        }

        canAdd = true;
    }

    IEnumerator makeGlow(int select)
    {
        addCurrent[select - 1] = 0;
        yield return new WaitForSeconds(0.2f);
        addCurrent[select - 1] = 4;

    }

    void addString(int add)
    {
        aud.clip = clips[add - 1];
        aud.Play();
        checkString += add;
        counter++;
        StartCoroutine(makeGlow(add));
    }

    bool doesConnect(int count)
    {
        if (count >= 0)
            if (hashString[count] != checkString[count]) return true;

        return false;
    }


    void CreateMember(int n)
    {
        //Debug.Log(n);
        int AddPosition = Screen.height - (n * (Screen.height / 4));
        ddr.Add(new DDRPosition(new Vector2(Screen.width, AddPosition), n));
    }

    private void OnGUI()
    {
        if (PuzzleType.Equals(0))
        {
            GUI.DrawTexture(new Rect(Screen.width / 2 - Screen.width / 6, -5, Screen.width / 3, Screen.height / 2), showSprites[3 + addCurrent[3]], ScaleMode.ScaleToFit); // UP
            GUI.DrawTexture(new Rect(Screen.width / 3 - Screen.width / 6, Screen.height * 0.4f, Screen.width / 3, Screen.height / 2), showSprites[0 + addCurrent[0]], ScaleMode.ScaleToFit);  // LEFT
            GUI.DrawTexture(new Rect(Screen.width - Screen.width / 3 - Screen.width / 6, Screen.height * 0.4f, Screen.width / 3, Screen.height / 2), showSprites[2 + addCurrent[2]], ScaleMode.ScaleToFit); // RIGHT
            GUI.DrawTexture(new Rect(Screen.width / 2 - Screen.width / 6, Screen.height * 0.4f, Screen.width / 3, Screen.height / 2), showSprites[1 + addCurrent[1]], ScaleMode.ScaleToFit); //DOWN
        }
        else if (PuzzleType.Equals(1))
        {
            for (int i = 1; i < 5; i++)
            {
                GUI.Box(new Rect(CheckPoint, Screen.height - (i * (Screen.height / 4) - DDRPosition.Size.x / 1.25f), Screen.width, 4), GUIContent.none);
            }


            for (int i = ddr.Count - 1; i >= 0; i--)
            {
                ddr[i].DrawPosition();
                if (!ddr[i].isLegal())
                {
                    missed++;
                    charCont.CameraShake(4);
                    Debug.Log(missed + " Miss");
                    ddr.Remove(ddr[i]);
                    if (missed > 3) StartCoroutine(RestartDance());
                }
            }

        }

    }


    private IEnumerator RestartDance()
    {
        ddr.Clear();
        missed = 0;
        StopCoroutine(DanceMovement);
        randomize(10);
        yield return new WaitForSeconds(1);
        DanceMovement = StartCoroutine(DanceDance());
    }

    void Step(int n) //Get it, it's like DDR hahaha
    {
        DDRPosition temp = null;
        float hold = -100;
        for (int i = ddr.Count - 1; i >= 0; i--)
        {
            if (ddr[i].Value == n)
            {
                if (ddr[i].CheckPosition(out float check))
                {
                    if (check > hold)
                    {
                        temp = ddr[i];
                        hold = check;
                    }
                }
            }
        }

        if (temp != null)
            ddr.Remove(temp);
        else
        {
            missed++;
            charCont.CameraShake(4);
        }
        if (missed > 3) StartCoroutine(RestartDance());

    }

    public bool GameFinished()
    {
        if (ddr.Count > 0)
            return false;
        else return true;
    }


    private void FixedUpdate()
    {
        if (ddr.Count > 0)
        {
            for (int i = 0; i < ddr.Count; i++)
            {
                ddr[i].Move();
            }
        }
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.X)) { StartCoroutine(GameWin()); }

        if (PuzzleType == 1)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) Step(0);
            if (Input.GetKeyDown(KeyCode.DownArrow)) Step(1);
            if (Input.GetKeyDown(KeyCode.RightArrow)) Step(3);
            if (Input.GetKeyDown(KeyCode.UpArrow)) Step(2);
        }

        else if (canAdd)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) addString(1);
            if (Input.GetKeyDown(KeyCode.DownArrow)) addString(2);
            if (Input.GetKeyDown(KeyCode.RightArrow)) addString(3);
            if (Input.GetKeyDown(KeyCode.UpArrow)) addString(4);
        }

        // Right Arrow = Up


        if (counter == length || doesConnect(counter - 1))
        {
            if (checkString == hashString)
                StartCoroutine(GameWin());
            else
            {
                StartCoroutine(EndAttempt(0.2f));
                aud.clip = clips[4];
                aud.Play();
            }


        }





    }
}
