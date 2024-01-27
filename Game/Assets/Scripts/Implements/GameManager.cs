using Codice.Client.BaseCommands.BranchExplorer;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
    List<Vector2> teammatesInitPositions = new List<Vector2>();
    List<Vector2> enemiesInitPositions = new List<Vector2>();

    List<GameObject> teammates = new List<GameObject>();
    List<GameObject> enemies = new List<GameObject>();
    public GameObject ball;

    public TextMeshProUGUI TextScore;
    public TextMeshProUGUI TimeRecord;
    public Button StartButton;

    float leftTime = 0;
    float timeSpeed = 1;
    State currentState = State.End;


    public void BallIn()
    {
        throw new System.NotImplementedException();
    }

    public void EndGame()
    {
        throw new System.NotImplementedException();
    }

    public int GetCurrentScore()
    {
        throw new System.NotImplementedException();
    }

    public int GetLastGameSecond()
    {
        throw new System.NotImplementedException();
    }

    public void Init()
    {
        InitLevels();
        InitPlayerPositions();
    }

    void InitPlayerPositions() 
    { 
        // Teammates
        teammatesInitPositions.Add(new Vector3(800, -540, 0));
        teammatesInitPositions.Add(new Vector3(650, -340, 0));
        teammatesInitPositions.Add(new Vector3(650, -780, 0));
        teammatesInitPositions.Add(new Vector3(500, -140, 0));
        teammatesInitPositions.Add(new Vector3(500, -980, 0));

        // Enemies
        enemiesInitPositions.Add(new Vector3(1100, -540, 0));
        enemiesInitPositions.Add(new Vector3(1300, -340, 0));
        enemiesInitPositions.Add(new Vector3(1300, -780, 0));
        enemiesInitPositions.Add(new Vector3(1500, -140, 0));
        enemiesInitPositions.Add(new Vector3(1500, -980, 0));
    }


    void InitLevels()
    {
        var level1 = new LevelInfo();
        level1.TeammateCount = 3;
        level1.EnemyCount = 1;
        level1.Target = 1;
        level1.GameTime = 120f;

        var level2 = new LevelInfo();
        level2.TeammateCount = 5;
        level2.EnemyCount = 3;
        level2.Target = 3;
        level2.GameTime = 120f;

        var level3 = new LevelInfo();
        level3.TeammateCount = 3;
        level3.EnemyCount = 5;
        level3.Target = 5;
        level3.GameTime = 180f;

        levels.Add(level1);
        levels.Add(level2);
        levels.Add(level3);
    }

    public void NextRound()
    {
        throw new System.NotImplementedException();
    }

    public void StartBulletTime()
    {
        timeSpeed = 0.1f;
    }

    public void StartNewGame(int level = 1)
    {
        StartButton.enabled = false;
        currentState = State.Start;
        currentLevel = level;
        var levelInfo = levels[currentLevel - 1];
        leftTime = levelInfo.GameTime;
        CreateTeammatesAndEnemiesAndBall(levelInfo);
    }

    void CreateTeammatesAndEnemiesAndBall(LevelInfo levelInfo)
    {
        var prefabBall = Resources.Load("Prefabs/Ball") as GameObject;
        ball = Instantiate(prefabBall);
        ball.transform.SetParent(GameObject.Find("Canvas").transform);

        int teammateCount = levelInfo.TeammateCount;
        int enemyCount = levelInfo.EnemyCount;

        var prefabTeammate = Resources.Load("Prefabs/Teammate") as GameObject;
        for (int i = 0; i<teammateCount; i++)
        {
            var teammate = Instantiate(prefabTeammate);
            teammate.transform.SetParent(GameObject.Find("Canvas").transform);
            var rectTransform = teammate.GetComponent<RectTransform>();
            rectTransform.anchoredPosition3D = teammatesInitPositions[i];
            teammates.Add(teammate);
        }

        var prefabEnemy = Resources.Load("Prefabs/Enemy") as GameObject;
        for (int i = 0;i<enemyCount; i++) 
        {
            var enemy = Instantiate(prefabEnemy);
            enemy.transform.SetParent(GameObject.Find("Canvas").transform);
            var rectTransform = enemy.GetComponent<RectTransform>();
            rectTransform.anchoredPosition3D = enemiesInitPositions[i];
            enemies.Add(enemy);
        }

        var holder = teammates[Random.Range(0, teammates.Count)];
        ball.GetComponent<Ball>().SetOwner(holder);
        holder.GetComponent<IPlayer>().OnCatchBall(ball);
        ball.transform.position = holder.transform.position;
    }

    void Start()
    {
        Init();   
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
}
