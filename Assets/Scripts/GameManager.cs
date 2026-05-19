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
        Time.timeScale = 1f; // 重置时间，防止子弹时间卡住
        gameOverPanel.SetActive(true);
    }

    public void Victory()
    {
        Time.timeScale = 1f;
        victoryPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}