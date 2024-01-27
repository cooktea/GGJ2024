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
	[SerializeField] ContactFilter2D contactFilter;
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
		DoMove(Time.deltaTime);

		if (speed > 0)
		{
			speed -= Time.deltaTime * 10;
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
			if (dis <= 1f)
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
		int num = ballCollider.OverlapCollider(contactFilter, others);
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				var player = others[i].GetComponentInParent<IPlayer>();
				if (player is not null)
				{
					player.OnCatchBall(gameObject);
					Owner = others[i].gameObject;
					state = BallState.Held;
					continue;
				}
			}
		}
		else
		{
			FreeMove(dt);
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
		var filter = contactFilter;
		var mask = contactFilter.layerMask;
		mask = LayerMask.NameToLayer("None");
		filter.layerMask = mask;
		// todo: fix time
		yield return new WaitForSeconds(noContactTime);
		contactFilter = filter;
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
}
