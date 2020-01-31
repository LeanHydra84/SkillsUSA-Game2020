using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostDialogHandler : MonoBehaviour
{
    public string name;
    public string lines;

    private void Start()
    {
        if(name == "Uncle Vicente") lines = "It’s my least favorite nephew!\nWhy did you have to come to my rescue?\n...\nStill no mouth, huh?\nWhile I was performing Clair de Lune, the piano slipped and I was unfortunately crushed\nIt was probably those damn ghosts\n...\nDon't look so wide-eyed, of course my mansions is haunted\nIt's also the reason why I can’t move\nIt’s almost like they cursed me to the piano!\nMaybe if you finish the song I was playing…\nAlthough you happen to be the worst player in the family\nIf only you were more like the rest of the family, instead of choosing trumpet and wanting jazz.\nIt’s fine, just go find the sheet music, and play the pia-\nTHESE LITTLE  BUGGERS STOLE THE KEYS RIGHT OFF MY PIANO!\nListen, let's make a deal, if you can get those piano keys from those pesky ghosts then I can help you with your trumpet playing\nAs long as you don’t run into any Red ghosts, you should be okay.\nThose are the one’s with malicious intent, they might just be the ones who stole them.\nYou should hurry, otherwise I might end up haunting you!";
    }

}
