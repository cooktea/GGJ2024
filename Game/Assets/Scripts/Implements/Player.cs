using System.Collections;
using System.Linq;
using UnityEngine;

public enum BTState
{
	AttackingWithBall,
	TryGetBallFromGround,
	TryGetBallFromEnemy,
	Defencing,
	Roaming,
	Enter,
}

public class Player : MonoBehaviour, IPlayer
{
    float GateToMiddle = 7.4f;
    float UpperBound = 3.9f;
    float Lowerbound = -3.9f;

    [Header("AI")]
    [SerializeField] BTState btState = BTState.Enter;
    [SerializeField] float Speed = 30;
    [SerializeField] float PlayerDistanceThreshold;
    [SerializeField] float GateDistanceThreshold;

    public bool HoldBall { get; private set; }
    public bool OpponentHoldBall { get; private set; }

    public IPlayer.PlayerSide Side => side;

    public IBall BallRef { get; private set; }
    public GameManager GM;
    Gate gate;
    Gate selfGate;


    [SerializeField] IPlayer.PlayerSide side;
    Coroutine coroutine;
    Vector2 Forward => new Vector2(gate.transform.position.x - transform.position.x, 0).normalized;
    Vector2 Backward => -Forward;

    Player[] Enemies;
    Player[] Teamates;

    private void Start()
    {
        GetEnemyList();
        GetTeamateList();

        switch (side)
        {
            case IPlayer.PlayerSide.Human:
                gate = GM.GateAI.GetComponent<Gate>();
                selfGate = GM.GateHuman.GetComponent<Gate>();
                break;
            default:
                gate = GM.GateHuman.GetComponent<Gate>();
                selfGate = GM.GateAI.GetComponent<Gate>();
                break;
        }
    }

    public void OnCatchBall(GameObject ball)
    {
        HoldBall = true;
        BallRef = ball.GetComponent<IBall>();
        btState = BTState.Enter;
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
        HoldBall = false;
        btState = BTState.Enter;
    }

    private void Update()
    {
        // When OnCatchBall() and Shoot() will re-enter BT
        if (btState == BTState.Enter)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }

            if (HoldBall)
            {
                BeginAttackWithBall();
                return;
            }


            if (GM.Ball.GetComponent<IBall>().GetOwner() == null)
            {
                StartMoveToBall();
                return;
            }

