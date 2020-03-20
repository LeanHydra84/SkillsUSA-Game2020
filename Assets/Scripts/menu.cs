using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class transDat
{
    //A LOT MORE WILL HAVE TO BE ADDED TO THIS AAAAAAAAAAA
    public int keys;
    public int health;
    public float time;

    public float[] HoldPosition = new float[2];
    public float x;
    public float y;
    public float z;
}

public class menu : MonoBehaviour, IPointerClickHandler
{

    public static menu instance;

    public Text Press_Label;
    public Image Menu_Br;

    private int MenuStatus;

    static menu()
    {
        newGame = true;
    }

    public static bool newGame;

    public void ContinueGame()
    {
        newGame = false;
        SceneManager.LoadScene("LoadingScreen");
    }

    private void Start()
    {
        instance = this;
        MenuStatus = 0;
        Menu_Br.gameObject.SetActive(false);
        Debug.Log(Application.persistentDataPath);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && MenuStatus == 0) 
        { 
            MenuStatus = 1;
            Press_Label.enabled = false;
            Menu_Br.gameObject.SetActive(true);
        }
    }

    void OnGUI()
    {

        if (MenuStatus == 1)
        {

            if (GUI.Button(new Rect(100, 100, 100, 100), "New Game"))
            {
                newGame = true;
                SceneManager.LoadScene("LoadingScreen");
            }

            if (File.Exists(Application.persistentDataPath + "//save.txt"))
            {
                if (GUI.Button(new Rect(100, 200, 100, 100), "Continue"))
                {
                    ContinueGame();
                }
            }

            if (File.Exists(Application.persistentDataPath + "//save.txt"))
            {
                if (GUI.Button(new Rect(100, 300, 100, 100), "Delete Save"))
                {
                    File.Delete(Application.persistentDataPath + "//save.txt");
                }
            }
        }

    }


    public void OnPointerClick(PointerEventData eventData)
    {
        print("I was clicked");
    }
}
