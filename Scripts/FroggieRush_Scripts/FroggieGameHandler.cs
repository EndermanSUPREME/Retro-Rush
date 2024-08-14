using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FroggieGameHandler : MonoBehaviour
{
    [SerializeField] GameObject FroggiePref, currentFrog, currentChunk;
    [SerializeField] GameObject[] Chunks, Collectables;
    List<GameObject> loadedCollectables = new List<GameObject>();
    List<Vector2> WaterPoints = new List<Vector2>(), ObstaclePoints = new List<Vector2>();
    int WaterPointSize = 0, ObstaclePointSize = 0;
    [SerializeField] int xBound = 4, yBound = 4;

    AudioSource froggieEat;
    
    RetroRushHandler MainGameHandler;

    void Awake()
    {
        FindWaterPoints();
    }

    void Start()
    {
        // Application.targetFrameRate = 65;
        MainGameHandler = GameObject.FindObjectOfType<RetroRushHandler>();
        froggieEat = GameObject.Find("froggieEat").GetComponent<AudioSource>();

        Invoke("LoadNewCollectables", 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentFrog == null)
        {
            currentFrog = GameObject.FindObjectOfType<FrogMovement>().gameObject;
        } else
            {
                if (currentFrog.GetComponent<FrogMovement>().IsFrogDead() || Mathf.Abs(currentFrog.transform.localPosition.x) >= 4.55f)
                    RespawnFrog();
            }
    }

    void LateUpdate()
    {
        if (currentFrog != null)
        {
            CheckForCarCollision(currentFrog.transform.position);
            CanFrogCollect();
        }
    }

    void FindWaterPoints()
    {
        // clean the slate
        WaterPoints.Clear();
        ObstaclePoints.Clear();

        WaterPointSize = 0;
        ObstaclePointSize = 0;

        // Gather new points and track sizes
        GameObject[] WaterTiles = GameObject.FindGameObjectsWithTag("waterTile");

        WaterPointSize = WaterTiles.Length;

        if (WaterTiles.Length > 0)
        {
            for (int i = 0; i < WaterTiles.Length; i++)
            {
                WaterPoints.Add(WaterTiles[i].transform.position);
            }
        }
    }

    void CheckForCarCollision(Vector2 pos)
    {
        GameObject[] LoadedCars = GameObject.FindGameObjectsWithTag("car");

        if (LoadedCars.Length > 0)
        {
            for (int i = 0; i < LoadedCars.Length; i++)
            {
                // if the frog is within bounds we can conclude theyve been hit by the car
                // or jumped on the car

                float dist = Vector2.Distance(pos, LoadedCars[i].transform.position);
                if (dist <= 0.75f)
                {
                    // break loop and respawn frog
                    RespawnFrog();
                    break;
                }
            }
        }
    }

    void LoadNewCollectables()
    {
        GameObject[] UncollectedCollectables = GameObject.FindGameObjectsWithTag("FroggieCollectable");
        if (UncollectedCollectables.Length > 0)
        {
            for (int i = 0; i < UncollectedCollectables.Length; i++)
            {
                Destroy(UncollectedCollectables[i]);
            }
        }
        UncollectedCollectables = null;

        loadedCollectables.Clear();

        List<Vector2> collectablePoints = new List<Vector2>();

        int numOfCollectables = Random.Range(3, 8);

        // Generate 2d vectors
        for (int i = 0; i < numOfCollectables; i++)
        {
            float x = Random.Range(-xBound, xBound);
            x = ((int) x) + 0.5f;

            if (x > xBound)
            {
                x = xBound - 0.5f;
            }

            if (x < -xBound)
            {
                x = xBound + 0.5f;
            }

            float y = Random.Range(-yBound, yBound);
            y = ((int) y) + 0.5f;

            if (y > yBound)
            {
                y = yBound - 0.5f;
            }

            if (y < -yBound)
            {
                y = yBound + 0.5f;
            }

            Vector2 rPos = new Vector2(x, y);

            // Check for dup. vectors and valid vectors
            if (!collectablePoints.Contains(rPos) && !ObstaclePoints.Contains(rPos))
                collectablePoints.Add(rPos);
            else
                i--;
        }

        // Load the Objects in from the generated positions
        for (int i = 0; i < collectablePoints.ToArray().Length; i++)
        {
            GameObject selectedPref = Collectables[Random.Range(0, Collectables.Length - 1)];
            Vector2 spawnPos = collectablePoints.ToArray()[i];

            GameObject newCollectable = Instantiate(selectedPref, spawnPos, transform.rotation);

            newCollectable.transform.name = newCollectable.transform.name.Replace("(Clone)", "");

            loadedCollectables.Add(newCollectable);
            newCollectable.transform.SetParent(transform);
        }
    }

    public void AddObstaclePoint(Vector2 pos)
    {
        ObstaclePoints.Add(pos);
        ObstaclePointSize++;
    }

    public bool ValidMovement(Vector2 nPos)
    {
        // if no obstacle tiles exists we can move anywhere we want in our bounds
        if (ObstaclePointSize == 0)
        {
            return true;
        }

        // if frog is jumping from log
        if (currentFrog.transform.parent != transform)
        {
            for (int i = 0; i < ObstaclePointSize; i++)
            {
                if (Vector2.Distance(nPos, ObstaclePoints.ToArray()[i]) < 0.75f) return false;
            }

            return true;
        }

        return !ObstaclePoints.Contains(nPos);
    }

    void CheckFrogPosition()
    {
        // Gerneal check position for changes in state
        if (currentFrog.transform.position.y > 5) NextChunk();
    }

    void CanFrogCollect()
    {
        // check if frog has collected a collectable
        for (int i = 0; i < loadedCollectables.ToArray().Length; i++)
        {
            if (Vector2.Distance(currentFrog.transform.position, loadedCollectables.ToArray()[i].transform.position) <= 0.01f)
            {
                // Collected Something
                string objName = loadedCollectables.ToArray()[i].transform.name;

                switch (objName.ToLower())
                {
                    case "fly":
                        MainGameHandler.UpdateScore(100);
                    break;
                    case "cherry":
                        MainGameHandler.UpdateScore(125);
                    break;
                    case "watermelon":
                        MainGameHandler.UpdateScore(175);
                    break;
                    case "apple":
                        MainGameHandler.UpdateScore(215);
                    break;

                    default:
                    break;
                }

                froggieEat.pitch = Random.Range(0.75f, 1);
                froggieEat.Play();

                // dispose object
                Destroy(loadedCollectables.ToArray()[i]);
                loadedCollectables.RemoveAt(i);
            }
        }
    }

    public bool SafeLanding(Vector2 nPos)
    {
        // CheckForCarCollision(nPos);

        CheckFrogPosition();

        // If the frog landed on a log the player landed safely
        GameObject[] activeLogs = GameObject.FindGameObjectsWithTag("log");
        for (int i = 0; i < activeLogs.Length; i++)
        {
            if (Vector2.Distance(nPos, activeLogs[i].transform.position) <= 0.45f)
            {
                // Set frog as child to log so the log carries the player
                currentFrog.transform.SetParent(activeLogs[i].transform);
                return true;
            }
        }

        // if no water tiles exists we always land safely
        if (WaterPointSize == 0)
        {
            return true;
        }

        // When WaterPoints contains nPos we will return false to indicate bad landing
        return !WaterPoints.Contains(currentFrog.GetComponent<FrogMovement>().AlignPosition(nPos));
    }

    public void NextChunk()
    {
        StartCoroutine(LoadChunk());
    }

    IEnumerator LoadChunk()
    {
        currentFrog.SetActive(false);
        currentFrog.transform.localPosition = new Vector3 (currentFrog.transform.localPosition.x, -4.5f, 0);

        yield return new WaitForSeconds(0.25f);
        // remove the old chunk and replace it
        currentChunk.SetActive(false);
        Destroy(currentChunk);
        WaterPoints.Clear();
        ObstaclePoints.Clear();

        WaterPointSize = 0;
        ObstaclePointSize = 0;

        currentChunk = Instantiate(Chunks[Random.Range(0, Chunks.Length - 1)]);
        currentChunk.transform.name = "FroggieRush_Chunk";
        currentChunk.transform.SetParent(transform);
        currentChunk.SetActive(true);
        
        FindWaterPoints();
        Invoke("LoadNewCollectables", 0.5f);

        yield return new WaitForSeconds(0.25f);
        currentFrog.SetActive(true);

        MainGameHandler.RevealGameExternal();
    }

    void RespawnFrog()
    {
        // remove frog and spawn in a new one
        currentFrog.GetComponent<FrogMovement>().FrogHasDied();
        Destroy(currentFrog);

        MainGameHandler.LoseLife();

        currentFrog = Instantiate(FroggiePref);

        currentChunk.transform.name = "Froggie";
        currentFrog.transform.SetParent(transform);
    }

    public void ResetFroggie()
    {
        if (currentFrog != null)
        {
            if (currentFrog.GetComponent<FrogMovement>() != null) currentFrog.GetComponent<FrogMovement>().ResetFroggie();
        }
    }
}//EndScript