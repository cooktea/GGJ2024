﻿using BehaviorDesigner.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Player : MonoBehaviour, IPlayer
{
    public SharedVariable<bool> HoldBall {  get; private set; }

    public bool OpponentHoldBall {  get; private set; }

    public IPlayer.PlayerSide Side => side;

    public IBall BallRef { get; private set; }

    [SerializeField] IPlayer.PlayerSide side;

    public void OnCatchBall(GameObject ball)
    {
        HoldBall.SetValue(true);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IBall ball = collision.GetComponent<IBall>();
        if (ball is not null)
        {
            Debug.Log("ball");
        }
    }

    public void Shoot()
    {
        HoldBall.SetValue(false);
    }
}