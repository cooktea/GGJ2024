using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

// 鼠标控制器
public interface IMouseController
{
    public void OnEnterPlayer(GameObject player);
    public void OnExitPlayer();
    public void OnLeftButtonDown();
    public void OnLeftButtonHold();
    public void OnLeftButtonRelease();
    //public void OnMouseMove(Vector2 oldPosition, Vector2 newPosition);
}