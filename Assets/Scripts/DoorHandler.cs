using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHandler : MonoBehaviour
{

    public bool locked;
    public int bossDesignation;

    public bool Open { get; set; }
    private Animator anim;

    void checkLockStatus(int disRegard)
    {
        if(bossDesignation > 0)
        {
            Debug.Log($"{PlayerState.Keys[bossDesignation - 1]} is the value of the key slot");
            if (PlayerState.Keys[bossDesignation - 1] >= 2)
            {
                locked = false;
            }
        }
        
    }

    void Start()
    {
        bossDesignation++;
        anim = GetComponent<Animator>();
        Key_Handler.update += checkLockStatus;
    }

    public void OpenCloseDoor()
    {
        if(!locked)
        {
            if (Open) anim.SetTrigger("Close");
            else anim.SetTrigger("Open");
            Open = !Open;
        }
        
    }

}
