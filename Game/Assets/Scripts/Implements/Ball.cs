using PlasticGui;
using System;
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
    [SerializeField] Vector2[] waypointList;
    Queue<Vector2> wayPoints = new Queue<Vector2>();

    [Header("Collision Info")]
    [SerializeField] List<GameObject> playersInCollied;
    Collider2D[] others = new Collider2D[10];
    Collider2D ballCollider;
    RectTransform rectTransform;
    enum BallState
    {
        Free,
        Held,
    }


    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        rb = GetComponent<Rigidbody2D>();
        ballCollider = GetComponent<CircleCollider2D>();
        wayPoints.Clear();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        DoMove(Time.deltaTime);
        waypointList = wayPoints.ToArray();
    }

    private void FreeMove(float dt)
    {
        var ballPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + Screen.height);
        var dis = Vector2.Distance(ballPosition, nextWayPoint);
        Vector2 wayPoint;
        if (dis <= 2f)
        {
            if (wayPoints.TryDequeue(out wayPoint))
            {
                nextWayPoint = wayPoint;
                Debug.Log(nextWayPoint);
            }
        }

        dir = (nextWayPoint - ballPosition).normalized;
        rb.velocity = speed * dt * dir;
    }

    void StateFree(float dt)
    {
        if (this.playersInCollied.Count > 0)
        {
            var p = this.playersInCollied[0];
            p.GetComponent<IPlayer>().OnCatchBall(gameObject);
            this.Owner = p;
            state = BallState.Held;
        }
        else
        {
            //FreeMove(dt);
        }
    }

    void StateHeld(float dt)
    {
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
        Debug.Log(string.Join(" ", wayPoints.Take(5)));
        Owner = null;
        nextWayPoint = wayPoints.Skip(1).First();
        wayPoints.Dequeue();
        state = BallState.Free;
        StartCoroutine(resetContact());
    }

    IEnumerator resetContact()
    {
        ballCollider.enabled = false;
        // todo: fix time
        yield return new WaitForSeconds(noContactTime);
        ballCollider.enabled = true;
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

    public bool CheckIsScore()
    {
        return isScore;
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

    public void SetIsScore(bool value)
    {
        isScore = value;
    }

    bool isOutLine;
    bool isScore;
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
