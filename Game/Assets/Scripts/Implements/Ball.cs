using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour, IBall
{
    GameObject Owner;
    IPlayer ownerPlayer => Owner?.GetComponent<IPlayer>();

    [Header("Movement")]
    [SerializeField] float speed;
    [SerializeField] Vector3 dir;
    [SerializeField] float noContactTime = 0.5f;
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
            for(int i = 0; i < num; i++)
            {
                var player = others[i].GetComponentInParent<IPlayer>();
                if (player is not null)
                {
                    player.OnCatchBall(gameObject);
                    Owner = others[i].gameObject;
                    state = BallState.Held;
                    break;
                }
            }
        }
        else
        {
            FreeMove();
        }
    }

    void StateHeld() {
        transform.position = Owner.transform.position + (Vector3)Random.insideUnitCircle;
    }

    #region IBall
    public GameObject GetOwner()
    {
        return Owner;
    }

    public void Shoot()
    {
        Owner= null;
        state = BallState.Free;
        StartCoroutine(resetContact());
    }

    IEnumerator resetContact()
    {
        var filter = contactFilter;
        contactFilter = contactFilter.NoFilter();
        yield return new WaitForSeconds(noContactTime);
        contactFilter = filter;
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
