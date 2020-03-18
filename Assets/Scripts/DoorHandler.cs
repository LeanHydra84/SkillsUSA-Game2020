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
            //Debug.Log(bossDesignation);
            //Debug.Log($"{PlayerState.Keys[bossDesignation - 2]} is the value of the key slot");
            if (PlayerState.Keys[bossDesignation - 2] >= 2)
            {
                locked = false;
            }
        }
        
    }

    void OnEnable()
    {
        if(Open)
        {
            if(gameObject.name == "Single_Door")
            {
                anim.Play("StayOpenSingle");
            }
        else
            anim.Play("StayOpen");
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
