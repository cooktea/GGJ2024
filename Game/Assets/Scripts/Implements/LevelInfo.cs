using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelInfo
{
    public float GameTime;
    public List<Vector2> AIPlayerPosition;
    public List<Vector2> HumanPlayerPosition;
    public int ScoreTarget;
    public int Level;
}
