using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject gameOverPanel;
    public GameObject victoryPanel;

    private bool isGameOver = false;

    void Awake()
    {
        Instance = this;
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        Time.timeScale = 1f;

        // 停止BGM
        AudioManager.Instance.StopBGM();
        AudioManager.Instance.StopSlowTime();

        // 播放失败音效
        AudioManager.Instance.PlaySFX(AudioManager.Instance.gameOver);

        // 禁用玩家输入和子弹
        DisableAllBullets();
        DisablePlayer();

        gameOverPanel.SetActive(true);
    }

    public void Victory()
    {
        if (isGameOver) return;
        isGameOver = true;

        Time.timeScale = 1f;

        // 停止BGM
        AudioManager.Instance.StopBGM();
        AudioManager.Instance.StopSlowTime();

        // 播放胜利音效
        AudioManager.Instance.PlaySFX(AudioManager.Instance.victory);

        // 禁用子弹
        DisableAllBullets();
        DisablePlayer();

        victoryPanel.SetActive(true);
    }

    public void RestartGame()
    {
        isGameOver = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void DisableAllBullets()
    {
        // 找到所有子弹，停止它们的移动
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bullets)
        {
            Bullet b = bullet.GetComponent<Bullet>();
            if (b != null) b.enabled = false;

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null) rb.simulated = false;
        }

        // 玩家子弹也一起停
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

        // 禁用移动和射击脚本
        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = false;

        PlayerShooter ps = player.GetComponent<PlayerShooter>();
        if (ps != null) ps.enabled = false;
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;

        // 编辑器里停止Play，打包后退出程序
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}