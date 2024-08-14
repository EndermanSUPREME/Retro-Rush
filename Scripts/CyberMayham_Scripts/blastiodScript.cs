using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blastiodScript : MonoBehaviour
{
    Vector3 destPts;
    CyberMayhamGameHandler cyberMayhamHandler;
    [SerializeField] [Range(0.075f, 0.125f)] float projectileSpeed = 0.1f;

    void Start()
    {
        cyberMayhamHandler = GameObject.FindObjectOfType<CyberMayhamGameHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (destPts != Vector3.zero)
        {
            ObjectMovement();
        }
    }

    void ObjectMovement()
    {
        transform.position = Vector2.MoveTowards(transform.position, destPts, projectileSpeed);

        if (Vector2.Distance(transform.position, destPts) < 0.001f)
        {
            // check if player can be damaged and remove object from scene
            cyberMayhamHandler.TryDamagePlayer(destPts);
            Destroy(gameObject);
        }
    }

    public void SlowDownBlastiod(float value)
    {
        // Allow control of blastiod speed to not create insane scenerios
        projectileSpeed *= value;
    }

    public void SetTargetDestination(Vector2 pts)
    {
        destPts = pts;
    }
}//EndScript