using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour, IGameManager
{
    enum State
    {
        End,
        Start,
        Pause,
        BulletTime
    }

    int currentLevel = 1;
    List<LevelInfo> levels = new List<LevelInfo>();
    List<Vector3> teammatesInitPositions = new List<Vector3>();
    List<Vector3> enemiesInitPositions = new List<Vector3>();

    public List<GameObject> teammates = new List<GameObject>();
    public List<GameObject> enemies = new List<GameObject>();
    GameObject ball;
    public GameObject Ball => ball;

    public TextMeshProUGUI TextScore;
    public TextMeshProUGUI TimeRecord;
    public Button StartButton;

    public GameObject GateHuman;
    public GameObject GateAI;

    float leftTime = 0;
    float timeSpeed = 1;
    public float deltaTime => Time.deltaTime * timeSpeed;
    State currentState = State.End;

    int ScoreHuman = 0;
    int ScoreAI = 0;

    [SerializeField] LevelInfo LevelInfo;
    public void BallIn(IPlayer.PlayerSide side)
    {
        if (side == IPlayer.PlayerSide.AI)
        {
            ScoreHuman++;
        }
        else
        {
            ScoreAI++;
        }
        TextScore.text = $"{ScoreHuman}";
        Debug.Log($"Human {ScoreHuman}:{ScoreAI} AI");

        if (ScoreHuman >= LevelInfo.ScoreTarget)
        {
            Debug.Log("Human win!");
            if (SceneManager.sceneCount >= LevelInfo.Level + 1)
            {
                SceneManager.LoadScene(LevelInfo.Level + 1);
            }
        }
        else
        {
            RestartGame();
        }
    }

    private void RestartGame()
    {
        EndGame();
    }

    public void EndGame()
    {
        ClearGameObject();

        StartButton.enabled = true;
        currentState = State.End;
        currentLevel = LevelInfo.Level;
        leftTime = LevelInfo.GameTime;
    }

    public int GetCurrentScore()
    {
        throw new System.NotImplementedException();
    }

    public int GetLastGameSecond()
    {
        throw new System.NotImplementedException();
    }

    void InitPlayerPositions()
    {
        // Teammates
        teammatesInitPositions.Add(new Vector3(800, 540, 1));
        teammatesInitPositions.Add(new Vector3(650, 340, 1));
        teammatesInitPositions.Add(new Vector3(650, 780, 1));
        teammatesInitPositions.Add(new Vector3(500, 140, 1));
        teammatesInitPositions.Add(new Vector3(500, 980, 1));

        // Enemies
        enemiesInitPositions.Add(new Vector3(1100, 540, 1));
        enemiesInitPositions.Add(new Vector3(1300, 340, 1));
        enemiesInitPositions.Add(new Vector3(1300, 780, 1));
        enemiesInitPositions.Add(new Vector3(1500, 140, 1));
        enemiesInitPositions.Add(new Vector3(1500, 980, 1));
    }

    public void NextRound()
    {
        throw new System.NotImplementedException();
    }

    public void StartBulletTime()
    {
        timeSpeed = 0.1f;
    }

    public void StartNewGame()
    {
        ClearGameObject();

        StartButton.enabled = false;
        currentState = State.Start;
        currentLevel = LevelInfo.Level;
        leftTime = LevelInfo.GameTime;
        CreateTeammatesAndEnemiesAndBall(LevelInfo);
        SetPlayerAndBallPosition(IPlayer.PlayerSide.Human);
    }

    void CreateTeammatesAndEnemiesAndBall(LevelInfo levelInfo)
    {
        var prefabBall = Resources.Load("Prefabs/Ball") as GameObject;
        ball = Instantiate(prefabBall);

        var prefabTeammate = Resources.Load("Prefabs/Teammate") as GameObject;
        for (int i = 0; i < levelInfo.HumanPlayerPosition.Count; i++)
        {
            var teammate = Instantiate(prefabTeammate);
            teammates.Add(teammate);
            teammate.GetComponent<Player>().GM = this;
        }

        var prefabEnemy = Resources.Load("Prefabs/Enemy") as GameObject;
        for (int i = 0; i < levelInfo.AIPlayerPosition.Count; i++)
        {
            var enemy = Instantiate(prefabEnemy);
            enemies.Add(enemy);
            enemy.GetComponent<Player>().GM = this;
        }
    }

    void SetPlayerAndBallPosition(IPlayer.PlayerSide ballHolderSide)
    {
        for (int i = 0; i < teammates.Count; i++)
        {
            var p = teammates[i];
            p.transform.position =
                new Vector3(
                    LevelInfo.HumanPlayerPosition[i].x,
                    LevelInfo.HumanPlayerPosition[i].y,
                    1);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            var p = enemies[i];
            p.transform.position =
                new Vector3(
                    LevelInfo.AIPlayerPosition[i].x,
                    LevelInfo.AIPlayerPosition[i].y,
                    1);
        }


        GameObject holder;
        if (ballHolderSide == IPlayer.PlayerSide.AI)
        {
            holder = enemies[Random.Range(0, enemies.Count)];
        }
        else
        {
            holder = teammates[Random.Range(0, teammates.Count)];
        }
        ball.GetComponent<Ball>().SetOwner(holder);
        holder.GetComponent<IPlayer>().OnCatchBall(ball);
        ball.transform.position = holder.transform.position;
    }


    void ClearGameObject()
    {
        foreach (var obj in teammates)
        {
            Destroy(obj);
        }

        foreach (var obj in enemies)
        {
            Destroy(obj);
        }

        teammates.Clear();
        enemies.Clear();

        if (ball is not null)
        {
            Destroy(ball);
            ball = null;
        }
    }

    void Start()
    {
    }

    void Update()
    {
        if (currentState == State.Start || currentState == State.BulletTime)
            leftTime -= Time.deltaTime * timeSpeed;
        TimeRecord.text = string.Format("{0:N2}s", leftTime);
    }

    public void EndBulletTime()
    {
        timeSpeed = 1;
    }

    #region AI SUPPORT
    public IPlayer.PlayerSide BallSide()
    {
        return Ball.GetComponent<Ball>().GetOwner().GetComponent<IPlayer>().Side;
    }
    #endregion
}
