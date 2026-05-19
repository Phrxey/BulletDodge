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

        // Boss变红提示玩家
        spriteRenderer.color = new Color(1f, 0.3f, 0.3f);

        // 射速加快
        bossShooter.fireRate = 0.8f;
    }

    void Die()
    {
        Debug.Log("Boss死亡，玩家胜利");
        // 现在先只打印，之后加胜利界面
        gameObject.SetActive(false);
    }
}