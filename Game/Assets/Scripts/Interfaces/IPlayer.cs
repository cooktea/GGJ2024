using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 场上的球员
public interface IPlayer
{
    public enum PlayerType
    {
        TeamMate = 0x01,
        Enemy = 0x01 << 1,
        Holder = 0x01 << 2,     // 持球的人
        GoalKeeper = 0x01 << 3, // 守门员
    }
    void TryRefreshActionTree();
    void TryDoActionTree();
    public void OnCatchBall();
}
