using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

// 行为树节点
public interface IActionNode
{
    IPlayer player { get; }
    public void Update();

    void UpdateCore();
}
