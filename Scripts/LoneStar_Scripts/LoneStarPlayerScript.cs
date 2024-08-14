using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoneStarPlayerScript : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rb2D;
    [SerializeField] float movementSpeed = 1, laserSpeed = 5;
    [SerializeField] GameObject CursorObject, LaserShotPref;
    bool shootingLaser = false, Iframe = false;
    LoneStarGameHandler loneStarHandler;

    AudioSource civilianSaved, playerDamage;

    // Start is called before the first frame update
    void Start()
    {
        // Comment later
        // Application.targetFrameRate = 65;
        
        anim = transform.GetComponent<Animator>();
        rb2D = transform.GetComponent<Rigidbody2D>();

        loneStarHandler = GameObject.FindObjectOfType<LoneStarGameHandler>();

        civilianSaved = GameObject.Find("loneStarCivilianSaved").GetComponent<AudioSource>();
        playerDamage = GameObject.Find("loneStarInjured").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        TrackAliveCivilians();
        UpdateReticlePosition();

        if (Input.GetButtonDown("Fire1"))
        {
            if (!shootingLaser) FireLaser();
        }
    }

    void TrackAliveCivilians()
    {
        CivilianScript[] loadedCivilians = GameObject.FindObjectsOfType<CivilianScript>();

        for (int i = 0; i < loadedCivilians.Length; i++)
        {
            float dist = Vector2.Distance(transform.localPosition, loadedCivilians[i].gameObject.transform.localPosition);
            if (dist < 0.1f)
            {
                civilianSaved.pitch = Random.Range(0.75f, 1);
                civilianSaved.Play();

                loneStarHandler.CivilianRescued();
                Destroy(loadedCivilians[i].gameObject);
            }
        }
    }

    void UpdateReticlePosition()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        Vector2 MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CursorObject.transform.position = new Vector3(MousePosition.x, MousePosition.y, 0);
    }

    void Movement()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector2 inputVect = new Vector2(x, y);

        if (inputVect != Vector2.zero)
        {
            anim.Play("moving");
        } else
            {
                anim.Play("idle");
            }

        Vector2 Move = (transform.right * x + transform.up * y);

        rb2D.velocity = Move * movementSpeed;
    }

    void FireLaser()
    {
        shootingLaser = true;

        Vector2 tarPos = CursorObject.transform.position;
        GameObject laserShot = Instantiate(LaserShotPref, transform.position, transform.rotation);

        Vector2 shotDir = (tarPos - (Vector2)transform.position).normalized;
        laserShot.GetComponent<Rigidbody2D>().velocity = shotDir * laserSpeed;

        Invoke("LaserCoolDown", 0.25f);
    }

    void LaserCoolDown()
    {
        shootingLaser = false;
    }

    public void LoseLife()
    {
        // Iframe lets the player have a period to get themselves together
        // also prevents instant death if say 3 or more bullets are tightly packed
        if (!Iframe)
        {
            playerDamage.pitch = Random.Range(0.75f, 1);
            playerDamage.Play();

            if (GameObject.FindObjectOfType<LoneStarGameHandler>() != null)
            {
                GameObject.FindObjectOfType<LoneStarGameHandler>().LoseLife();
                StartCoroutine(FlashIndicator());
            } else
                {
                    Debug.LogError("Object Missing! [LoneStarGameHandler]");
                    Application.Quit();
                }
        }
    }

    public void IframePlayer()
    {
        StartCoroutine(FlashIndicator());
    }

    IEnumerator FlashIndicator()
    {
        Iframe = true;

        for (int i = 0; i < 10; i++)
        {
            transform.GetComponent<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(0.1f);
            transform.GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(0.1f);
        }

        transform.GetComponent<SpriteRenderer>().enabled = true;
        Iframe = false;
    }
}//EndScript