using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void updateSprite(int designation);

public class Key_Handler : MonoBehaviour
{
    public static updateSprite update;
    public int keyNumber;
    public int bossNumber { get; private set; }

    public static Sprite[] KeySprites;
    private SpriteRenderer spr;

    void Start()
    {
        
        update += recalculateImages;
        spr = GetComponent<SpriteRenderer>();

        bossNumber = PlayerState.Bosses[keyNumber-1];
        Debug.Log(bossNumber);
        if(KeySprites == null)
        {
            Object[] sprite_preload = Resources.LoadAll("Keys", typeof(Sprite));
            KeySprites = new Sprite[sprite_preload.Length];
            for (int i = 0; i < KeySprites.Length; i++)
            {
                KeySprites[i] = (Sprite)sprite_preload[i];
            }
        }

        spr.sprite = KeySprites[bossNumber * 2 - 2];

    }
    
    void recalculateImages(int des)
    {
        if (des == keyNumber)
        {
            spr.sprite = KeySprites[bossNumber * 2 - 1];
            update -= recalculateImages;
        }
            
    }

    

    public void win()
    {
        PlayerState.Keys[keyNumber-1]++;
        charCont.shouldBeFocused = true;
        update(keyNumber);
        Destroy(gameObject);
    }


    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - charCont.mainCam.transform.position);
    }
}
