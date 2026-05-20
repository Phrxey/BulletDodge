using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    private BossShooter bossShooter;
    private SpriteRenderer spriteRenderer;
    public bool isPhaseTwo = false;

    public Slider healthBar; // 新增

    void Start()
    {
        currentHealth = maxHealth;
        bossShooter = GetComponent<BossShooter>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = maxHealth;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        // 更新血量条
        if (healthBar != null)
            healthBar.value = currentHealth;

        if (!isPhaseTwo && currentHealth <= maxHealth * 0.5f)
            EnterPhaseTwo();

        if (currentHealth <= 0)
            Die();
    }

    void EnterPhaseTwo()
    {
        isPhaseTwo = true;
        spriteRenderer.color = new Color(1f, 0.3f, 0.3f);
        bossShooter.SetPhaseTwo();
    }

    void Die()
    {
        if (healthBar != null)
            healthBar.value = 0;
        GameManager.Instance.Victory();
        gameObject.SetActive(false);
    }
}