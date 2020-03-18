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
    public bool dynamic = false;
    public bool returnFire = false;
    private SpriteRenderer itsAbigBattle;
    private Sprite startingTexture;
    private Light glow;
    Camera mc;

    public Sprite StartingTexture
    {
        get => startingTexture;
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
        GetComponent<Collider>().enabled = false;
        float opacity = 1;
        while (opacity > 0)
        {
            glow.intensity = opacity;
            itsAbigBattle.color = new Color(1, 1, 1, opacity);
            opacity -= 0.05f;
            yield return new WaitForSeconds(0.01f);
        }
        System.GC.Collect();
        Destroy(gameObject);
    }

    IEnumerator ShowAfterTime(float time)
    {
        itsAbigBattle.enabled = false;
        yield return new WaitForSeconds(time);
        itsAbigBattle.enabled = true;
    }

    private void Start()
    {

        if (pickupAble) mainScript.controls.Controller.Interact.started += e => PickUp();
        mc = Camera.main;
        beginTime = Time.time;
        glow = transform.GetChild(1).GetComponent<Light>();
        if (dynamic)
            StartCoroutine(ShowAfterTime(0.01f));
    }

    private void Update()
    {
        itsAbigBattle.transform.rotation = Quaternion.LookRotation(transform.position - mc.transform.position);
        if (Time.time - beginTime > 6f)
        {
            StartCoroutine(fadeOut());
        }

        if (Input.GetKeyDown(KeyCode.E)) PickUp();

    }

    void PickUp()
    {
        if (pickupAble)
        {
            if (Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) < 5)
            {
                PlayerState.Ammo++;
                mainScript.controls.Controller.Interact.started -= e => PickUp();
                Destroy(gameObject);
            }
        }
    }


    private bool isWall(Collision col)
    {
        return (col.gameObject.tag != "boss" && col.gameObject.tag != "Player") ? true : false;
    }

    private void OnCollisionEnter(Collision col)
    {
        if (dynamic && isWall(col) && beginTime - Time.time < 0.01f) Destroy(gameObject);

        if(returnFire && col.gameObject.tag == "boss")
        {
            col.gameObject.GetComponent<bossFight>().BossHealth--;
            StartCoroutine(fadeOut());
        }

        if (col.collider.gameObject.tag == "Player" && PlayerState.IsInBossFight)
        {
            PlayerState.Health -= damage;
            Destroy(gameObject);
        }

        if (isWall(col) && isShrap)
        {
            //MOVE AWAY FROM WALL
            transform.position += col.contacts[0].normal / 4;
            bossFight.RingAttack(transform, 2, transform.rotation, true);
            Destroy(gameObject);
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
