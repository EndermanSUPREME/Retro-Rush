using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AttackerScript : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rb2D;

    [SerializeField] float movementSpeed = 1, laserSpeed = 5, avoidanceRange = 0.5f, Speed = 0;
    [SerializeField] int boundX = 4, boundY = 4, Health = 1, searchRange = 3;
    [SerializeField] GameObject LaserShotPref;

    List<float> distLengths = new List<float>();

    GameObject PlayerObject, targetCivilian;
    Vector2 movementDest = Vector2.zero, avoidancePush = Vector2.zero;

    bool killHumans = false, shootingLaser = false;
    LoneStarGameHandler loneStarHandler;

    AudioSource attackerDamage;

    // Start is called before the first frame update
    void Start()
    {
        anim  = transform.GetComponent<Animator>();
        anim.Play("moving");

        Speed = movementSpeed;

        loneStarHandler = GameObject.FindObjectOfType<LoneStarGameHandler>();

        rb2D = transform.GetComponent<Rigidbody2D>();

        if (GameObject.FindObjectOfType<LoneStarPlayerScript>() != null)
        {
            PlayerObject = GameObject.FindObjectOfType<LoneStarPlayerScript>().gameObject;
        }

        TaskPrioity();

        float x = Random.Range((float)(-boundX), (float)(boundX));
        float y = Random.Range((float)(-boundY), (float)(boundY));

        movementDest = new Vector2(x, y);

        attackerDamage = GameObject.Find("loneStarEnemyDead").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Health > 0)
        {
            Movement();
    
            if (killHumans)
            {
                ChaseCivilians();
            } else
                {
                    movementDest = PlayerObject.transform.localPosition;
                    if (!shootingLaser) ShootAtPlayer();
                }
        } else
            {
                AttackDie();
            }
    }

    void TaskPrioity()
    {
        killHumans = (Random.Range(0,25) % 2 == 0);
    }

    void Movement()
    {
        AvoidanceMovement();

        if (Vector2.Distance(transform.localPosition, movementDest) > 0.1f)
        {
            rb2D.velocity = ((movementDest - (Vector2)transform.localPosition).normalized + avoidancePush) * Speed;
        } else
            {
                if (killHumans)
                {
                    // if an attacker kills a civilian while
                    // the player has some streak, revoke the streak
                    loneStarHandler.RemoveStreak();
                    loneStarHandler.CivilianKilled(targetCivilian);
                }

                float x = Random.Range((float)(-boundX), (float)(boundX));
                float y = Random.Range((float)(-boundY), (float)(boundY));
                movementDest = new Vector2(x, y);
            }
    }

    void ChaseCivilians()
    {
        if (Vector2.Distance(transform.localPosition, PlayerObject.transform.localPosition) < searchRange)
        {
            if (!shootingLaser) ShootAtPlayer();
        }

        GameObject[] allCivilians = GameObject.FindGameObjectsWithTag("civilian");
        if (allCivilians.Length == 0) return;

        for (int i = 0; i < allCivilians.Length; i++)
        {
            if (Vector2.Distance(transform.localPosition, allCivilians[i].transform.localPosition) < searchRange)
            {
                targetCivilian = allCivilians[i];
                movementDest = allCivilians[i].transform.localPosition;
            }
        }
    }

    void AvoidanceMovement()
    {
        AttackerScript[] renderedAttackers = GameObject.FindObjectsOfType<AttackerScript>();
        distLengths.Clear();

        // Gather up a list of distances
        if (renderedAttackers.Length > 0)
        {
            for (int i = 0; i < renderedAttackers.Length; i++)
            {
                // we dont want to track this.transform or we will have a minimum distance
                // of 0, meaning the AI will stand still upon spawning into frame
                if (renderedAttackers[i].transform != transform)
                {
                    float dist = Vector2.Distance(transform.position, renderedAttackers[i].transform.position);
                    distLengths.Add(dist);
                }
            }

            // focus on the smallest float value to handle speed change

            float shortestDist = 0;
            
            if (distLengths.ToArray().Length > 0)
            {
                shortestDist = distLengths.Min();
            } else
                {
                    shortestDist = 100;
                }

            // if the distance is smaller than the avoidanceRange
            // we will get a value less than 1, in this case
            // we want to handle the speed adjustment accordingly

            if ( (shortestDist / avoidanceRange) > 0.25f && (shortestDist / avoidanceRange) < 1 )
            {
                Speed = movementSpeed * shortestDist;
            } else
                {
                    // if the ratio is over the value 1 the AI can have max speed
                    if ((shortestDist / avoidanceRange) >= 1)
                    {
                        Speed = movementSpeed;
                        avoidancePush = Vector2.zero;
                    }

                    // if the ratio is below 1/4 the AI will have low speed
                    if ((shortestDist / avoidanceRange) <= 0.25f)
                    {
                        Speed = movementSpeed * 0.75f;

                        int selectedIndex = distLengths.IndexOf(shortestDist);
                        
                        AttackerScript closestBody = (AttackerScript) renderedAttackers.GetValue(selectedIndex);

                        if (transform.position.x > closestBody.transform.position.x)
                            avoidancePush = ( Vector2.Perpendicular(rb2D.velocity) ) * 0.1f;
                        else
                            avoidancePush = -( Vector2.Perpendicular(rb2D.velocity) ) * 0.1f;
                    }
                }
        } else
            {
                Speed = movementSpeed;
                avoidancePush = Vector2.zero;
            }
    }

    void ShootAtPlayer()
    {
        shootingLaser = true;

        Vector2 tarPos = PlayerObject.transform.position;
        GameObject laserShot = Instantiate(LaserShotPref, transform.position, transform.rotation);

        Vector2 shotDir = (tarPos - (Vector2)transform.position).normalized;
        laserShot.GetComponent<Rigidbody2D>().velocity = shotDir * laserSpeed;

        Invoke("LaserCoolDown", Random.Range(2, 4));
    }

    void LaserCoolDown()
    {
        shootingLaser = false;
    }

    public void TakeDamage(int amount)
    {
        Health -= amount;
    }

    void AttackDie()
    {
        loneStarHandler.AttackerDestroyed();

        attackerDamage.pitch = Random.Range(0.75f, 1);
        attackerDamage.Play();

        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, avoidanceRange);
    }

}//EndScript