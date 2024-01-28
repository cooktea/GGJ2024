using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelInfo
{
    [Tooltip("单位：秒")]
    public float GameTime;
    [Tooltip("对方球员位置")]
    public List<Vector2> AIPlayerPosition;
    [Tooltip("对方球员状态")]
    public List<PlayerState> AIPlayerStates;
    [Tooltip("我方球员位置")]
    public List<Vector2> HumanPlayerPosition;
    [Tooltip("我方球员状态")]
    public List<PlayerState> HumanPlayerStates;
    [Tooltip("目标进球数")]
    public int ScoreTarget;
    [Tooltip("关卡序号")]
    public int Level;
}
