using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float slowTimeScale = 0.3f;
    public float slowDuration = 6f;

    private float currentSlowTime;
    public Slider slowTimeBar;

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
        if (Input.GetKeyDown(KeyCode.LeftShift) && currentSlowTime > 0)
        {
            AudioManager.Instance.StartSlowTime();
            FindObjectOfType<SlowTimeEffect>().TriggerRipple();
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) || currentSlowTime <= 0)
        {
            AudioManager.Instance.StopSlowTime();
        }

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

        if (slowTimeBar != null)
            slowTimeBar.value = currentSlowTime / slowDuration;
    }

    public bool IsSlowed()
    {
        return Input.GetKey(KeyCode.LeftShift) && currentSlowTime > 0;
    }

    void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}