            if (GM.BallSide() != side)
            {
                StartDefence();
            }
        }
    }

    void StartMoveToBall()
    {
        btState = BTState.TryGetBallFromGround;
        StartCoroutine(MoveToBall());
    }

    private IEnumerator MoveToBall()
    {
        while (GM.Ball.GetComponent<IBall>().GetOwner() == null)
        {
            var dir = GM.Ball.transform.position - transform.position;
            Move(dir.normalized);
            yield return null;
        }
        btState = BTState.Enter;
    }

    void BeginAttackWithBall()
    {
        btState = BTState.AttackingWithBall;
        coroutine = StartCoroutine(AttackWithBall());
    }

    IEnumerator AttackWithBall()
    {
        while (HoldBall)
        {
            // -- 与对方所有球员距离都大于X时
            if (DisToCloestEnemy() > PlayerDistanceThreshold)
            {
                int p = Random.Range(0, 100);
                if (p < 40)
                {
                    Move(Forward);
                }
                else if (p < 60)
                {
                    Move(Backward);
                }
                else if (p < 80)
                {
                    Move(Vector2.up);
                }
                else
                {
                    Move(Vector2.up);
                }
            }
            else //-- 与对方任一球员距离小于等于X时
            {
                int p = Random.Range(0, 100);
                if (p < 20)
                {
                    Move(Forward);
                }
                else if (p < 40)
                {
                    Move(Backward);
                }
                else if (p < 60)
                {
                    Move(Vector2.up);
                }
                else if (p < 80)
                {
                    Move(Vector2.up);
                }
                else if (p < 90)
                {
                    MoveAwayToCloestEnemy();
                }
                else
                {
                    MoveToCloestTeamate();
                }
            }
            yield return null;
        }

        btState = BTState.Enter;
    }


    IEnumerator AttackBallOwner()
    {
        while (GM.Ball.GetComponent<IBall>().GetOwner() != null)
        {
            var targetPos = GM.Ball.GetComponent<IBall>().GetOwner().transform.position;
            var dir = targetPos - transform.position;
            Move(dir.normalized);

            yield return null;
        }
        btState = BTState.Enter;
    }

    IEnumerator Defence()
    {
        GameObject ballOwner;
        while ((ballOwner = GM.Ball.GetComponent<IBall>().GetOwner()) != null)
        {
            var p = Random.Range(0, 100);
            var disToGate = Vector2.Distance(ballOwner.transform.position, selfGate.transform.position);

            if (disToGate < this.GateDistanceThreshold)
            {
                DefenceCloseToGate(p);
            }
            else
            {
                Defence(p);
            }
            yield return null;
        }

    }

    private void DefenceCloseToGate(int p)
    {
        if (p < 20)
        {
            MoveToBall();
        }
        else if (p < 40)
        {
            var enemy = FindCloestPlayer(Enemies);
            Vector2 dir = enemy.transform.position - transform.position;
            Move(dir.normalized);
        }
        else if (p < 60)
        {
            Move(Vector2.up);
        }
        else if (p < 80)
        {
            Move(Vector2.down);
        }
        else if (p < 90)
        {
            Move(Forward);
        }
        else
        {
            Move(Backward);
        }
    }

    private void Defence(int p)
    {
        if (p < 20)
        {
            MoveToBall();
        }
        else if (p < 40)
        {
            var enemy = FindCloestPlayer(Enemies);
            Vector2 dir = enemy.transform.position - transform.position;
            Move(dir.normalized);
        }
        else if (p < 60)
        {
            Move(Vector2.up);
        }
        else if (p < 80)
        {
            Move(Vector2.down);
        }
        else if (p < 90)
        {
            Move(Forward);
        }
        else
        {
            Move(Backward);
        }
    }

    void StartDefence()
    {
        btState = BTState.Defencing;
        coroutine = StartCoroutine(Defence());
    }

    void Move(Vector2 dir)
    {
        if (transform.position.y >= UpperBound)
        {
            dir.y = -Mathf.Abs(dir.y);
        }
        else if (transform.position.y <= Lowerbound)
        {
            dir.y = Mathf.Abs(dir.y);
        }

        var movement = Speed * GM.deltaTime * dir;
        transform.position = (Vector2)transform.position + movement;
    }

    void MoveToGate()
    {
        Vector2 dir = gate.transform.position - transform.position;
        var movement = Speed * GM.deltaTime * dir;
        transform.position = (Vector2)transform.position + movement;
    }

    void MoveToCloestTeamate()
    {
        var t = FindCloestPlayer(Teamates);
        if (t != null)
        {
            Vector2 dir = t.transform.position - transform.position;
            Move(dir.normalized);
        }
    }

    void MoveAwayToCloestEnemy()
    {
        var t = FindCloestPlayer(Enemies);
        Vector2 dir = transform.position - t.transform.position;
        Move(dir.normalized);
    }

    Player FindCloestPlayer(Player[] players)
    {
        if (players == null)
        {
            return null;
        }

        Player p = players[0];
        float mindis = float.MaxValue;
        for (int i = 1; i < players.Length; i++)
        {
            var dis = Vector2.Distance(transform.position, players[i].transform.position);
            if (dis < mindis)
            {
                mindis = dis;
                p = players[i];
            }
        }

        return p;
    }

    float DisToGate()
    {
        return Vector2.Distance(gate.transform.position, transform.position);
    }

    float DisToCloestTeamate()
    {
        float dis = float.MaxValue;
        for (int i = 0; i < Teamates.Length; i++)
        {
            dis = Mathf.Min(dis, Vector2.Distance(transform.position, Teamates[i].transform.position));
        }
        return dis;
    }

    float DisToCloestEnemy()
    {
        float dis = float.MaxValue;
        for (int i = 0; i < Enemies.Length; i++)
        {
            dis = Mathf.Min(dis, Vector2.Distance(transform.position, Enemies[i].transform.position));
        }
        return dis;
    }

    float DisToBallOwner()
    {
        return Vector2.Distance(GM.Ball.GetComponent<IBall>().GetOwner().transform.position, transform.position);
    }

    float DisToMiddle()
    {
        return Mathf.Abs(transform.position.x);
    }

    void GetEnemyList()
    {
        switch (side)
        {
            case IPlayer.PlayerSide.Human:
                Enemies = GM.enemies.Select(obj => obj.GetComponent<Player>()).Where(p => p != this).ToArray();
                break;
            case IPlayer.PlayerSide.AI:
                Enemies = GM.teammates.Select(obj => obj.GetComponent<Player>()).Where(p => p != this).ToArray();
                break;
        }
    }
    void GetTeamateList()
    {
        switch (side)
        {
            case IPlayer.PlayerSide.AI:
                Teamates = GM.enemies.Select(obj => obj.GetComponent<Player>()).Where(p => p != this).ToArray();
                break;
            case IPlayer.PlayerSide.Human:
                Teamates = GM.teammates.Select(obj => obj.GetComponent<Player>()).Where(p => p != this).ToArray();
                break;
        }
    }
}
