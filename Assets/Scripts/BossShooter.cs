using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform player;

    // 弹幕参数
    public int bulletCount = 48;
    public float angularSpeed = 15f;   // 旋转速度，度/秒
    public float radialSpeed = 4f;   // 向外扩散速度

    // 第二阶段
    private bool isPhaseTwo = false;

    void Start()
    {
        StartCoroutine(ShootingLoop());
    }

    IEnumerator ShootingLoop()
    {
        while (true)
        {
            yield return StartCoroutine(OrbitBurst());
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator OrbitBurst()
    {
        int count = isPhaseTwo ? bulletCount * 2 : bulletCount;
        float angular = isPhaseTwo ? angularSpeed * 1.2f : angularSpeed;
        float radial = isPhaseTwo ? radialSpeed * 1.2f : radialSpeed;

        // 瞬间生成一整圈
        for (int i = 0; i < count; i++)
        {
            float angle = (360f / count) * i;
            float rad = angle * Mathf.Deg2Rad;

            // 从Boss位置稍微偏移生成，形成初始圆圈
            Vector2 offset = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * 0.5f;
            Vector3 spawnPos = transform.position + new Vector3(offset.x, offset.y, 0);

            GameObject b = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

            // 颜色随角度变化，彩虹效果
            Color c = Color.HSVToRGB(angle / 360f, 1f, 1f);
            b.GetComponent<SpriteRenderer>().color = c;

            Bullet bullet = b.GetComponent<Bullet>();
            bullet.SetOrbit(transform, angular, radial);
        }

        yield return null;
    }

    public void SetPhaseTwo()
    {
        isPhaseTwo = true;
    }
}