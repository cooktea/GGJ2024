using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class MouseController : MonoBehaviour, IMouseController
{
	enum State
	{
		IDLE,
		HOLD
	}

	GameManager GM;
	GameObject currentPlayer;
	public Canvas canvas;
	State currentState = State.IDLE;
	float checkTick = 0.05f;
	float lastCheckTs = 0;

	List<Vector2> pathPoints = new List<Vector2>(); 

	[SerializeField]
	LineRenderer lineRenderer;
 
	Vector2 Vector3ToVector2(Vector3 pos)
	{
		return new Vector2 (pos.x, pos.y); 
	}

	public void OnEnterPlayer(GameObject player)
	{
		currentPlayer = player;
	}

	public void OnExitPlayer()
	{
		currentPlayer = null;
	}

	public void OnLeftButtonDown()
	{
		pathPoints.Clear();
		AddPointToPath(Input.mousePosition);
	}

	public void OnLeftButtonHold()
	{
		lineRenderer.enabled = true;
		if(Time.time > lastCheckTs + checkTick)
		{
			lastCheckTs = Time.time;
			var lastPoint = pathPoints[pathPoints.Count - 1];
			if ((Vector3ToVector2(Input.mousePosition) - lastPoint).magnitude > 1)
			{
				AddPointToPath(Input.mousePosition);

				lineRenderer.positionCount = pathPoints.Count;
				lineRenderer.SetPositions(pathPoints.Select(p => new Vector3(p.x, p.y, 1f)).ToArray());
			}
		}
	}

	void AddPointToPath(Vector3 point)
	{
		var pathPoint = Camera.main.ScreenToWorldPoint(new Vector3(point.x, point.y, 1f));
		pathPoints.Add(Vector3ToVector2(pathPoint));
	}

	public void OnLeftButtonRelease()
	{
		var Iball = GM.ball.GetComponent<IBall>();
		Iball.SetInitSpeed(200);
		Iball.SetPath(pathPoints);
		Iball.Shoot();

		lineRenderer.positionCount = 0;
		lineRenderer.SetPositions(new Vector3[] { });
		lineRenderer.enabled = false;
	}

	// Start is called before the first frame update
	void Start()
	{

		GM = GetComponent<GameManager>();
	}

	// Update is called once per frame
	void Update()
	{
		if (currentState == State.IDLE)
		{
			bool isPointPlayer = false;
			var mousePositionInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			foreach (var teammate in GM.teammates)
			{
				if (((Vector2)teammate.transform.position - (Vector2)mousePositionInWorld).magnitude <= 1f)
				{
					isPointPlayer = true;
					OnEnterPlayer(teammate);
					break;
				}
			}

			if (currentPlayer != null && !isPointPlayer)
			{
				OnExitPlayer();
			}

			if (Input.GetMouseButton(((int)MouseButton.LeftMouse)))
			{
				if (currentPlayer != null && GM.ball.GetComponent<Ball>().GetOwner() == currentPlayer)
				{
					currentState = State.HOLD;
					OnLeftButtonDown();
				}
			}

		}
		else
		{
			if (Input.GetMouseButton(((int)MouseButton.LeftMouse)))
			{
				OnLeftButtonHold();
			}

			if (Input.GetMouseButtonUp(((int)MouseButton.LeftMouse)))
			{
				OnLeftButtonRelease();
				currentState = State.IDLE;
			}

		}
	}
}
