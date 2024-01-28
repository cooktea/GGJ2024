using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;

public enum BTState
{
    AttackingWithBall,
    TryGetBallFromGround,
    Defencing,
    Roaming,
    Enter,
}

public class Player : MonoBehaviour, IPlayer
{
    [Header("AI")]
    [SerializeField] BTState btState = BTState.Enter;
    [SerializeField] float Speed = 1;
    [SerializeField] float PlayerDistanceThreshold;
    [SerializeField] float GateDistanceThreshold;
    [SerializeField] Vector2 dir;
    [SerializeField] float AIUpdateTime = 0.125f;

    Vector2 birthPlace;

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
        btState = BTState.Enter;
        birthPlace = transform.position;
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
                Debug.Log($"{name} BeginAttackWithBall");
                return;
            }

            if (GM.Ball.GetComponent<IBall>().GetOwner() == null)
            {
                StartMoveToBall();
                Debug.Log($"{name} move to ball");
                return;
            }

            if (GM.BallSide() != side)
            {
                StartDefence();
                Debug.Log($"{name} defence still");
            }
        }
    }

    void StartRoaming()
    {
        btState = BTState.Roaming;
        coroutine = null;
    }

    private void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        var movement = Speed * GM.deltaTime * dir;
        transform.position = (Vector2)transform.position + movement;
    }

    void StartMoveToBall()
    {
        btState = BTState.TryGetBallFromGround;
        StartCoroutine(MoveToPickBall());
    }

    private IEnumerator MoveToPickBall()
    {
        while (GM.Ball.GetComponent<IBall>().GetOwner() == null)
        {
            MoveToBall();
            yield return new WaitForSeconds(AIUpdateTime);
        }
        btState = BTState.Enter;
    }

    void MoveToBall()
    {
        var dir = GM.Ball.transform.position - transform.position;
        SetDir(dir.normalized);
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
            MoveToGate();

            yield return new WaitForSeconds(AIUpdateTime);
        }

        btState = BTState.Enter;
    }

    IEnumerator Defence()
    {
        GameObject ballOwner = null, preOwner = null;
        while (preOwner == ballOwner && (ballOwner = GM.Ball.GetComponent<IBall>().GetOwner()) != null)
        {
            if (DisToBirthPlace() > 1.0f)
            {
                MoveToBirthPlace();
            }
            else
            {
                RandomMove();
            }

            yield return new WaitForSeconds(AIUpdateTime);

            preOwner = ballOwner;
        }
        btState = BTState.Enter;
    }
    void StartDefence()
    {
        btState = BTState.Defencing;
        coroutine = StartCoroutine(Defence());
    }

    void SetDir(Vector2 dir)
    {
        this.dir = Data.ClampDir(this.transform, dir).normalized;
    }

    void RandomMove()
    {
        int p = Random.Range(0, 100);
        if (p < 25)
        {
            SetDir(Vector2.up);
        }
        else if (p < 50)
        {
            SetDir(Vector2.down);
        }
        else if (p < 75)
        {
            SetDir(Forward);
        }
        else
        {
            SetDir(Backward);
        }
    }

    void MoveToGate()
    {
        
        if (side == IPlayer.PlayerSide.AI)
        {
            int p = Random.Range(0, 100);
            if (p < 50)
            {
                var path = new List<Vector2>
                {
                    (Vector2)transform.position + Forward,
                    gate.transform.position,
                };
                BallRef.SetPath(path);
                BallRef.Shoot();
                Shoot();
            }
        }

        Vector2 dir = gate.transform.position - transform.position;
        SetDir(dir);
    }

    void MoveToBirthPlace()
    {
        Vector2 dir = birthPlace - (Vector2)transform.position;
        SetDir(dir.normalized);
    }

    float DisToBirthPlace()
    {
        return Vector2.Distance(transform.position, birthPlace);
    }

    void MoveToCloestTeamate()
    {
        var t = FindCloestPlayer(Teamates);
        if (t != null)
        {
            Vector2 dir = t.transform.position - transform.position;
            SetDir(dir.normalized);
        }
    }

    void MoveAwayToCloestEnemy()
    {
        var t = FindCloestPlayer(Enemies);
        Vector2 dir = transform.position - t.transform.position;
        SetDir(dir.normalized);
    }

    Player FindCloestPlayer(Player[] players)
    {
        if (players == null || players.Length == 0)
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

    bool IamCloestPlayerToEnemyHolder()
    {
        return GM.CloestPlayerToEnemyHolder() == this as IPlayer;
    }
}
