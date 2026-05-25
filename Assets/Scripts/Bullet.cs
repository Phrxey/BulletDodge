using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 4f;
    private Vector2 direction;

    // 旋转运动参数
    private bool hasOrbit = false;
    private Transform orbitCenter;
    private float angularSpeed = 0f;  // 度/秒
    private float radialSpeed = 0f;   // 向外扩散速度
    private float currentRadius = 0f; // 当前半径

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        speed *= multiplier;
    }

    // 设置轨道运动
    public void SetOrbit(Transform center, float angular, float radial)
    {
        orbitCenter = center;
        angularSpeed = angular;
        radialSpeed = radial;
        hasOrbit = true;
        currentRadius = Vector2.Distance(transform.position, center.position);
    }

    void Update()
    {
        if (hasOrbit && orbitCenter != null)
        {
            // 半径持续增加（向外扩散）
            currentRadius += radialSpeed * Time.deltaTime;

            // 当前角度
            Vector2 offset = transform.position - orbitCenter.position;
            float currentAngle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;

            // 角度增加（旋转）
            currentAngle += angularSpeed * Time.deltaTime;

            // 根据新角度和新半径计算位置
            float rad = currentAngle * Mathf.Deg2Rad;
            transform.position = orbitCenter.position +
                new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * currentRadius;
        }
        else
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }

        if (Mathf.Abs(transform.position.x) > 15f ||
            Mathf.Abs(transform.position.y) > 15f)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().TakeDamage();
            Destroy(gameObject);
        }
    }
}