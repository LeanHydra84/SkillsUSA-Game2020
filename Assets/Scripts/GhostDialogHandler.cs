using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GhostDialogHandler : MonoBehaviour
{
    public string ghostName;
    public string[] script;
    private int ln;
    [HideInInspector] public int LineNumber { get => ln; set { if (value < script.Length) ln = value; } }
    public string lines
    {
        get => script[ln];
    }

    

    private void Start()
    {

    }


}
