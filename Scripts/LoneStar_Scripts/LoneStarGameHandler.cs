using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LoneStarGameHandler : MonoBehaviour
{
    RetroRushHandler MainGameHandler;
    int Level = 0, numberOfEnemiesLeft = 0, streak = 0,
        boundX = 4, boundY = 4, totalAttackers = 0, totalCivilians = 0;
    bool levelActive = true, backupAttackersReady = false;

    [SerializeField] GameObject AttackerPref, CivilianPref, PlayerObj, GameSpecializedUI;
    List<GameObject> SpawnedAttackers = new List<GameObject>(), SpawnedCivilians = new List<GameObject>();
    [SerializeField] Text EnemyCounterDisplay, CivilianCounterDisplay;

    // Start is called before the first frame update
    void Start()
    {
        // Application.targetFrameRate = 65;
        MainGameHandler = GameObject.FindObjectOfType<RetroRushHandler>();
    }

    void Update()
    {
        if (GameSpecializedUI.activeSelf) GameSpecializedUI.SetActive(true);

        if (backupAttackersReady)
        {
            AttackerScript[] renderedAttackers = GameObject.FindObjectsOfType<AttackerScript>();

            if (renderedAttackers.Length <= 4)
            {
                backupAttackersReady = false;
                LoadInBackUpAttackers();
            }
        }

        if (numberOfEnemiesLeft <= 0 && levelActive && !backupAttackersReady) ReadyNextLevel(1);
    }

    public void DisableGameUI()
    {
        GameSpecializedUI.SetActive(false);
    }

    public void ReadyNextLevel(int i)
    {
        StartCoroutine(RenderLevel(i));
    }

    IEnumerator RenderLevel(int i)
    {
        levelActive = false;

        Level += i;

        SpawnedAttackers.Clear();

        // Incase we do a game swap we want the player to be spawned back into a clean zone
        AttackerScript[] renderedAttackers = GameObject.FindObjectsOfType<AttackerScript>();

        if (renderedAttackers.Length > 0)
        {
            for (int k = 0; i < renderedAttackers.Length; i++)
            {
                Destroy(renderedAttackers[k]);
            }
        }

        // Despawn Old Civilians
        if (totalCivilians > 0)
        {
            for (int j = 0; j < totalCivilians; j++)
            {
                if (SpawnedCivilians[j] != null) Destroy(SpawnedCivilians[j]);
            }
        }

        SpawnedCivilians.Clear();

        backupAttackersReady = false;

        yield return new WaitForSeconds(0.25f);

        SpawnInCivilians();
        yield return new WaitForSeconds(0.1f);
        SpawnInAttackers();

        yield return new WaitForSeconds(0.25f);

        levelActive = true;
    }

    IEnumerator PrepareBackUp()
    {
        yield return new WaitForSeconds(1);
        backupAttackersReady = true;
    }

//==========================================================================
//=========================== SPAWNING =====================================
//==========================================================================

    void SpawnInAttackers()
    {
        // Load in New Attackers

        // [14 x log(level)] + 5
        // totalAttackers = (int)((14 * Mathf.Log(Level)) + 5);
        totalAttackers = 5;
        EnemyCounterDisplay.text = totalAttackers.ToString();
        numberOfEnemiesLeft = totalAttackers;

        // REP = 14
    	int REP = totalAttackers;

        // 14 > 8
        if (totalAttackers > 8)
        {
            // REP = 8
            REP = 8;
            
            StartCoroutine(PrepareBackUp());
        }

        // Spawn REP=8 enemies
        for (int i = 0; i < REP; i++)
        {
            // if (totalAttackers - i < 8)
            // {
                float x = Random.Range((float)(-boundX), (float)(boundX));
                float y = Random.Range((float)(-boundY), (float)(boundY));
    
                Vector2 spawnPoint = new Vector2(x, y);

                // Make sure the spawn point isnt right on top of the player
                while (Vector2.Distance(PlayerObj.transform.position, spawnPoint) <= 2)
                {
                    x = Random.Range((float)(-boundX), (float)(boundX));
                    y = Random.Range((float)(-boundY), (float)(boundY));
    
                    spawnPoint = new Vector2(x, y);
                }
    
                GameObject newChallenger = Instantiate(AttackerPref, spawnPoint, AttackerPref.transform.rotation);
                newChallenger.transform.SetParent(transform);
                SpawnedAttackers.Add(newChallenger);

            // }
        }
    }

    void LoadInBackUpAttackers()
    {
        int REP = numberOfEnemiesLeft;

        if (REP > 8)
        {
            REP = 8;
            StartCoroutine(PrepareBackUp());
        }

        for (int i = 0; i < REP; i++)
        {
            
            float x = Random.Range((float)(-boundX), (float)(boundX));
            float y = Random.Range((float)(-boundY), (float)(boundY));
    
            Vector2 spawnPoint = new Vector2(x, y);

            // Make sure the spawn point isnt right on top of the player
            while (Vector2.Distance(PlayerObj.transform.position, spawnPoint) <= 1)
            {
                x = Random.Range((float)(-boundX), (float)(boundX));
                y = Random.Range((float)(-boundY), (float)(boundY));
    
                spawnPoint = new Vector2(x, y);
            }
    
            GameObject newChallenger = Instantiate(AttackerPref, spawnPoint, AttackerPref.transform.rotation);
            newChallenger.transform.SetParent(transform);
            SpawnedAttackers.Add(newChallenger);
        }
    }

    void SpawnInCivilians()
    {
        totalCivilians = Random.Range(4, 8);

        CivilianCounterDisplay.text = totalCivilians.ToString();

        for (int i = 0; i < totalCivilians; i++)
        {
            float x = Random.Range((float)(-boundX), (float)(boundX));
            float y = Random.Range((float)(-boundY), (float)(boundY));

            Vector2 spawnPoint = new Vector2(x, y);

            GameObject newCivilian = Instantiate(CivilianPref, spawnPoint, CivilianPref.transform.rotation);
            newCivilian.transform.SetParent(transform);
            SpawnedCivilians.Add(newCivilian);
        }
    }

//==========================================================================
//============================ REMOVES =====================================
//==========================================================================

    public void AttackerDestroyed()
    {
        if (numberOfEnemiesLeft > 0) --numberOfEnemiesLeft;

        EnemyCounterDisplay.text = numberOfEnemiesLeft.ToString();

        MainGameHandler.UpdateScore(50);
    }

    public void CivilianRescued()
    {
        MainGameHandler.UpdateScore(70 + (12 * streak));
        if (streak < 5) ++streak;

        if (totalCivilians > 0) --totalCivilians;
        CivilianCounterDisplay.text = totalCivilians.ToString();
    }

    public void CivilianKilled(GameObject civilianObject)
    {
        Destroy(civilianObject);

        if (totalCivilians > 0) --totalCivilians;
        CivilianCounterDisplay.text = totalCivilians.ToString();
    }

    public void RemoveStreak()
    {
        streak = 0;
    }

    public void LoseLife()
    {
        MainGameHandler.LoseLife();
    }

    public void IframePlayer()
    {
        LoneStarPlayerScript playerScript = GameObject.FindObjectOfType<LoneStarPlayerScript>();
        if (playerScript != null) 
        {
            playerScript.IframePlayer();
        } else
            {
                Debug.LogError("LoneStar Player Script Not Found!");
            }
    }

    public void CleanUpArea()
    {
        // Remove bullets
        LaserShotScript[] bulletsActive = GameObject.FindObjectsOfType<LaserShotScript>();
        for (int i = 0; i < bulletsActive.Length; ++i)
        {
            Destroy(bulletsActive[i].gameObject);
        }

        // Remove all Civilians and Enemies
        for (int i = 0; i < SpawnedAttackers.ToArray().Length; ++i)
        {
            if (SpawnedAttackers.ToArray()[i] != null) Destroy(SpawnedAttackers.ToArray()[i].gameObject);
        }

        for (int i = 0; i < SpawnedCivilians.ToArray().Length; ++i)
        {
            if (SpawnedCivilians.ToArray()[i] != null) Destroy(SpawnedCivilians.ToArray()[i].gameObject);
        }

        RemoveStreak();

        totalAttackers = 0;
        totalCivilians = 0;

        SpawnedAttackers = new List<GameObject>();
        SpawnedCivilians = new List<GameObject>();

        // Reset player position
        PlayerObj.transform.position = new Vector3(0, 0, PlayerObj.transform.position.z);
    }
}//EndScript