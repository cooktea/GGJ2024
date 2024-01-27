using BehaviorDesigner.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

// 场上的球员
public interface IPlayer
{
    public enum PlayerType
    {
        Player,
        GoalKeeper,
    }

    public enum PlayerSide
    {
        Human,
        AI,
    }

    bool HoldBall { get; }

    bool OpponentHoldBall { get; }

    public PlayerSide Side { get; }

    public IBall BallRef { get; }
    void TryRefreshActionTree();
    void TryDoActionTree();
    public void OnCatchBall(GameObject ball);
    public void OnCatchEnemy(GameObject ball);

    public void Shoot();
}
