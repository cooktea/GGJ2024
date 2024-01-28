using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ball : MonoBehaviour, IBall
{
    [SerializeField] GameObject Owner;
    [SerializeField] BallState state;
    IPlayer ownerPlayer => Owner?.GetComponent<IPlayer>();
    Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] float speed;
    [SerializeField] Vector3 dir;
    [SerializeField] float noContactTime = 0.5f;
    [SerializeField] Vector2 nextWayPoint;
    Queue<Vector2> wayPoints = new Queue<Vector2>();

    [Header("Collision Info")]
    [SerializeField] List<GameObject> playersInCollied;
    Collider2D[] others = new Collider2D[10];
    Collider2D ballCollider;
    enum BallState
    {
        Free,
        Held,
    }


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ballCollider = GetComponent<CircleCollider2D>();
        wayPoints.Clear();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        DoMove(Time.fixedDeltaTime);

        if (speed > 0)
        {
            speed -= Time.fixedDeltaTime * 10;
            if (speed < 0)
            {
                speed = 0;
            }
        }
    }

    private void FreeMove(float dt)
    {
        if (wayPoints.Count > 0)
        {
            var ballPosition = new Vector2(transform.position.x, transform.position.y);
            var dis = Vector2.SqrMagnitude(ballPosition - nextWayPoint);
            if (dis <= 5f)
            {
                if (wayPoints.TryDequeue(out var wayPoint))
                {
                    nextWayPoint = wayPoint;
                    dir = (nextWayPoint - ballPosition).normalized;
                }
            }
        }

        transform.position += dir * speed * dt;
    }

    void StateFree(float dt)
    {
        if (this.playersInCollied.Count > 0)
        {
            var p = this.playersInCollied[0];
            p.GetComponent<IPlayer>().OnCatchBall(gameObject);
            state = BallState.Held;
            Owner = p;
        }
        else
        {
            FreeMove(dt);
        }
    }

    void StateHeld(float dt)
    {
        speed = 0;
        transform.position = Owner.transform.position;
    }

    public void SetOwner(GameObject owner)
    {
        Owner = owner;
        state = BallState.Held;
    }

    #region IBall
    public GameObject GetOwner()
    {
        return Owner;
    }

    public void Shoot()
    {
        //Debug.Log(string.Join(" ", wayPoints.Take(5)));
        if (wayPoints.Count > 1)
        {
            wayPoints.Dequeue();
            nextWayPoint = wayPoints.Dequeue();
            state = BallState.Free;
            StartCoroutine(resetContact());
        }
        Owner.GetComponent<IPlayer>().Shoot();
        Owner = null;
    }

    IEnumerator resetContact()
    {
        var player = Owner;
        player.GetComponent<Collider2D>().enabled = false;
        // todo: fix time
        yield return new WaitForSeconds(noContactTime);
        player.GetComponent<Collider2D>().enabled = true;
    }

    public void SetPath(List<Vector2> pathPoints)
    {
        wayPoints = new Queue<Vector2>(pathPoints);
    }

    public float SetInitSpeed(float speed)
    {
        this.speed = speed;
        return speed;
    }

    public void DoMove(float deltaTime)
    {
        switch (state)
        {
            case BallState.Free:
                StateFree(deltaTime);
                break;
            case BallState.Held:
                StateHeld(deltaTime);
                break;
            default:
                break;
        }
    }

    public bool CheckOutLine()
    {
        return isOutLine;
    }

    public bool CheckCatched()
    {
        return ownerPlayer != null;
    }

    public void SetOutLine(bool value)
    {
        isOutLine = value;
    }

    bool isOutLine;
    #endregion IBall

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IPlayer p = collision.GetComponent<IPlayer>();
        if (p != null)
        {
            this.playersInCollied.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IPlayer p = collision.GetComponent<IPlayer>();
        if (p != null)
        {
            this.playersInCollied.Remove(collision.gameObject);
        }
    }

}
