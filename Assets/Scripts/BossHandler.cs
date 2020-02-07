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
    private static Dictionary<string, string> names = new Dictionary<string, string>();
    [HideInInspector] public string myName;

    private int BossIndex; //Which boss out of the randomized

    static BossHandler()
    {
        BossNames = new string[4];
        Lines = new string[4];
        BossNames[0] = "Flute";
        BossNames[1] = "Harp";
        BossNames[2] = "Trombone";
        BossNames[3] = "Violin";

        Lines[0] = "Oh, is that my flute?\nThank you child, this is a kind thing you have done.\nWhy don’t I play you a song to show my gratitude.\nBe warned, this tune is a tad… energetic.";
        Lines[1] = "You must be lost my sweet child.\nDo not worry, for I shall protect you from the evils that haunt this house.\nCome, and I will play you a lullaby.";
        Lines[2] = "Ah Ha! Another brass brother!\nWelcome, do you wish to battle?\nAlthough you are young, I make no promises for your safety!";
        Lines[3] = "What the- Is that my violin?\nWhere did you get that!\nThief! For that I will teach you a lesson you’ll never forget!";

    }

    void Start()
    {
        BossIndex = PlayerState.Bosses[BossNumber-1]-1;
        Debug.Log(BossIndex);
        myName = BossNames[BossIndex];
        foreach (GameObject x in arr) x.SetActive(false);
        transform.Find(myName).gameObject.SetActive(true);
        
        
        if (player == null) player = GameObject.Find("Player");
    }

    public void Interact()
    {
        mainScript.dg.Initialize(myName, Lines[BossIndex], myName);
    }

}
