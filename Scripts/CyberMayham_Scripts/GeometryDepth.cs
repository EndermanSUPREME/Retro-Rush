using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeometryDepth : MonoBehaviour
{
    [SerializeField] GeometryStations AffordanceStation, StationOutter, StationInner;

    void Start()
    {
        DrawStationDepth();
    }

    public Transform[] GetGeometryPoints()
    {
        return AffordanceStation.GetGeometryPoints();
    }

    public Transform[] GetGeometryDepthPoints()
    {
        return StationOutter.GetGeometryPoints();
    }

    void DrawStationDepth()
    {
        Transform[] sPts = StationOutter.GetGeometryPoints();
        Transform[] ePts = StationInner.GetGeometryPoints();

        for (int i = 0; i < sPts.Length; ++i)
        {
            if (sPts[i].GetComponent<LineRenderer>() != null)
            {
                Vector3[] pts = {sPts[i].position, ePts[i].position};

                // Debug.DrawLine(sPts[i].position, ePts[i].position, Color.red);

                sPts[i].GetComponent<LineRenderer>().SetPositions(pts);
            }
        }
    }

    void OnDrawGizmos()
    {
        DrawStationDepth();
    }
}//EndScript