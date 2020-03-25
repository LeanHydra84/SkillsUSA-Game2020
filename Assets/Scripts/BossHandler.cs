using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHandler : MonoBehaviour
{
    public static string[] BossNames;


    private static string[] Lines;

    //Flute
    //Harp
    //Trombone
    //Violin

    private static GameObject player;

    public GameObject[] arr;

    public int BossNumber; //Which boss in order
    public static Dictionary<string, string> names = new Dictionary<string, string>(8);
    [HideInInspector] public string myName;

    private int BossIndex; //Which boss out of the randomized

    static BossHandler()
    {
        BossNames = new string[8];
        Lines = new string[8];
        BossNames[0] = "Accordion";
        BossNames[1] = "Bassoon";
        BossNames[2] = "Cello";
        BossNames[3] = "Clarinet";
        BossNames[4] = "Flute";
        BossNames[5] = "Harp";
        BossNames[6] = "Trombone";
        BossNames[7] = "Violin";

        names.Add("Accordion", "Thalia");
        names.Add("Bassoon", "Flip");
        names.Add("Cello", "Vode");
        names.Add("Clarinet", "Odric");
        names.Add("Flute", "Florence");
        names.Add("Harp", "Gillea");
        names.Add("Trombone", "Desmark");
        names.Add("Violin", "Melissa");

        Lines[0] = "I hear you are the worst musician to set foot in this house.:=Why are you even here?:=You will only embarrass yourself trying to outplay a master like myself.:=Still, I'll humor you";
        Lines[1] = "You must be the new guest here.:=I don’t see why anyone would come to this place by choice.:=Nothing good has happened in this house since the day it was built.:=I must give it some credit,  however.:=It houses some of the greatest musicians to ever walk this earth.:=Why don’t you join us?";
        Lines[2] = "Hmm...:=Was that your uncle who was crushed by the piano?:=He is not a good man.:=Whatever he put you up to can be nothing but trouble.:=I don’t want to hurt a child, but stopping a man like your uncle...:=Now that might just change my mind.";
        Lines[3] = "Your foolish uncle tried to stop us, but that old coward can’t imagine the pain he caused.:=Come hither young one:=It is time to pay for your uncle’s crime.";
        Lines[4] = "Oh, is that my flute?:=Thank you child, this is a kind thing you have done.:=Why don’t I play you a song to show my gratitude.:=Be warned, this tune is a tad… energetic.";
        Lines[5] = "You must be lost my sweet child.:=Do not worry, for I shall protect you from the evils that haunt this house.:=Come, and I will play you a lullaby.";
        Lines[6] = "Ah Ha! Another brass brother!:=Welcome, do you wish to battle?:=Although you are young, I make no promises for your safety!";
        Lines[7] = "What the- Is that my violin?:=Where did you get that!:=Thief! For that I will teach you a lesson you’ll never forget!";
        

    }

    void Start()
    {
        BossIndex = PlayerState.Bosses[BossNumber-1]-1;
        myName = BossNames[BossIndex];
        foreach (GameObject x in arr) x.SetActive(false);
        transform.Find(myName).gameObject.SetActive(true);
        
        
        if (player == null) player = GameObject.Find("Player");
    }

    public void Interact()
    {
        bossFight.BossCorner = BossNumber;
        mainScript.dg.Initialize(myName, Lines[BossIndex], myName);
    }

}
