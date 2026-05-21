using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject gameOverPanel;
    public GameObject victoryPanel;

    void Awake()
    {
        Instance = this;
    }

    public void GameOver()
    {
        Time.timeScale = 1f;
        AudioManager.Instance.PlaySFX(AudioManager.Instance.gameOver);
        gameOverPanel.SetActive(true);
    }

    public void Victory()
    {
        Time.timeScale = 1f;
        AudioManager.Instance.PlaySFX(AudioManager.Instance.victory);
        victoryPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}