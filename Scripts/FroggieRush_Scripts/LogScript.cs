using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting;
using UnityEngine;

public class LogScript : MonoBehaviour
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