using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleScript : MonoBehaviour
{
    [SerializeField] Sprite[] obstacleTileSprites;
    bool activeObstacle = false;

    void Start()
    {
        transform.tag = "Untagged";
        
        float x, y;
        x = ((int) transform.localPosition.x);

        if (x > 0 || transform.localPosition.x > 0)
        {
            x += 0.5f;
        } else if (x < 0 || transform.localPosition.x < 0)
            {
                x -= 0.5f;
            }

        y = transform.localPosition.y;

        transform.localPosition = new Vector3(x, y, 0);

        if (transform.childCount != 0)
        {
            transform.GetChild(0).gameObject.SetActive(false);

            if (Random.Range(0, 10) % 2 == 0)
            {
                // first child is the obstacle object
                // transform is just a script holder
                // transform.tag = "obstacle";
                activeObstacle = true;
                transform.GetChild(0).gameObject.SetActive(true);
                transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = obstacleTileSprites[Random.Range(0, obstacleTileSprites.Length - 1)];
            
                // Push valid vectors to the game handler list
                FroggieGameHandler gameHandler = Object.FindObjectOfType<FroggieGameHandler>();
                if (gameHandler != null)
                {
                    gameHandler.AddObstaclePoint(transform.localPosition);
                }
            }
        }
    }

    public bool isActiveObstacle()
    {
        return activeObstacle;
    }
}//EndScript