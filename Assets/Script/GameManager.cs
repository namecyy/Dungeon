using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameState InitState = GameState.MainMenu;
    [SerializeField] private GameState currentState;
    private int levelCount = 1;

    public int GetLevelCounter()
    {
        return levelCount;
    }
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetState(InitState);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerDied += GameOver;
        GameEvents.NextLevel += NextLevel;

        GameEvents.OnBossDied += Victory;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerDied -= GameOver;
        GameEvents.NextLevel -= NextLevel;

        GameEvents.OnBossDied -= Victory;
    }
    
    

    

    public GameState GetState()
    {
        return currentState;
    }

    //ÔÝÍ£×´Ì¬ÁÐ±í
    private static readonly List<GameState> PausedStates = new List<GameState>
    {
        GameState.Menu,
        GameState.GameOver
    };

    private void SetState(GameState newState)
    {
        currentState = newState;
        GameEvents.OnGameStateChanged?.Invoke(newState);

        Time.timeScale = (PausedStates.Contains(currentState) ? 0f : 1f);
    }

    //ÔÝÍ£
    public void TogglePause()
    {
        if(currentState == GameState.Menu)
        {
            SetState(GameState.Playing);   
        }else if(currentState == GameState.Playing)
        {
            SetState(GameState.Menu);
        }
    }

    public void StartGame()
    {
        levelCount = 1;
        SetState(GameState.Playing);
        PlayerData.Instance.Reset();
        SceneManager.LoadScene("GameScene");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Exit");
    }

    public void MainMenu()
    {
        SetState(GameState.MainMenu);
        SceneManager.LoadScene("MainMenu");
    }

    public void Restart()
    {
        levelCount = 1;
        PlayerData.Instance.Reset();
        SetState(GameState.Playing);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void GameOver()
    {
        SetState(GameState.GameOver);
    }

    private void NextLevel()
    {
        levelCount++;
        /*SetState(GameState.Playing);*/
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Victory()
    {
        SetState(GameState.Victory);
        SceneManager.LoadScene("Victory");
        Debug.Log("Victory");
    }
}
