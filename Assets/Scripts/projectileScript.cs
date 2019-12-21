using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectileScript : MonoBehaviour
{
    float beginTime;
    public int damage;
    public int bounceCount = 0;
    public bool isShrap;
    public bool pickupAble;
    private SpriteRenderer itsAbigBattle;
    private Sprite startingTexture;
    Camera mc;

    public Sprite StartingTexture
    {
        get { return startingTexture; }
        set
        {
            if (itsAbigBattle == null) itsAbigBattle = transform.GetChild(0).GetComponent<SpriteRenderer>();
            startingTexture = value;
            itsAbigBattle.sprite = startingTexture;
        }
    }


    IEnumerator fadeOut()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        float opacity = 1;
        while (opacity > 0)
        {
            itsAbigBattle.color = new Color(1, 1, 1, opacity);
            opacity -= 0.05f;
            yield return new WaitForSeconds(0.01f);
        }
        System.GC.Collect();
        Destroy(gameObject);
    }


    private void Start()
    {
        mc = Camera.main;
        beginTime = Time.time;
    }

    private void Update()
    {
        itsAbigBattle.transform.rotation = Quaternion.LookRotation(transform.position - mc.transform.position);
        if (Time.time - beginTime > 6f)
        {
            StartCoroutine(fadeOut());
        }

    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.collider.gameObject.tag == "Player")
        {
            PlayerState.Health -= damage;
            Destroy(gameObject);
        }

        if (col.gameObject.tag != "boss" && col.gameObject.tag != "Player" && isShrap)
        {
            bossFight.RingAttack(transform, 2, transform.rotation);
        }

        if (bounceCount == 0 && Time.time - beginTime > 0.1f)
            StartCoroutine(fadeOut());
        else if (bounceCount > 0)
        {
            StartingTexture = bossFight.spriteNames["Bounce" + bounceCount];
            bounceCount--;
        }
    }
}
