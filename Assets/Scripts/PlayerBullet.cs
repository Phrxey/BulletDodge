using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float speed = 30f;
    public float damage = 2f;
    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, Vector2.zero) > 15f)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Boss"))
        {
            other.GetComponent<BossHealth>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}