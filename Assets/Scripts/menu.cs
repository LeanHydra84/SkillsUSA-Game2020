using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;


public static class Settings
{

    static Settings()
    {
        volume = 100;
    }

    static int volume;
    public static int Volume 
    { 
        get => volume;
        set
        {
            if (value >= 0 && value <= 100)
                volume = value;
        }
    }

}

[Serializable]
public class transDat
{
    //A LOT MORE WILL HAVE TO BE ADDED TO THIS AAAAAAAAAAA
    public int[] keys;
    public int health;
    public float time;

    public float x;
    public float y;
    public float z;

    public int[] Bosses;
    public int[] PuzzlePositions;
    public bool[] Victories;
    public bool hasMask;
}

public class menu : MonoBehaviour, IPointerClickHandler
{

    public Dictionary<string, Action> Buttons;

    public static menu instance;

    public Text Press_Label;
    public Image Menu_Br;

    public GameObject[] ButtonLayout;

    private int MenuStatus;



    static menu()
    {
        newGame = true;
    }

    public static bool newGame;

    public void ContinueGame(bool newG)
    {
        newGame = newG;
        LoadSceneHandler.Scene = 2;
        SceneManager.LoadScene(1);
    }

    void EnableButton()
    {
        int a = MenuStatus - 1;
        for (int i = 0; i < ButtonLayout.Length; i++)
            ButtonLayout[i].SetActive(i == a);
    }

    private void Start()
    {
        print($"{Screen.width} x {Screen.height}");
        instance = this;
        DefineButtons();
        MenuStatus = 0;
        Menu_Br.gameObject.SetActive(false);
        Debug.Log(Application.persistentDataPath);

    }

    void DefineButtons()
    {
        Buttons = new Dictionary<string, Action>();
        Buttons.Add("newGame", () => ContinueGame(true));
        Buttons.Add("loadGame", () => ContinueGame(false));
        Buttons.Add("options", () => ShowMenu(2));
        Buttons.Add("quit", () => Application.Quit());
        Buttons.Add("back-options", () => ShowMenu(1));
        Buttons.Add("sound-settings", () => ShowMenu(3));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && MenuStatus == 0)
        {
            ShowMenu(1);
            Press_Label.enabled = false;
            Menu_Br.gameObject.SetActive(true);
        }
    }

    void ShowMenu(int a)
    {
        MenuStatus = a;
        EnableButton();
    }

    void OnGUI()
    {

        if (MenuStatus == 1)
        {

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
