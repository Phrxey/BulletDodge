using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float slowTimeScale = 0.3f;
    public float slowDuration = 3f;

    private float currentSlowTime;
    public Slider slowTimeBar; // 能量槽

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
        transform.Translate(move * speed * Time.unscaledDeltaTime);
    }

    void HandleSlowTime()
    {
        if (Input.GetKey(KeyCode.LeftShift) && currentSlowTime > 0)
        {
            Time.timeScale = slowTimeScale;
            currentSlowTime -= Time.unscaledDeltaTime;
        }
        else
        {
            Time.timeScale = 1f;

            if (currentSlowTime < slowDuration)
                currentSlowTime += Time.unscaledDeltaTime * 0.5f;

            currentSlowTime = Mathf.Clamp(currentSlowTime, 0, slowDuration);
        }

        // 更新UI，把剩余时间转换成0~1的比例
        if (slowTimeBar != null)
            slowTimeBar.value = currentSlowTime / slowDuration;
    }

    void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}