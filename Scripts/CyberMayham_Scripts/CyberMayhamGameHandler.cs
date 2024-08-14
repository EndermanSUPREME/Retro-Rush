using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyberMayhamGameHandler : MonoBehaviour
{
    [SerializeField] Transform playerObject, playerConnectOne, playerConnectTwo;
    [SerializeField] GeometryDepth[] stations;
    [SerializeField] Gradient playerGradient, defaultGradient;
    [SerializeField] Transform[] playerMeshPoints;
    [SerializeField] Animator cyberAnim;
    [SerializeField] [Range(0, 0.5f)] float movementSpeed = 0.025f;

    RetroRushHandler MainHandler;
    AV AntiVirus;
    Transform[] GeometryPoints;
    int geoIndex = 0, selectedStationIndex = -1, tempScore = 0;
    Vector2 playerDestPoint, connectionOne, connectionTwo;
    bool pauseMovement = false;

    AudioSource playerHit, playerMove;

    // Start is called before the first frame update
    void Start()
    {
        // Application.targetFrameRate = 65;

        MainHandler = GameObject.FindObjectOfType<RetroRushHandler>();
        // AntiVirus = GameObject.FindObjectOfType<AV>();

        playerHit = GameObject.Find("cyberDamage").GetComponent<AudioSource>();
        playerMove = GameObject.Find("cyberPlayerMovement").GetComponent<AudioSource>();

        ReturnToCyber();
    }

    public AV GetAntiVirus()
    {
        return AntiVirus;
    }

    void FixedUpdate()
    {
        if (!pauseMovement) ShiftPosition();
    }

    void LateUpdate()
    {
        // snap player to the destination when they are within range
        if (Vector2.Distance(playerObject.position, playerDestPoint) < 0.1)
        {
            playerObject.position = playerDestPoint;
            ColorAffordance();
        } else
            {
                playerObject.position = Vector2.MoveTowards(playerObject.position, playerDestPoint, 1);
            }
    }

    public void StationTransfer()
    {
        // Camera Transitioning
        pauseMovement = true;

        AntiVirus.DisableForTransition();
        cyberAnim.Play("spin");
        SetTempScore(MainHandler.GetScore());

        Invoke("ChangeStation", 1.9f);
    }

    void ChangeStation()
    {
        // Run this code after the animation is over
        stations[selectedStationIndex].gameObject.SetActive(false);
        ReturnToCyber();
        Invoke("AllowMovement", 0.5f);
    }

    public void ReturnToCyber()
    {
        int tempInt = 0;
        // Try to ensure the tempInt is randomized and minimize duplication
        for (int i = 0; i < 5; ++i) tempInt = Random.Range(0, stations.Length);

        selectedStationIndex = tempInt;

        stations[selectedStationIndex].gameObject.SetActive(true);

        GeometryPoints = stations[selectedStationIndex].GetGeometryPoints();

        if (stations[selectedStationIndex].GetComponent<AV>() != null)
        {
            AntiVirus = stations[selectedStationIndex].GetComponent<AV>();
            stations[selectedStationIndex].GetComponent<AV>().ScanForStations();
        }

        geoIndex = 0;
        playerObject.position = GetNewDestination(1);
    }

    // When the player score hits a certian quota we change the station when possible
    public bool canTransition()
    {
        if (MainHandler.GetScore() - tempScore >= 500) return true;
        return false;
    }

    public void SetTempScore(int value)
    {
        tempScore = value;
    }

//=======================================================================================================
//=======================================================================================================
//========================================= PLAYER CODE =================================================
//=======================================================================================================
//=======================================================================================================

    public void ReadyCyberMayhamState()
    {
        AllowMovement();
    }

    void AllowMovement()
    {
        pauseMovement = false;
    }

    void ShiftPosition()
    {
        // Shifting to the Left
        if (Input.GetKey(KeyCode. A) || Input.GetKey(KeyCode. LeftArrow))
        {
            playerMove.pitch = Random.Range(0.75f, 1);
            playerMove.Play();

            if ((Vector2)playerObject.position == playerDestPoint) playerObject.position = GetNewDestination(-1);
        }

        // Shifting to the Right
        if (Input.GetKey(KeyCode. D) || Input.GetKey(KeyCode. RightArrow))
        {
            playerMove.pitch = Random.Range(0.75f, 1);
            playerMove.Play();
            
            if ((Vector2)playerObject.position == playerDestPoint) playerObject.position = GetNewDestination(1);
        }
    }

    void ColorAffordance()
    {
        DrawPlayerMesh();

        // Set all Lines to default color
        for (int i = 0; i < GeometryPoints.Length; ++i)
        {
            GeometryPoints[i].GetComponent<GeometryPair>().ColorChange(1);
        }

        // Set the color affordance to the players area
        for (int i = 0; i < GeometryPoints.Length; ++i)
        {
            int i2 = i+1;

            if (i2 >= GeometryPoints.Length) i2 = 0;
            if (i2 < 0) i2 = GeometryPoints.Length - 1;

            float midPointX = (GeometryPoints[i].position.x + GeometryPoints[i2].position.x) / 2;
            float midPointY = (GeometryPoints[i].position.y + GeometryPoints[i2].position.y) / 2;

            Vector2 testPos = new Vector2(midPointX, midPointY);

            if (testPos == playerDestPoint)
            {
                GeometryPoints[i].transform.GetComponent<LineRenderer>().colorGradient = playerGradient;
                GeometryPoints[i].GetComponent<GeometryPair>().ColorChange(0);
            } else
                {
                    GeometryPoints[i].transform.GetComponent<LineRenderer>().colorGradient = defaultGradient;
                }
        }
    }

    void DrawPlayerMesh()
    {
        playerConnectOne.position = connectionOne;
        playerConnectTwo.position = connectionTwo;

        for (int i = 1; i < playerMeshPoints.Length - 1; ++i)
        {
            Vector3 fwd = (transform.position - playerObject.position).normalized;
            Vector3 dir = (playerMeshPoints[i].position - playerObject.position).normalized;

            if (Vector3.Angle(fwd, dir) < 85)
            {
                playerMeshPoints[i].position -= fwd * 1.25f;
            }
        }

        if (playerObject.GetComponent<LineRenderer>() != null)
        {
            playerObject.GetComponent<LineRenderer>().positionCount = playerMeshPoints.Length;

            for (int i = 0; i < playerMeshPoints.Length; ++i) playerObject.GetComponent<LineRenderer>().SetPosition(i, playerMeshPoints[i].position);
        }
    }

    Vector2 GetNewDestination(int dir)
    {
        // to calculate the new destination we take the dir value into account
        int tIndex = geoIndex + dir;

        if (tIndex >= GeometryPoints.Length) tIndex = 0;
        if (tIndex < 0) tIndex = GeometryPoints.Length - 1;

        float midPointX = (GeometryPoints[tIndex].position.x + GeometryPoints[geoIndex].position.x) / 2;
        float midPointY = (GeometryPoints[tIndex].position.y + GeometryPoints[geoIndex].position.y) / 2;

        Vector2 nDest = new Vector2(midPointX, midPointY);

        connectionOne = GeometryPoints[geoIndex].position;
        connectionTwo = GeometryPoints[tIndex].position;

        playerDestPoint = nDest;

        geoIndex = tIndex;

        pauseMovement = true;
        Invoke("AllowMovement", movementSpeed);

        return nDest;
    }

    public void TryDamagePlayer(Vector2 pts)
    {
        if (Vector2.Distance(playerDestPoint, pts) <= 0.0001f)
        {
            // Deal Damage to Player

            playerHit.pitch = Random.Range(0.75f, 1);
            playerHit.Play();
            
            MainHandler.LoseLife();
        } else
            {
                MainHandler.UpdateScore(25);
            }
    }

    public Vector2[] GetStationPoints()
    {
        List<Vector2> pts = new List<Vector2>();
        
        for (int i = 0; i < GeometryPoints.Length; ++i)
        {
            float midPointX = 0;
            float midPointY = 0;

            if (i+1 >= GeometryPoints.Length)
            {
                midPointX = (GeometryPoints[0].position.x + GeometryPoints[i].position.x) / 2;
                midPointY = (GeometryPoints[0].position.y + GeometryPoints[i].position.y) / 2;
            }
            else
                {
                    midPointX = (GeometryPoints[i+1].position.x + GeometryPoints[i].position.x) / 2;
                    midPointY = (GeometryPoints[i+1].position.y + GeometryPoints[i].position.y) / 2;
                }

            pts.Add(new Vector2(midPointX, midPointY));
        }

        return pts.ToArray();
    }

    void OnDrawGizmos()
    {
        for (int i = 0; i < stations.Length; ++i)
        {
            if (stations[i].gameObject.activeSelf) GeometryPoints = stations[i].GetGeometryPoints();
        }
    }

}//EndScript