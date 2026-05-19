using UnityEngine;

public class BossShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform player;
    public float fireRate = 1.5f;

    private float nextFireTime;

    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            FireAtPlayer();
            nextFireTime = Time.time + fireRate;
        }
    }

    void FireAtPlayer()
    {
        if (player == null) return;

        // 朝玩家方向发射
        Vector2 direction = (player.position - transform.position);

        GameObject b = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        b.GetComponent<Bullet>().SetDirection(direction);
    }
}