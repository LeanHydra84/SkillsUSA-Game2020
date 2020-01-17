using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

//Created by DialogScript dialog = AddComponent<DialogScript>().Initialize("name", "TEXT TEXT TEXT TEXT \n TEXT TEXT TEXT");

public class DialogScript : MonoBehaviour
{
    public GUIStyle myStyle;
    private int CanRun = 0;
    private int lineReader;
    public int characterReader;
    private string lineSoFar;
    private float sinceTime;

    private string myName;
    private string[] lines;
    
    private float divSize;
    private float additionConstant;

    public Texture2D background;
    float startTime;
    float showTime;

    void Start()
    {
        Color selectColor = new Color(0, 0, 0, 0.5f);
        background = new Texture2D(1, 1);
        background.SetPixel(0, 0, selectColor);
        background.Apply();
        myStyle = new GUIStyle();
        myStyle.alignment = (TextAnchor)TextAlignment.Center;

    }

    public void Initialize(string name, string text, bool forceRead)
    {
        lines = Regex.Split(text, "\r\n ?|\n");
        CanRun = 1;
        additionConstant = 0.3f;
        startTime = Time.time;
        divSize = 0;
    }

    private void Update()
    {
        //Debug.Log(CanRun + " " + Time.time);
        if(CanRun == 2 && Input.GetKeyDown(KeyCode.Mouse0))
        {
            lineReader++;
            characterReader = 0;
            lineSoFar = "";
            if (lineReader == lines.Length) CanRun++;
        }
    }

    void OnGUI()
    {

        if (CanRun == 1)
        {
            additionConstant = Mathf.Lerp(additionConstant, 0.005f, (Time.time - startTime) * Time.deltaTime*4);
            if (divSize < 0.29) divSize += additionConstant * Time.deltaTime;
            else 
            { 
                CanRun++;
                sinceTime = Time.time; 
            }
        }
        else if (CanRun == 2)
        {

            //DISPLAY LINES
            if(Time.time - sinceTime > 0.2f)
            {
                if (lines[lineReader].Length != characterReader)
                {
                    lineSoFar += lines[lineReader][characterReader];
                    characterReader++;
                }
            }

        }
        else if(CanRun == 3)
        {

        }
        GUI.Label(new Rect(0, Screen.height - Screen.height * 0.2f, Screen.width / 2, Screen.height * 0.2f), lineSoFar, myStyle);
        //GUI.Label(new Rect(Screen.width / 2, Screen.height - Screen.height * 0.2f, Screen.width / 2, Screen.height * 0.2f), lineSoFar, myStyle);
        GUI.DrawTexture(new Rect(0, Screen.height - (Screen.height * divSize), Screen.width, Screen.height * divSize), background, ScaleMode.StretchToFill);

    }

}