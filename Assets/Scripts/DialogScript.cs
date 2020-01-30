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
    private bool skip;

    void Start()
    {
        mainScript.controls.Controller.ShootFlashlight.started += ctx => skip = false;
        Color selectColor = new Color(0, 0, 0, 0.5f);
        background = new Texture2D(1, 1);
        background.SetPixel(0, 0, selectColor);
        background.Apply();
        myStyle = new GUIStyle();
        //Constant calculated with: 1/72(point size) * 25(reference point amount) * 96(reference dpi) / 539 (reference resolution)
        myStyle.fontSize = (int)(100f / 1617f * Screen.height / Screen.dpi * 72);
        myStyle.fontStyle = FontStyle.Bold;
        myStyle.alignment = (TextAnchor)TextAlignment.Center;
    }

    public void Initialize(string name, string text, bool forceRead)
    {
        lines = Regex.Split(text, "\r\n ?|\n");
        myName = name;
        CanRun = 1;
        additionConstant = 0.3f;
        startTime = Time.time;
        divSize = 0;
        lineSoFar = "";
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0)) skip = true;
        if (CanRun == 2 && skip)
        {
            lineReader++;
            characterReader = 0;
            lineSoFar = "";
            if (lineReader == lines.Length)
            {
                CanRun++;
                startTime = Time.time;
            }
        }
        skip = false;
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

    }

    public static int getNewLines(string str) => Regex.Split(str, "\n").Length;

    void OnGUI()
    {

        if (CanRun == 1)
        {
            ShowHideBanner(x => x < 0.3, 1);
        }
        else if (CanRun == 2)
        {

            //DISPLAY LINES
            if (Time.time - sinceTime > 0.02f)
            {

                if (lines[lineReader].Length != characterReader)
                {
                    if (characterReader > 45 * getNewLines(lineSoFar)) lineSoFar = lineSoFar.Insert(lineSoFar.LastIndexOf(' '), "\n");
                    lineSoFar += lines[lineReader][characterReader];
                    characterReader++;
                    sinceTime = Time.time;
                }
            }

        }
        else if (CanRun == 3)
        {
            additionConstant = 0.3f;

            ShowHideBanner(x => x > float.Epsilon, -1);

        }
        else if (CanRun == 4)
        {
            lineReader = 0;
            characterReader = 0;
            lineSoFar = "";
            CanRun = 0;
        }


        GUI.Label(new Rect(Screen.width / 2, Screen.height - Screen.height * 0.2f, 0, 0), lineSoFar, myStyle);
        GUI.Label(new Rect(Screen.width * 0.2f, Screen.height - ((Screen.height * divSize) - Screen.height / 90), 0, 0), myName, myStyle);
        GUI.DrawTexture(new Rect(0, Screen.height - (Screen.height * divSize), Screen.width, Screen.height * divSize), background, ScaleMode.StretchToFill);

    }

}
