using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float slowTimeScale = 0.3f;
    public float slowDuration = 3f;      // 最多能慢几秒

    private float currentSlowTime;       // 剩余可用时间
    private bool isSlowed = false;

    void Start()
    {
        currentSlowTime = slowDuration;
        Time.timeScale = 1f;
    }

    void Update()
    {
        HandleSlowTime();
        HandleMovement();
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector2 move = new Vector2(moveX, moveY).normalized;

        // 用unscaledDeltaTime，玩家移动不受子弹时间影响
        transform.Translate(move * speed * Time.unscaledDeltaTime);
    }

    void HandleSlowTime()
    {
        // 按住Shift且还有剩余时间
        if (Input.GetKey(KeyCode.LeftShift) && currentSlowTime > 0)
        {
            Time.timeScale = slowTimeScale;
            currentSlowTime -= Time.unscaledDeltaTime;
            isSlowed = true;
        }
        else
        {
            Time.timeScale = 1f;
            isSlowed = false;

            // 松手后慢慢回复（回复速度是消耗速度的一半）
            if (currentSlowTime < slowDuration)
                currentSlowTime += Time.unscaledDeltaTime * 0.5f;

            currentSlowTime = Mathf.Clamp(currentSlowTime, 0, slowDuration);
        }
    }

    // 游戏结束时记得重置timeScale
    void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}