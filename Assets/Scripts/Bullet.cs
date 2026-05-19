using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 4f;
    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);

        // 飞出屏幕就销毁（后面会换成object pool）
        if (Vector2.Distance(transform.position, Vector2.zero) > 15f)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("玩家被击中");
            Destroy(gameObject);
        }
    }
}