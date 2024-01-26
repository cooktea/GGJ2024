using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

// 球相关
public interface IBall
{
    public GameObject GetOwner();
    public void Shoot();
    public void SetPath(List<Vector2> pathPoints);
    public float SetInitSpeed (float speed);
    public void DoMove(float deltaTime);
    public bool CheckIsScore();
    public bool CheckOutLine();
    public bool CheckCatched();
}
