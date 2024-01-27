using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour, IBall
{
    GameObject Owner;

    [Header("Movement")]
    [SerializeField] float speed;
    [SerializeField] Vector3 dir;
    Queue<Vector2> wayPoint = new Queue<Vector2>();
    Vector2 nextWayPoint = Vector2.zero;

    [Header("Collision Info")]
    [SerializeField] ContactFilter2D contactFilter;
    Collider2D[] others = new Collider2D[10];
    Collider2D ballCollider;
    enum BallState
    {
        Free,
        Held,
    }
    BallState state;


    // Start is called before the first frame update
    void Start()
    {
        ballCollider = GetComponent<CircleCollider2D>();
        state = BallState.Free;
        wayPoint.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case BallState.Free:
                StateFree();
                break;
            case BallState.Held:
                StateHeld();
                break;
            default:
                break;
        }
    }

    private void FreeMove()
    {
        var dis = Vector3.Distance(transform.position, nextWayPoint);
        if (dis <= float.Epsilon)
        {
            if (wayPoint.TryDequeue(out nextWayPoint))
            {
                dir = (Vector3)nextWayPoint - transform.position;
            }
        }

        transform.position = speed * Time.deltaTime * dir;
    }

    void StateFree()
    {
        int num = ballCollider.OverlapCollider(contactFilter, others);
        if (num > 0)
        {
            foreach (var item in others)
            {
                var player = item.GetComponentInParent<IPlayer>();
                if (player is not null)
                {
                    player.OnCatchBall();
                    Owner = item.gameObject;
                }
            }
        }
        else
        {
            FreeMove();
        }
    }

    void StateHeld() { }

    #region IBall
    public GameObject GetOwner()
    {
        return Owner;
    }

    public void Shoot()
    {
        throw new System.NotImplementedException();
    }

    public void SetPath(List<Vector2> pathPoints)
    {
        wayPoint = new Queue<Vector2>(pathPoints);
    }

    public float SetInitSpeed(float speed)
    {
        this.speed = speed;
        return speed;
    }

    public void DoMove(float deltaTime)
    {
        throw new System.NotImplementedException();
    }

    public bool CheckIsScore()
    {
        throw new System.NotImplementedException();
    }

    public bool CheckOutLine()
    {
        throw new System.NotImplementedException();
    }

    public bool CheckCatched()
    {
        throw new System.NotImplementedException();
    }

    #endregion IBall
}
