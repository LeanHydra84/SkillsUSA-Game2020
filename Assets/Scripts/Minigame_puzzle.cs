using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigame_puzzle : MonoBehaviour
{
    public string hashString;
    public string checkString;
    public int length;
    public int counter;

    private int[] addCurrent;

    bool canAdd;

    //public GameObject[] showSprites;
    private static Texture2D[] showSprites;
    public static AudioClip[] clips;
    private AudioSource aud;

    void SetAll(int a) { for (int i = 0; i < 4; i++) { addCurrent[i] = a; } }

    void Start()
    {

        mainScript.controls.Controller.MG_Up.started += ctx => addString(4);
        mainScript.controls.Controller.MG_Down.started += ctx => addString(2);
        mainScript.controls.Controller.MG_Left.started += ctx => addString(1);
        mainScript.controls.Controller.MG_Right.started += ctx => addString(3);

        if(clips == null)
        {
            Object[] sprites = Resources.LoadAll("Minigame_Sprites", typeof(Texture2D));
            Object[] trumpet_sounds = Resources.LoadAll("Trumpet_Sounds", typeof(AudioClip));

            showSprites = new Texture2D[8];
            clips = new AudioClip[5];

            int counter = 0;
            foreach (Object a in sprites)
            {
                showSprites[counter] = (Texture2D)a;
                counter++;
            }

            counter = 0;
            foreach (Object x in trumpet_sounds)
            {
                clips[counter] = (AudioClip)x;
                counter++;
            }
        }
        


        addCurrent = new int[4];
        SetAll(0);

        //Randomization goes here
        StartCoroutine(EndAttempt(0.2f));
        aud = GetComponent<AudioSource>();


        SetAll(4);
    }

    IEnumerator GameWin()
    {
        Debug.Log("You win");
        yield return new WaitForSeconds(aud.clip.length);
        mainScript.AllRooms.Remove(gameObject);
        mainScript.dontCheck.Add(gameObject);
        Destroy(gameObject);
        GetComponent<Key_Handler>().win();
    }

    void randomize()
    {
        hashString = "";
        int rand = Random.Range(4, 5);
        for (int i = 0; i < rand; i++)
        {
            hashString += Random.Range(1, 5).ToString();
        }
        length = hashString.Length;
    }


    IEnumerator EndAttempt(float time)
    {
        counter = 0;
        checkString = "";
        randomize();
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
        addCurrent[select-1] = 0;
        yield return new WaitForSeconds(0.2f);
        addCurrent[select-1] = 4;

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

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(Screen.width / 2 - Screen.width / 6, -5, Screen.width / 3, Screen.height / 2), showSprites[3 + addCurrent[3]], ScaleMode.ScaleToFit); // UP
        GUI.DrawTexture(new Rect(Screen.width / 3 - Screen.width / 6, Screen.height * 0.4f, Screen.width / 3, Screen.height / 2), showSprites[0 + addCurrent[0]], ScaleMode.ScaleToFit);  // LEFT
        GUI.DrawTexture(new Rect(Screen.width - Screen.width / 3 - Screen.width / 6, Screen.height * 0.4f, Screen.width / 3, Screen.height / 2), showSprites[2 + addCurrent[2]], ScaleMode.ScaleToFit); // RIGHT
        GUI.DrawTexture(new Rect(Screen.width / 2 - Screen.width / 6, Screen.height * 0.4f, Screen.width / 3, Screen.height / 2), showSprites[1 + addCurrent[1]], ScaleMode.ScaleToFit); //DOWN
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X)) { StartCoroutine(GameWin()); }

        if (canAdd)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) addString(1);
            if (Input.GetKeyDown(KeyCode.DownArrow)) addString(2);
            if (Input.GetKeyDown(KeyCode.RightArrow)) addString(3);
            if (Input.GetKeyDown(KeyCode.UpArrow)) addString(4);
        }

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
