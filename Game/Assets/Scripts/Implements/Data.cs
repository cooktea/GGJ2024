using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Data
{
    public static readonly float GateToMiddle = 7.0f;
    public static readonly float UpperBound = 3.7f;
    public static readonly float Lowerbound = -3.7f;

    public static Vector3 ClampDir(Transform transform, Vector3 dir)
    {
        if (transform.position.y >= UpperBound)
        {
            dir.y = -Mathf.Abs(dir.y);
        }
        else if (transform.position.y <= Lowerbound)
        {
            dir.y = Mathf.Abs(dir.y);
        }

        if (transform.position.x >= GateToMiddle)
        {
            dir.x = -Mathf.Abs(dir.x);
        }
        else if (transform.position.x <= -GateToMiddle)
        {
            dir.x = Mathf.Abs(dir.x);
        }

        return dir;
    }
}
