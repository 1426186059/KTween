using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
public static class SimpleTweenFunc
{
    public static Vector3 easeLinear(Vector3 from, Vector3 to, float fTimePercent)
    {
        return from * (1 - fTimePercent) + to * fTimePercent;
    }

    public static Vector2 easeLinear(Vector2 from, Vector2 to, float fTimePercent)
    {
        return from * (1 - fTimePercent) + to * fTimePercent;
    }

    public static float easeLinear(float from, float to, float fTimePercent)
    {
        return from * (1 - fTimePercent) + to * fTimePercent;
    }
}
