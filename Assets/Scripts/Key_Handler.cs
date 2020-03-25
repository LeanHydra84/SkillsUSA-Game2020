using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void updateSprite(int designation);

public class Key_Handler : MonoBehaviour
{
    public static updateSprite update;
    public int keyNumber;
    public int bossNumber { get; private set; }
    public GameObject ambient_player;

    public static Sprite[] KeySprites;
    private SpriteRenderer spr;

    private static GameObject player_character;
    [HideInInspector] public bool isSecond { get; private set; }

    void Start()
    {
        
        update += recalculateImages;
        spr = GetComponent<SpriteRenderer>();
        bossNumber = PlayerState.Bosses[keyNumber-1];
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
        gameObject.SetActive(false);
    }

    /*private void OnEnable()
    {
        recalculateImages(keyNumber);
    }*/

    void recalculateImages(int des)
    {
        
        if(PlayerState.Keys[keyNumber - 1] == 1)
        {
            isSecond = true;
            spr.sprite = KeySprites[bossNumber * 2 - 1];
            update -= recalculateImages;
        }

            
    }

    

    public void win()
    {
        PlayerState.Keys[keyNumber-1]++;
        charCont.shouldBeFocused = true;
        update(keyNumber);
        Destroy(ambient_player);
        Destroy(gameObject);
    }


    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - charCont.mainCam.transform.position);
    }
}
