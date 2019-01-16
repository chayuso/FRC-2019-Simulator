using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimeLightData : MonoBehaviour
{
    public double tvValidTarget = 0.0;
    public double txOffsetX = 0.0;
    public double tyOffsetY = 0.0;
    public double taTargetArea = 0.0;
    public double tsSkewRotation = 0.0;
    public double tlLatency = 11.0;

    public void resetValues()
    {
        tvValidTarget = 0.0;
        txOffsetX = 0.0;
        tyOffsetY = 0.0;
        taTargetArea = 0.0;
        tsSkewRotation = 0.0;
        tlLatency = 11.0;
    }
}
