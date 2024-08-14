using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogMovement : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] int jumpDist = 1;
    [SerializeField] float intervalDelay = 0.5f;

    AudioSource froggieJump, froggieDeath;

    FroggieGameHandler gameHandler;

    [SerializeField] bool isDead = false, isMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        // Comment later
        // Application.targetFrameRate = 65;

        froggieJump = GameObject.Find("froggieJump").GetComponent<AudioSource>();
        froggieDeath = GameObject.Find("froggieDie").GetComponent<AudioSource>();
        
        gameHandler = GameObject.FindObjectOfType<FroggieGameHandler>();
        Invoke("RepairMovement", 0.1f);
    }

    void RepairMovement()
    {
        isDead = false;
        isMoving = false;
    }

    public void ResetFroggie()
    {
        Invoke("RepairMovement", 0.1f);
    }

    void Update()
    {
        if (!isDead) Movement();
    }

    IEnumerator MoveFrog(Vector2 nPos)
    {
        // slowly move the frog to its new location
        if (Vector2.Distance(transform.position, nPos) > 0.0001f)
        {
            transform.position = Vector2.MoveTowards(transform.position, nPos, 0.25f);
            yield return new WaitForSeconds(intervalDelay);
            StartCoroutine(MoveFrog(nPos));
        }
        else
        {
            transform.position = nPos;

            // check where the player landed is a safe space
            if (gameHandler.SafeLanding(transform.localPosition))
            {
                isMoving = false;
            }
            else // frog landed in water
            {
                FrogHasDied();
            }

            yield return null;
        }
    }

    public Vector2 AlignPosition(Vector2 pos)
    {
        int xBase = (int)pos.x;

        // pos.x = 1.84f :: xBase = 1 :: 

        if (Mathf.Abs(pos.x - (xBase - 0.5f)) <= Mathf.Abs(pos.x - (xBase + 0.5f)))
        {
            pos.x = (xBase - 0.5f);
            return pos;
        }

        if (Mathf.Abs(pos.x - (xBase + 0.5f)) < Mathf.Abs(pos.x - (xBase - 0.5f)))
        {
            pos.x = (xBase + 0.5f);
            return pos;
        }

        return pos;
    }

    void Movement()
    {
        if (!isMoving)
        {
            // Move Up
            if ((Input.GetKeyDown(KeyCode. UpArrow) || Input.GetKeyDown(KeyCode. W)) && transform.localPosition.y < 5) // frog jumps outside of camera view to goto next chunk
            {
                anim.transform.localEulerAngles = new Vector3(0,0,0);

                Vector2 nPos = new Vector2(transform.position.x, transform.position.y + jumpDist);

                if (transform.parent == gameHandler.transform) nPos = AlignPosition(nPos);

                if (gameHandler.ValidMovement(nPos))
                {
                    isMoving = true;
                    anim.Play("jump");

                    froggieJump.pitch = Random.Range(0.75f, 1);
                    froggieJump.Play();

                    transform.SetParent(gameHandler.transform);
                    StartCoroutine(MoveFrog(nPos));
                }
            }

            // Move Down
            if ((Input.GetKeyDown(KeyCode. DownArrow) || Input.GetKeyDown(KeyCode.S)) && transform.localPosition.y > -4)
            {
                anim.transform.localEulerAngles = new Vector3(0, 0, 180);

                Vector2 nPos = new Vector2(transform.position.x, transform.position.y - jumpDist);

                if (transform.parent == gameHandler.transform) nPos = AlignPosition(nPos);

                if (gameHandler.ValidMovement(nPos))
                {
                    isMoving = true;
                    anim.Play("jump");

                    froggieJump.pitch = Random.Range(0.75f, 1);
                    froggieJump.Play();

                    transform.SetParent(gameHandler.transform);
                    StartCoroutine(MoveFrog(nPos));
                }
            }

            // Move Left
            if ((Input.GetKeyDown(KeyCode. LeftArrow) || Input.GetKeyDown(KeyCode.A)) && transform.localPosition.x > -4)
            {
                anim.transform.localEulerAngles = new Vector3(0, 0, 90);

                Vector2 nPos = new Vector2(transform.position.x - jumpDist, transform.position.y);

                if (transform.parent == gameHandler.transform) nPos = AlignPosition(nPos);

                if (gameHandler.ValidMovement(nPos))
                {
                    isMoving = true;
                    anim.Play("jump");

                    froggieJump.pitch = Random.Range(0.75f, 1);
                    froggieJump.Play();

                    transform.SetParent(gameHandler.transform);
                    StartCoroutine(MoveFrog(nPos));
                }
            }

            // Move Right
            if ((Input.GetKeyDown(KeyCode. RightArrow) || Input.GetKeyDown(KeyCode.D)) && transform.localPosition.x < 4)
            {
                anim.transform.localEulerAngles = new Vector3(0, 0, -90);

                Vector2 nPos = new Vector2(transform.position.x + jumpDist, transform.position.y);

                if (transform.parent == gameHandler.transform) nPos = AlignPosition(nPos);

                if (gameHandler.ValidMovement(nPos))
                {
                    isMoving = true;
                    anim.Play("jump");

                    froggieJump.pitch = Random.Range(0.75f, 1);
                    froggieJump.Play();

                    transform.SetParent(gameHandler.transform);
                    StartCoroutine(MoveFrog(nPos));
                }
            }
        }
    }

    public void FrogHasDied()
    {
        froggieDeath.pitch = Random.Range(0.75f, 1);
        froggieDeath.Play();
        
        isDead = true;
    }

    public bool IsFrogDead()
    {
        return isDead;
    }
}//EndScript