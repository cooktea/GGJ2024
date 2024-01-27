using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamMate : MonoBehaviour, ITeammate
{
    public SharedVariable<bool> HoldBall { get; private set; }

    public bool OpponentHoldBall { get; }

    public IPlayer.PlayerSide Side => IPlayer.PlayerSide.Human;

    public IBall BallRef => throw new System.NotImplementedException();

    public void OnCatchBall(GameObject ball)
    {
    }

    public void OnCatchEnemy(GameObject ball)
    {
        throw new System.NotImplementedException();
    }

    public void OnShootBall()
    {
        throw new System.NotImplementedException();
    }

    public void Shoot()
    {
        throw new System.NotImplementedException();
    }

    public void TryDoActionTree()
    {
        throw new System.NotImplementedException();
    }

    public void TryRefreshActionTree()
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
