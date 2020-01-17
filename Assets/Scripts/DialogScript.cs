using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

//Created by DialogScript dialog = AddComponent<DialogScript>().Initialize("name", "TEXT TEXT TEXT TEXT \n TEXT TEXT TEXT");

delegate bool divCheck(float n);

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

    private Texture2D background;
    float startTime;
    float showTime;

    void Start()
    {
        Color selectColor = new Color(0, 0, 0, 0.5f);
        background = new Texture2D(1, 1);
        background.SetPixel(0, 0, selectColor);
        background.Apply();
        myStyle = new GUIStyle();
        myStyle.fontSize = 25;
        myStyle.fontStyle = FontStyle.Bold;
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

    void ShowHideBanner(divCheck dc, int mult)
    {
        additionConstant = Mathf.Lerp(additionConstant, 0.006f, (Time.time - startTime) * 0.06f);
        if (dc(divSize)) divSize += additionConstant * 0.018f * mult;
        else
        {
            CanRun++;
            sinceTime = Time.time;
        }
        
        Debug.Log(Time.deltaTime);

    }


    void OnGUI()
    {

        if (CanRun == 1)
        {
            ShowHideBanner(x => x < 0.3, 1);
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
            additionConstant = 0.3f;
            ShowHideBanner(x => x > 0, -1);
        }
        else if(CanRun == 4)
        {
            lineReader = 0;
            characterReader = 0;
            lineSoFar = "";
            CanRun = 0;
        }


        GUI.Label(new Rect(Screen.width/2, Screen.height - Screen.height * 0.2f, 0, 0), lineSoFar, myStyle);
        GUI.DrawTexture(new Rect(0, Screen.height - (Screen.height * divSize), Screen.width, Screen.height * divSize), background, ScaleMode.StretchToFill);

    }

}