using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogSpawnScript : MonoBehaviour
{
    [SerializeField] GameObject FloatingLogPref;
    [SerializeField] float LogSpeed = 1;

    void Start()
    {
        StartCoroutine(SpawnNewLog(Random.Range(1, 3)));
    }

    IEnumerator SpawnNewLog(float t)
    {
        yield return new WaitForSeconds(t);

        LoadNewLog();

        StartCoroutine(SpawnNewLog(Random.Range(4, 6)));
    }

    void LoadNewLog()
    {
        GameObject newFloatingLog = Instantiate(FloatingLogPref, transform.position, transform.rotation);
        
        if (newFloatingLog.GetComponent<Rigidbody2D>() != null)
        {
            float xDir = 1;
            if (transform.position.x > 0)
            {
                xDir *= -LogSpeed;
            }

            if (transform.position.x < 0)
            {
                xDir *= LogSpeed;
            }
            
            newFloatingLog.GetComponent<Rigidbody2D>().velocity = new Vector2(xDir,0);
        }

        newFloatingLog.transform.SetParent(transform);
    }
}//EndScript