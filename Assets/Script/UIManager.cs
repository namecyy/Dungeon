using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject BossHealthBar;
    private void Start()
    {
        menuPanel.SetActive(false);
        gameOverPanel.SetActive(false);

    }

    private void OnEnable()
    {
        GameEvents.OnGameStateChanged += OnStateChanged;
        GameEvents.OnBossHealthChanged += BossBar;
    }

    private void OnDisable()
    {
        GameEvents.OnGameStateChanged -= OnStateChanged;
        GameEvents.OnBossHealthChanged -= BossBar;
    }

    public void BackGamebtn() => GameManager.Instance.TogglePause();

    public void OnRestartClick() => GameManager.Instance.Restart();

    public void OnMainMenuClick() => GameManager.Instance.MainMenu();

    private void OnStateChanged(GameState newState)
    {
        gameOverPanel.SetActive(newState == GameState.GameOver);
        menuPanel.SetActive(newState == GameState.Menu);
    }

    private void BossBar(int current,int max)
    {
        if (current == max) BossHealthBar.SetActive(true);
        if (current == 0) BossHealthBar.SetActive(false);
    }


}
