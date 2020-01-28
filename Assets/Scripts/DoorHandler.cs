using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHandler : MonoBehaviour
{
    
    public bool Open { get; set; }
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void OpenCloseDoor()
    {
        if (Open) anim.SetTrigger("Close");
        else anim.SetTrigger("Open");
        Open = !Open;
    }

}
