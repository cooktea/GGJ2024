using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
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
    List<GameObject> pointObjects = new List<GameObject>();

    [SerializeField]
    LineRenderer lineRenderer;
 
    Vector2 Vector3ToVector2(Vector3 pos)
    {
        return new Vector2 (pos.x, pos.y); 
    }

    GameObject CreatePathPoint(Vector3 pos)
    {
        var prefab = Resources.Load("Prefabs/PathPoint");
        var ob = Instantiate(prefab) as GameObject;
        ob.transform.position = pos;
        ob.transform.SetParent(canvas.transform);
        return ob;
    }

    public void OnEnterPlayer(GameObject player)
    {
        Debug.Log("OnEnterPlayer");
        currentPlayer = player;
    }

    public void OnExitPlayer()
    {
        Debug.Log("OnExitPlayer");
        currentPlayer = null;
    }

    public void OnLeftButtonDown()
    {
        Debug.Log("OnLeftButtonDown");
        pathPoints.Clear();
        pathPoints.Add(Vector3ToVector2(Input.mousePosition));
        pointObjects.Add(CreatePathPoint(Input.mousePosition));
    }

    public void OnLeftButtonHold()
    {
        if(Time.time > lastCheckTs + checkTick)
        {
            lastCheckTs = Time.time;
            var lastPoint = pathPoints[pathPoints.Count - 1];
            if ((Vector3ToVector2(Input.mousePosition) - lastPoint).magnitude > 10)
            {
                pathPoints.Add(Vector3ToVector2 (Input.mousePosition));
                pointObjects.Add(CreatePathPoint(Input.mousePosition));

                lineRenderer.positionCount = pathPoints.Count;
                lineRenderer.SetPositions(pathPoints.Select(p => new Vector3(p.x, p.y, 1f)).ToArray());
            }
        }
        Debug.Log("OnLeftButtonHold");
    }

    public void OnLeftButtonRelease()
    {
        var Iball = GM.ball.GetComponent<IBall>();
        Iball.SetInitSpeed(200);
        Iball.SetPath(pathPoints);
        Iball.Shoot();
        foreach (var point in pointObjects)
        {
            Destroy(point);
        }
        pointObjects.Clear();
        lineRenderer.positionCount = 0;
        lineRenderer.SetPositions(new Vector3[] { });
        Debug.Log("OnLeftButtonRelease");
        Debug.Log(pathPoints.Count);
    }

    public void OnMouseMove(Vector2 oldPosition, Vector2 newPosition)
    {
        pathPoints.Add(newPosition);
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
            PointerEventData pointerEvent = new PointerEventData(EventSystem.current);
            pointerEvent.position = Input.mousePosition;
            GraphicRaycaster gr = canvas.GetComponent<GraphicRaycaster>();
            List<RaycastResult> results = new List<RaycastResult>();
            gr.Raycast(pointerEvent, results);
            bool isPointPlayer = false;
            if (results.Count > 0)
            {
                foreach (var item in results)
                {
                    if (item.gameObject.GetComponent<IPlayer>() != null)
                    {
                        isPointPlayer = true;
                        if (currentPlayer != item.gameObject)
                        {
                            OnEnterPlayer(item.gameObject);
                            break;
                        }
                    }
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
