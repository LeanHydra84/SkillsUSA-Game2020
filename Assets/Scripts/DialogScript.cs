using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.UI;

//Created by DialogScript dialog = AddComponent<DialogScript>().Initialize("name", "TEXT TEXT TEXT TEXT \n TEXT TEXT TEXT");

delegate bool divCheck(float n);

public class DialogScript : MonoBehaviour
{
    private static Animator Box;
    private static Text nameText;
    private static Image Head;
    public GUIStyle myStyle;
    private int canrun;
    public int CanRun
    {
        get => canrun;
        set
        {
            canrun = value;
            if (value == 1) StartCoroutine(FadeBox("FadeIn"));
            if (value == 3) StartCoroutine(FadeBox("FadeOut"));
        }
    }
    private int lineReader;
    public int characterReader;
    private string lineSoFar;
    private float sinceTime;
    private bool NotBoss;
    private string myName;
    private string[] lines;

    private string isBoss;

    private Texture2D background;

    private bool skip;

    void Start()
    {
        if (Box == null)
        {
            Box = GameObject.Find("DiagBox").GetComponent<Animator>();
            nameText = Box.transform.GetChild(0).GetComponent<Text>();
            Head = Box.transform.GetChild(1).GetComponent<Image>();
        }

        mainScript.controls.Controller.ShootFlashlight.started += ctx => skip = true;
        Color selectColor = new Color(0, 0, 0, 0.5f);
        background = new Texture2D(1, 1);
        background.SetPixel(0, 0, selectColor);
        background.Apply();
        myStyle = new GUIStyle();
        //Constant calculated with: 1/72(point size) * 25(reference point amount) * 96(reference dpi) / 539 (reference resolution) HAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAHAH
        myStyle.fontSize = (int)(100f / 1617f * Screen.height / Screen.dpi * 72);
        myStyle.fontStyle = FontStyle.Bold;
        myStyle.normal.textColor = Color.black;
        myStyle.alignment = (TextAnchor)TextAlignment.Center;
    }

    public void Initialize(string name, string text, string ib)
    {
        lines = Regex.Split(text, ":=");
        NotBoss = ib == "";
        myName = (NotBoss) ? name : BossHandler.names[name];
        nameText.text = myName;
        Head.gameObject.SetActive(!NotBoss);
        Head.sprite = HeadClass.instance.Pull(ib, 0);
        CanRun = 1;
        lineSoFar = "";
        isBoss = ib;
        charCont.isInDialog = true;
    }

    IEnumerator FadeBox(string animation)
    {
        Box.SetTrigger(animation);
        yield return new WaitForSeconds(1f);
        CanRun++;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) skip = true;
        if (CanRun == 2 && skip && !mainScript.isInPauseMenu)
        {
            lineReader++;
            characterReader = 0;
            lineSoFar = "";
            
            if (lineReader == lines.Length)
            {
                CanRun++;
            }
            else if (!NotBoss) Head.sprite = HeadClass.instance.Pull(isBoss, lineReader);
        }
        skip = false;
    }

    public static int getNewLines(string str) => Regex.Split(str, "\n").Length;

    Rect textPosition = new Rect(Screen.width / 2, Screen.height - Screen.height * 0.26f, 0, 0);

    void OnGUI()
    {

        if (CanRun == 2)
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

            GUI.Label(textPosition, lineSoFar, myStyle);

        }

        else if (CanRun == 4)
        {
            lineReader = 0;
            characterReader = 0;
            lineSoFar = "";
            CanRun = 0;
            if (isBoss != "")
            {
                charCont.isInEndBossFight = true;
                mainScript.instance.StartBossFight(isBoss);
            }
            charCont.isInDialog = false;
        }


        

    }

}
