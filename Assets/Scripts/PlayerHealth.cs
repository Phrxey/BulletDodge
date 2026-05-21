using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxLives = 3;
    private int currentLives;

    public float invincibleDuration = 2f;
    private bool isInvincible = false;

    private SpriteRenderer spriteRenderer;
    public UIManager uiManager;

    void Start()
    {
        currentLives = maxLives;
        spriteRenderer = GetComponent<SpriteRenderer>();
        uiManager.UpdateHearts(currentLives);
    }

    public void TakeDamage()
    {
        if (isInvincible) return;

        //currentLives--;
        uiManager.UpdateHearts(currentLives);

        // 玩家被击中音效
        AudioManager.Instance.PlaySFX(AudioManager.Instance.playerHit);

        if (currentLives <= 0)
            Die();
        else
            StartCoroutine(InvincibleCoroutine());
    }

    IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;
        float elapsed = 0f;
        while (elapsed < invincibleDuration)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.3f);
            yield return new WaitForSecondsRealtime(0.1f);
            spriteRenderer.color = new Color(1, 1, 1, 1f);
            yield return new WaitForSecondsRealtime(0.1f);
            elapsed += 0.2f;
        }
        spriteRenderer.color = new Color(1, 1, 1, 1f);
        isInvincible = false;
    }

    void Die()
    {
        Time.timeScale = 1f;
        AudioManager.Instance.PlaySFX(AudioManager.Instance.gameOver);
        GameManager.Instance.GameOver();
        gameObject.SetActive(false);
    }
}