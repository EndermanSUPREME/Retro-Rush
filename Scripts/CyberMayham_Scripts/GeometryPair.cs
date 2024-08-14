using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeometryPair : MonoBehaviour
{
    [SerializeField] LineRenderer DepthLineOne, DepthLineTwo;
    [SerializeField] Gradient playerGradient, defaultGradient;
    
    public void ColorChange(int m)
    {
        if (m == 0)
        {
            DepthLineOne.colorGradient = playerGradient;
            DepthLineTwo.colorGradient = playerGradient;
        } else
            {
                DepthLineOne.colorGradient = defaultGradient;
                DepthLineTwo.colorGradient = defaultGradient;
            }
    }
}//EndScript