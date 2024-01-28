using BehaviorDesigner.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Player : MonoBehaviour, IPlayer
{
    public bool HoldBall {  get; private set; }
    public bool OpponentHoldBall {  get; private set; }

    public IPlayer.PlayerSide Side => side;

    public IBall BallRef { get; private set; }

    [SerializeField] IPlayer.PlayerSide side;

    public void OnCatchBall(GameObject ball)
    {
        HoldBall = (true);
        BallRef = ball.GetComponent<IBall>();
    }

    public void OnCatchEnemy(GameObject ball)
    {
    }

    public void TryDoActionTree()
    {
    }

    public void TryRefreshActionTree()
    {
    }

    public void Shoot()
    {
        HoldBall = (false);
    }
}
