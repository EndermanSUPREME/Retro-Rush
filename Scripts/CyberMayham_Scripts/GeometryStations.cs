using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeometryStations : MonoBehaviour
{
    [SerializeField] Transform[] GeometryPoints;
    [SerializeField] bool isDepth = false;

    void Start()
    {
        DrawStationShape();
    }

    void DrawStationShape()
    {
        if (!isDepth)
        {
            for (int i = 0; i < GeometryPoints.Length; ++i)
            {
                if (GeometryPoints[i].GetComponent<LineRenderer>() != null)
                {
                    Vector3 endPoint;
                    if (i+1 >= GeometryPoints.Length)
                    {
                        endPoint = GeometryPoints[0].position;
                    } else
                        {
                            endPoint = GeometryPoints[i+1].position;
                        }
    
                    Vector3[] pts = {GeometryPoints[i].position, endPoint};
    
                    GeometryPoints[i].GetComponent<LineRenderer>().SetPositions(pts);
                }
            }
        }
    }

    public Transform[] GetGeometryPoints()
    {
        return GeometryPoints;
    }

    void OnDrawGizmos()
    {
        DrawStationShape();
    }
}//EndScript