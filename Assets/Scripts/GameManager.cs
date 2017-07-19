using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public float levelStartDelay = 2f;
    public float turnDelay = 0.1f;
    public static GameManager instance = null;
    public BoardManager boardScript;
    public int playerEnergyPoints = 100;
    [HideInInspector] public bool playersTurn = true;

    private Text levelText;
    private GameObject levelImage;
    private int level = 1;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doingSetup;
    private GameObject restartButton;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
        InitGame();
    }

    void Start()
    {
        SceneManager.sceneLoaded += delegate (Scene scene, LoadSceneMode mode)
        {
            level++;
            InitGame();
        };
    }

    //private void OnLevelWasLoaded(int level)
    //{
    //    InitGame();
    //}

    void InitGame()
    {
        doingSetup = true;
        restartButton = GameObject.Find("Startbutton");
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Day " + level;
        levelImage.SetActive(true);
        restartButton.SetActive(false);
        Invoke("HideLevelImage", levelStartDelay);

        enemies.Clear();
        boardScript.SetupScene(level);
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    public void GameOver()
    {
        levelImage.SetActive(true);
        levelText.text = "You lasted " + level + " days.";
        enabled = false;
        restartButton.SetActive(true);
        playerEnergyPoints = 100;
        level = 1;
        Destroy(gameObject);
    }

    public void GoToGameover()
    {
        SceneManager.LoadScene("GameOverScene");
    }

    // Update is called once per frame
    void Update() {
        if (playersTurn || enemiesMoving || doingSetup)
        {
            return;
        }
        StartCoroutine(MoveEnemies());
	}

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if(enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }

        for(int i = 0; i< enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }
        playersTurn = true;
        enemiesMoving = false;
    }
}
