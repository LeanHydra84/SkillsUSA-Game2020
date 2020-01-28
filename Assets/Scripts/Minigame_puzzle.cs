using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigame_puzzle : MonoBehaviour
{

    public string hashString;
    public string checkString;
    public int length;
    public int counter;

    bool canAdd;

    public GameObject[] showSprites;
    AudioSource aud;

    Color fullVis;
    Color deadFoxtrot;

    void Start()
    {
        //Randomization goes here
        StartCoroutine(EndAttempt(0.2f));
        aud = GetComponent<AudioSource>();

        fullVis = new Color(1, 1, 1, 1);
        deadFoxtrot = new Color(1, 1, 1, 0.3f);

        foreach (GameObject g in showSprites)
            g.GetComponent<SpriteRenderer>().color = deadFoxtrot;
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
                showSprites[k].GetComponent<SpriteRenderer>().color = fullVis;
            }
            yield return new WaitForSeconds(0.1f);
            for (int k = 0; k < 4; k++)
            {
                showSprites[k].GetComponent<SpriteRenderer>().color = deadFoxtrot;
            }
            yield return new WaitForSeconds(0.1f);
        }
        for (int i = 0; i < hashString.Length; i++)
        {
            SpriteRenderer spr = showSprites[int.Parse(hashString[i].ToString()) - 1].GetComponent<SpriteRenderer>();
            spr.color = fullVis;
            yield return new WaitForSeconds(time);
            spr.color = deadFoxtrot;
            yield return new WaitForSeconds(time);
        }

        canAdd = true;
    }

    IEnumerator makeGlow(int select)
    {
        SpriteRenderer spr = showSprites[select - 1].GetComponent<SpriteRenderer>();
        spr.color = fullVis;
        yield return new WaitForSeconds(0.2f);
        spr.color = deadFoxtrot;

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
