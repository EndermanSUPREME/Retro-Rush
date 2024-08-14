using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserShotScript : MonoBehaviour
{
    [SerializeField] bool HurtsPlayer = false;
    [SerializeField] float damageRadius = 0.25f;
    AudioSource bulletSound;

    void Start()
    {
        bulletSound = GameObject.Find("loneStarBullet").GetComponent<AudioSource>();

        bulletSound.pitch = Random.Range(0.75f, 1);
        bulletSound.Play();
    }

    void LateUpdate()
    {
        if (!HurtsPlayer)
        {
            SeekEnemies();
        } else
            {
                SeekPlayer();
            }

        // Destroy objects outside of screen
        if (Mathf.Abs(transform.position.x) > 7 || Mathf.Abs(transform.position.y) > 7) Destroy(gameObject);
    }

    void SeekEnemies()
    {
        GameObject[] renderedTargets = GameObject.FindGameObjectsWithTag("alien");

        if (renderedTargets.Length < 1) return;

        for (int i = 0; i < renderedTargets.Length; i++)
        {
            if (Vector2.Distance(transform.position, renderedTargets[i].transform.position) < damageRadius)
            {
                if (renderedTargets[i].GetComponent<AttackerScript>() != null)
                {
                    renderedTargets[i].GetComponent<AttackerScript>().TakeDamage(1);
                    Destroy(gameObject);
                    return;
                }
            }
        }
    }

    void SeekPlayer()
    {
        GameObject playerTarget = GameObject.FindObjectOfType<LoneStarPlayerScript>().gameObject;

        if (playerTarget == null) return;

        if (Vector2.Distance(transform.position, playerTarget.transform.position) < damageRadius)
        {
            playerTarget.GetComponent<LoneStarPlayerScript>().LoseLife();
            Destroy(gameObject);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,  damageRadius);
    }
}//EndScript