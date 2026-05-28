using UnityEngine;

public class Bullet : MonoBehaviour
{
    // BossShooter 通过 b.speed = xxx 直接赋值，这里的默认值只是保底
    public float speed = 3f;

    // =====================================================================
    //  直线模式
    // =====================================================================
    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        hasOrbit = false;
    }

    // =====================================================================
    //  轨道模式（BossShooter 目前未使用，保留供扩展）
    // =====================================================================
    private bool hasOrbit;
    private Transform orbitCenter;
    private float angularSpeed;
    private float radialSpeed;
    private float currentRadius;
    private float currentAngle;

    public void SetOrbit(Transform center, float angular, float radial)
    {
        orbitCenter = center;
        angularSpeed = angular;
        radialSpeed = radial;
        hasOrbit = true;

        Vector2 offset = transform.position - center.position;
        currentRadius = offset.magnitude;
        currentAngle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
    }

    // =====================================================================
    //  Update
    // =====================================================================
    void Update()
    {
        if (hasOrbit && orbitCenter != null)
            MoveOrbit();
        else
            MoveStraight();

        if (IsOutOfBounds())
            Destroy(gameObject);
    }

    void MoveStraight()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    void MoveOrbit()
    {
        currentRadius += radialSpeed * Time.deltaTime;
        currentAngle += angularSpeed * Time.deltaTime;

        float rad = currentAngle * Mathf.Deg2Rad;
        transform.position = orbitCenter.position
            + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * currentRadius;
    }

    bool IsOutOfBounds()
    {
        return Mathf.Abs(transform.position.x) > 15f
            || Mathf.Abs(transform.position.y) > 15f;
    }

    // =====================================================================
    //  碰撞
    // =====================================================================
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().TakeDamage();
            Destroy(gameObject);
        }
    }
}