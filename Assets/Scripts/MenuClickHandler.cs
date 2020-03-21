using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using System;

static class ClickActions
{

}

public class MenuClickHandler : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    public string function;

    public bool Locked;

    Text MeText;

    private static readonly Color based = Color.black;
    public Color newColor;
    public Color lockColor;

    private void Start()
    {
        MeText = GetComponent<Text>();
        if (Locked) MeText.color = lockColor;
    }

    void Lock(bool f)
    {
        Locked = f;
        if (f) MeText.color = lockColor;
    }

    private void Update()
    {
        if (function == "loadGame")
        {
            if (File.Exists(Application.persistentDataPath + "//save.txt"))
                Lock(false);
            else Lock(true);
        }
    }

    private void OnEnable()
    {
        if(MeText != null)
        if (!Locked) MeText.color = based;
    }

    public void Clicked()
    {
        menu.instance.Buttons[function]();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!Locked)
            Clicked();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!Locked)
            MeText.color = newColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Locked) MeText.color = lockColor;
        else MeText.color = based;
    }
}
