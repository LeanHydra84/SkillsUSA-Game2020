using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverHandler : MonoBehaviour
{
    private Image meImage;
    public static Sprite[] Screens;
    private int current = 0;
    public int CurrentShown
    {
        get => current;

        set
        {
            if(value != current)
            {
                meImage.sprite = Screens[value];
                current = value;
            }
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        meImage = GetComponent<Image>();
        if(Screens == null)
        {
            Screens = new Sprite[3];
            Object[] textures = Resources.LoadAll("GameOver", typeof(Sprite));
            for(int i = 0; i < textures.Length; i++)
            {
                Screens[i] = (Sprite)textures[i];
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            switch(CurrentShown)
            {
                case 2:
                    LoadSceneHandler.Scene = "Alright";
                    SceneManager.LoadScene("LoadingScreen");
                    break;
                case 1:
                    Application.Quit();
                    break;
                default:
                    Debug.Log("Nice click, moron");
                    break;
            }
        }
    }

    private void OnGUI()
    {
        Event current = Event.current;
        Vector2 mousePosition = new Vector2();

        mousePosition.x = current.mousePosition.x;
        mousePosition.y = Screen.height - current.mousePosition.y;

        if(mousePosition.y > Screen.height*0.3f)
        {
            CurrentShown = 0;
        }
        else if(mousePosition.x > Screen.width/2)
        {
            CurrentShown = 1;
        }
        else
        {
            CurrentShown = 2;
        }


    }

}
