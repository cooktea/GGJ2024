using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ÓÎÏ·¿ØÖÆÆ÷
public interface IGameManager
{
    public void Init();
    public void StartNewGame(int level);
    public void EndGame();
    public int GetCurrentScore();
    public void BallIn();
    public int GetLastGameSecond();
    public void StartBulletTime();
    public void NextRound();
}