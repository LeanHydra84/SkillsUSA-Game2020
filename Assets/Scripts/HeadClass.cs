using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadClass : MonoBehaviour
{

    public static HeadClass instance;

    private void Start()
    {
        instance = this;
    }

    [SerializeField] private Sprite[] Accordion;
    [SerializeField] private Sprite[] Bassoon;
    [SerializeField] private Sprite[] Cello;
    [SerializeField] private Sprite[] Clarinet;
    [SerializeField] private Sprite[] Flute;
    [SerializeField] private Sprite[] Harp;
    [SerializeField] private Sprite[] Trombone;
    [SerializeField] private Sprite[] Violin;

    public Sprite Pull(string boss, int i)
    {
        boss = boss.ToLower();
        
        switch(boss)
        {
            case "accordion":
                return Accordion[i];
            case "bassoon":
                return Bassoon[i];
            case "cello":
                return Cello[i];
            case "clarinet":
                return Accordion[i];
            case "flute":
                return Flute[i];
            case "harp":
                return Harp[i];
            case "trombone":
                return Trombone[i];
            case "violin":
                return Violin[i];
            default:
                return null;
        }
    }

}
