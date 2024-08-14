using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivilianScript : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rb2D;
    [SerializeField] float movementSpeed = 1, distThreashold = 0.1f;
    [SerializeField] int boundX = 4, boundY = 4, searchRange = 2;
    GameObject PlayerObject;
    Vector2 movementDest = Vector2.zero;
    bool findFleeDest = false;

    void Start()
    {
        anim  = transform.GetComponent<Animator>();
        anim.Play("moving");

        rb2D = transform.GetComponent<Rigidbody2D>();

        if (GameObject.FindObjectOfType<LoneStarPlayerScript>() != null)
        {
            PlayerObject = GameObject.FindObjectOfType<LoneStarPlayerScript>().gameObject;
        }

        float x = Random.Range((float)(-boundX), (float)(boundX));
        float y = Random.Range((float)(-boundY), (float)(boundY));

        movementDest = new Vector2(x, y);
    }

    Vector2 GetFleeDir(Vector2 attackerPos)
    {
        findFleeDest = true;

        Invoke("FleeCoolDown", 3);

        Vector2 dir = ((Vector2) transform.localPosition - attackerPos).normalized;
        return dir;
    }

    void Update()
    {
        GoToPlayer();
        Movement();
        SafeyCircle();
    }

    void Movement()
    {
        if (Vector2.Distance(transform.localPosition, movementDest) > distThreashold)
        {
            rb2D.velocity = (movementDest - (Vector2)transform.localPosition).normalized * movementSpeed;
        } else
            {
                float x = Random.Range((float)(-boundX), (float)(boundX));
                float y = Random.Range((float)(-boundY), (float)(boundY));
                
                // Make sure the new destination isnt too close to current destination
                while (Vector2.Distance(new Vector2(x, y), movementDest) < 4)
                {
                    x = Random.Range((float)(-boundX), (float)(boundX));
                    y = Random.Range((float)(-boundY), (float)(boundY));
                }

                movementDest = new Vector2(x, y);
            }
    }

    void SafeyCircle()
    {
        GameObject[] activeAttackers = GameObject.FindGameObjectsWithTag("alien");

        if (activeAttackers.Length < 1) return;

        for (int i = 0; i < activeAttackers.Length; i++)
        {
            if (Vector2.Distance(transform.position, activeAttackers[i].transform.position) < 1.5f)
            {
                if (!findFleeDest) movementDest = GetFleeDir(activeAttackers[i].transform.position);
            }
        }
    }

    void FleeCoolDown()
    {
        findFleeDest = false;

        float x = Random.Range((float)(-boundX), (float)(boundX));
        float y = Random.Range((float)(-boundY), (float)(boundY));

        movementDest = new Vector2(x, y);
    }

    void GoToPlayer()
    {
        if (Vector2.Distance(transform.localPosition, PlayerObject.transform.localPosition) < searchRange)
        {
            movementDest = PlayerObject.transform.localPosition;
        }
    }

    // void OnDrawGizmos()
    // {
    //     // Gizmos.DrawWireSphere(transform.position, searchRange);

    //     Gizmos.color = Color.green;
    //     Gizmos.DrawSphere(movementDest, distThreashold);
    // }
}//EndScript