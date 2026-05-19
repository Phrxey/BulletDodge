using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    private BossShooter bossShooter;
    private SpriteRenderer spriteRenderer;

    public bool isPhaseTwo = false;

    void Start()
    {
        currentHealth = maxHealth;
        bossShooter = GetComponent<BossShooter>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log("Boss血量: " + currentHealth);

        // 血量低于50%进入第二阶段
        if (!isPhaseTwo && currentHealth <= maxHealth * 0.5f)
        {
            EnterPhaseTwo();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void EnterPhaseTwo()
    {
        isPhaseTwo = true;
        Debug.Log("进入第二阶段");
        spriteRenderer.color = new Color(1f, 0.3f, 0.3f);

        // 调用新方法而不是直接改fireRate
        bossShooter.SetPhaseTwo();
    }

    void Die()
    {
        GameManager.Instance.Victory();
        gameObject.SetActive(false);
    }
}