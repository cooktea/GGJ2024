using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ÓÎÏ·¿ØÖÆÆ÷
public interface IGameManager
{
    public void StartNewGame();
    public void EndGame();
    public int GetCurrentScore();
    public void BallIn(IPlayer.PlayerSide side);
    public int GetLastGameSecond();
    public void StartBulletTime();
    public void EndBulletTime();
    public void NextRound();
}