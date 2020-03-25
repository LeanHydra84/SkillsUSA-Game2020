using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    private Text myText;
    Color based;

    public Color highlightColor;

    public UnityEvent ClickAction;

    public void OnPointerClick(PointerEventData eventData)
    {
        ClickAction.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        myText.color = highlightColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        myText.color = based;
    }




    // Start is called before the first frame update
    void Start()
    {
        myText = GetComponent<Text>();
        based = myText.color;
    }

    //Possible Methods

    public void QuitGame()
    {
        Application.Quit();
    }

    public void BackToMenu()
    {
        Time.timeScale = 1;
        LoadSceneHandler.Scene = "Menu";
        SceneManager.LoadScene("LoadingScreen");
    }

    public void SaveGame()
    {
        PlayerState.Save();
    }

}
