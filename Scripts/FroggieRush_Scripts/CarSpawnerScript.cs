using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawnerScript : MonoBehaviour
{
    [SerializeField] GameObject[] CarPrefs;
    [SerializeField] float CarSpeed = 1;

    void Start()
    {
        StartCoroutine(SpawnNewCar(Random.Range(1, 3)));
    }

    IEnumerator SpawnNewCar(float t)
    {
        yield return new WaitForSeconds(t);

        LoadNewCar();

        StartCoroutine(SpawnNewCar(Random.Range(4, 8)));
    }

    void LoadNewCar()
    {
        GameObject newCar = Instantiate(CarPrefs[Random.Range(0, CarPrefs.Length - 1)], transform.position, transform.rotation);
        
        if (newCar.GetComponent<Rigidbody2D>() != null)
        {
            float xDir = 1;

            // Right to Left
            if (transform.position.x > 0)
            {
                xDir *= -CarSpeed;
                newCar.transform.localEulerAngles = new Vector3(0,0,0);
            }

            // Left to Right
            if (transform.position.x < 0)
            {
                xDir *= -CarSpeed;
                newCar.transform.localEulerAngles = new Vector3(0,180,0);
            }
            
            newCar.GetComponent<Rigidbody2D>().velocity = new Vector2(xDir,0);
        }

        newCar.transform.SetParent(transform);
    }
}//EndScript