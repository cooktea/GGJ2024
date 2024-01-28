using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour, IPlayer
{
    float GateToMiddle = 7.4f;
    float UpperBound = 3.9f;
    float Lowerbound = -3.9f;

    enum PlayerState
    {
        AttackingWithBall,
        TryGetBallFromGround,
        TryGetBallFromEnemy,
        Defencing,
        Roaming,
        Still,
    }

    [Header("AI")]
    [SerializeField] PlayerState playerState = PlayerState.Still;
    [SerializeField] float Speed = 30;
    [SerializeField] float PlayerDistanceThreshold;
    [SerializeField] float GateDistanceThreshold;

    public bool HoldBall {  get; private set; }
    public bool OpponentHoldBall {  get; private set; }

    public IPlayer.PlayerSide Side => side;

    public IBall BallRef { get; private set; }
    public GameManager GM;
    public IBall GMBall => GM.Ball.GetComponent<Ball>();
    Gate gate;


    [SerializeField] IPlayer.PlayerSide side;
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
                break;
            default:
                gate = GM.GateHuman.GetComponent<Gate>();
                break;
        }
    }

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

    private void Update()
    {
        if (HoldBall)
        {
            AttackWithBall();
        }
        else
        {
            if (GM.BallSide() != side)
            {
                if (DisToBallOwner() < PlayerDistanceThreshold)
                {
                    AttackBallOwner();
                }
                else
                {
                    Defence();
                }
            }
            else
            {
                Defence();
            }
        }
    }

    void AttackWithBall() {
        if (DisToCloestEnemy() > PlayerDistanceThreshold)
        {
            int p = Random.Range(0, 100);
            if (p < 40 )
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
    }
    void AttackBallOwner() { }
    void Defence() { }

    void Move(Vector2 dir)
    {
        var movement = Speed * dir;
        transform.position =  (Vector2)transform.position + movement;
    }

    float DisToGate()
    {
        return Vector2.Distance(gate.transform.position, transform.position);
    }

    float DisToCloestTeamate() {
        float dis = float.MaxValue;
        for(int i = 0; i < Teamates.Length; i++)
        {
            dis = Mathf.Min(dis, Vector2.Distance(transform.position, Teamates[i].transform.position));
        }
        return dis;
    }

    float DisToCloestEnemy() {
        float dis = float.MaxValue;
        for (int i = 0; i < Enemies.Length; i++)
        {
            dis = Mathf.Min(dis, Vector2.Distance(transform.position, Enemies[i].transform.position));
        }
        return dis;
    }

    float DisToBallOwner()
    {
        return Vector2.Distance(GMBall.GetOwner().transform.position, transform.position);
    }

    float DisToMiddle()
    {
        return Mathf.Abs(transform.position.x);
    }

    void GetEnemyList() {
        Enemies = GM.enemies.Select(obj=> obj.GetComponent<Player>()).Where(p => p != this).ToArray();
    }
    void GetTeamateList() {
        Teamates = GM.teammates.Select(obj => obj.GetComponent<Player>()).Where(p => p != this).ToArray();
    }
}
