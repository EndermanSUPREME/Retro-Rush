using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AV : MonoBehaviour
{
    [SerializeField] Vector2[] stations = new Vector2[0];
    [SerializeField] CyberMayhamGameHandler cyberMayhamHandler;
    [SerializeField] GameObject projectilePref;
    [SerializeField] float laserShotDelay = 1, speedResistance;
    bool fireLasers = true, inactive = true;
    AudioSource avShot;

    void Start()
    {
        avShot = GameObject.Find("cyberAVShot").GetComponent<AudioSource>();
    }

    public void ShutDownAV()
    {
        blastiodScript[] avBullets = GameObject.FindObjectsOfType<blastiodScript>();
        for (int i = 0; i < avBullets.Length; ++i)
        {
            Destroy(avBullets[i].gameObject);
        }
    }

    public void ReloadCyberAV()
    {
        StopCoroutine(FireWallCheck(stations));
        ShutDownAV();
        ScanForStations();
    }

    public void ScanForStations()
    {
        if (cyberMayhamHandler != null)
        {
            inactive = false;
            stations = cyberMayhamHandler.GetStationPoints();

            Invoke("ReadyTheAV", 2);
        }
    }

    void ReadyTheAV()
    {
        fireLasers = true;
    }

    public void DisableForTransition()
    {
        StopCoroutine(FireWallCheck(stations));
        inactive = false;
    }

    void CheckScoreThreashold()
    {
        if (!cyberMayhamHandler.canTransition())
        {
            fireLasers = true;
        } else
            {
                cyberMayhamHandler.StationTransfer();
            }
    }

    void FixedUpdate()
    {
        // Pick a random set of station points and shoot
        // sets of objects towards them

        if (stations.Length > 0 && !inactive)
        {
            if (fireLasers)
            {
                int markedTars = Random.Range(2, stations.Length - 2);
        
                List<Vector2> tPts = new List<Vector2>();
        
                for (int i = 0; i < markedTars;)
                {
                    int k = Random.Range(0, stations.Length);

                    if (!tPts.Contains(stations[k]))
                    {
                        tPts.Add(stations[k]);
                        ++i;
                    }
                }
        
                StartCoroutine(FireWallCheck(tPts.ToArray()));
            }
        }
    }

    IEnumerator FireWallCheck(Vector2[] pts)
    {
        fireLasers = false;

        for (int i = 0; i < pts.Length; ++i)
        {
            GameObject nProj = Instantiate(projectilePref, transform.position, transform.rotation);
            nProj.GetComponent<blastiodScript>().SetTargetDestination(pts[i]);
            nProj.GetComponent<blastiodScript>().SlowDownBlastiod(speedResistance);

            avShot.pitch = Random.Range(0.75f, 1);
            avShot.Play();
            
            yield return new WaitForSeconds(Random.Range(0.25f, 0.5f));
        }

        yield return new WaitForSeconds(laserShotDelay);
        
        CheckScoreThreashold();
    }

    public void OnDrawGizmos()
    {
        if (cyberMayhamHandler != null) stations = cyberMayhamHandler.GetStationPoints();

        Gizmos.color = Color.red;

        for (int i = 0; i < stations.Length; ++i)
        {
            Gizmos.DrawSphere(stations[i], 0.35f);
        }
    }

}//EndScript