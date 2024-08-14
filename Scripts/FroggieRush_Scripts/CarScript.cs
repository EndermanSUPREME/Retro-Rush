using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarScript : MonoBehaviour
{

    void LateUpdate()
    {
        // Destory logs when they float off-screen
        if (Mathf.Abs(transform.position.x) >= 9)
        {
            Destroy(gameObject);
        }
    }
}//EndScript