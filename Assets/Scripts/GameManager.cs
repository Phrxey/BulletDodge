using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject gameOverPanel;
    public GameObject victoryPanel;
    public GameObject pausePanel;

    public bool isGameOver = false;
    private bool isPaused = false;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        // 游戏结束时不能暂停
        if (isGameOver) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
        AudioManager.Instance.StopBGM();

        // 暂停时禁用玩家
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerMovement pm = player.GetComponent<PlayerMovement>();
            if (pm != null) pm.enabled = false;
            PlayerShooter ps = player.GetComponent<PlayerShooter>();
            if (ps != null) ps.enabled = false;
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        AudioManager.Instance.PlayBGM();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerMovement pm = player.GetComponent<PlayerMovement>();
            if (pm != null) pm.enabled = true;
            PlayerShooter ps = player.GetComponent<PlayerShooter>();
            if (ps != null) ps.enabled = true;
        }
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 1f;
        AudioManager.Instance.StopBGM();
        AudioManager.Instance.StopSlowTime();
        AudioManager.Instance.PlaySFX(AudioManager.Instance.gameOver);
        DisableAllBullets();
        DisablePlayer();
        gameOverPanel.SetActive(true);
    }

    public void Victory()
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 1f;
        AudioManager.Instance.StopBGM();
        AudioManager.Instance.StopSlowTime();
        AudioManager.Instance.PlaySFX(AudioManager.Instance.victory);
        DisableAllBullets();
        DisablePlayer();
        victoryPanel.SetActive(true);
    }

    public void RestartGame()
    {
        isGameOver = false;
        isPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void DisableAllBullets()
    {
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bullets)
        {
            Bullet b = bullet.GetComponent<Bullet>();
            if (b != null) b.enabled = false;
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null) rb.simulated = false;
        }

        GameObject[] playerBullets = GameObject.FindGameObjectsWithTag("PlayerBullet");
        foreach (GameObject bullet in playerBullets)
        {
            PlayerBullet b = bullet.GetComponent<PlayerBullet>();
            if (b != null) b.enabled = false;
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null) rb.simulated = false;
        }
    }

    void DisablePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;
        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = false;
        PlayerShooter ps = player.GetComponent<PlayerShooter>();
        if (ps != null) ps.enabled = false;
    }
}