using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigame_puzzle : MonoBehaviour
{

    public string hashString;
    public string checkString;
    public int length;
    public int counter;

    private int addCurrent;

    bool canAdd;

    //public GameObject[] showSprites;
    private Texture2D[] showSprites;
    AudioSource aud;


    void Start()
    {
        Object[] sprites = Resources.LoadAll("Minigame_Sprites", typeof(Texture2D));
        showSprites = new Texture2D[8];
        int counter = 0;
        foreach(Object a in sprites)
        {
            showSprites[counter] = (Texture2D)a;
            counter++;
        }

        addCurrent = 0;

        //Randomization goes here
        StartCoroutine(EndAttempt(0.2f));
        aud = GetComponent<AudioSource>();



        addCurrent = 4;
    }

    void GameWin()
    {
        Debug.Log("You win");
        Destroy(this);
    }

    void randomize()
    {
        hashString = "";
        int rand = Random.Range(7, 10);
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
            for (int k = 0; k < 4; k++)
            {
                addCurrent = 4;
            }
            yield return new WaitForSeconds(0.1f);
            for (int k = 0; k < 4; k++)
            {
                addCurrent = 0;
            }
            yield return new WaitForSeconds(0.1f);
        }
        for (int i = 0; i < hashString.Length; i++)
        {
            addCurrent = 0;
            yield return new WaitForSeconds(time);
            addCurrent = 4;
            yield return new WaitForSeconds(time);
        }

        canAdd = true;
    }

    IEnumerator makeGlow(int select)
    {
        addCurrent = 0;
        yield return new WaitForSeconds(0.2f);
        addCurrent = 4;

    }

    void addString(int add)
    {
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
        GUI.DrawTexture(new Rect(Screen.width/2,Screen.height-Screen.height*0.8f,Screen.width/10,Screen.height/5), showSprites[0+addCurrent], ScaleMode.ScaleToFit);
    }

    void Update()
    {


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
                GameWin();

            StartCoroutine(EndAttempt(0.2f));

        }

    }
}
