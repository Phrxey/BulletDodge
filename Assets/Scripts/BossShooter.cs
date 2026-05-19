using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform player;
    public float fireRate = 3f;

    private float spiralAngle = 0f;
    private int lastPattern = -1;
    private List<System.Func<IEnumerator>> patterns;

    void Start()
    {
        patterns = new List<System.Func<IEnumerator>>
        {
            SpiralShot,          // 单螺旋
            DoubleSpiral,        // 双螺旋
            RingBurst,           // 同心圆爆炸
            Windmill,            // 风车
            SineWave,            // 正弦波
            Cardioid,            // 心形线
            FanSweep,            // 扇形扫射
            QuadraticSpray,      // 二次曲线扩散
            StarBurst,           // 星形爆发
            PolarRose,           // 极坐标玫瑰线
        };

        StartCoroutine(ShootingLoop());
    }

    IEnumerator ShootingLoop()
    {
        while (true)
        {
            int next;
            do { next = Random.Range(0, patterns.Count); }
            while (next == lastPattern && patterns.Count > 1);
            lastPattern = next;

            yield return StartCoroutine(patterns[next]());
            yield return new WaitForSeconds(1f);
        }
    }

    // ── 模式 0：单螺旋 ──────────────────────────────
    IEnumerator SpiralShot()
    {
        for (int i = 0; i < 80; i++)
        {
            float rad = spiralAngle * Mathf.Deg2Rad;
            SpawnBullet(new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)), Color.cyan);
            spiralAngle += 13f;
            yield return new WaitForSeconds(0.05f);
        }
    }

    // ── 模式 1：双螺旋（顺逆反向） ─────────────────
    IEnumerator DoubleSpiral()
    {
        float a = 0f, b = 180f;
        for (int i = 0; i < 60; i++)
        {
            SpawnBullet(AngleToDir(a), Color.blue);
            SpawnBullet(AngleToDir(b), Color.red);
            a += 11f; b += 11f;
            yield return new WaitForSeconds(0.06f);
        }
    }

    // ── 模式 2：同心圆连续爆炸 ─────────────────────
    IEnumerator RingBurst()
    {
        // 每波子弹数递增，形成层次感
        int[] counts = { 8, 12, 16, 20 };
        foreach (int count in counts)
        {
            for (int i = 0; i < count; i++)
            {
                float angle = (360f / count) * i;
                Color c = Color.Lerp(Color.yellow, Color.red, (float)i / count);
                SpawnBullet(AngleToDir(angle), c);
            }
            yield return new WaitForSeconds(0.35f);
        }
    }

    // ── 模式 3：风车 ────────────────────────────────
    // 4条臂同时旋转，每条臂连续发射形成叶片
    IEnumerator Windmill()
    {
        float baseAngle = 0f;
        int arms = 4;
        for (int i = 0; i < 60; i++)
        {
            for (int arm = 0; arm < arms; arm++)
            {
                float angle = baseAngle + (360f / arms) * arm;
                Color c = arm % 2 == 0
                    ? new Color(1f, 0.8f, 0f)   // 金色
                    : new Color(1f, 0.3f, 0.8f); // 粉色
                SpawnBullet(AngleToDir(angle), c);
            }
            baseAngle += 7f;
            yield return new WaitForSeconds(0.05f);
        }
    }

    // ── 模式 4：正弦波扫射 ──────────────────────────
    // 基础方向朝下，横向偏移量用sin函数控制，形成波浪
    IEnumerator SineWave()
    {
        float t = 0f;
        for (int i = 0; i < 60; i++)
        {
            // sin控制左右偏移
            float sideOffset = Mathf.Sin(t * 3f) * 45f;
            // 同时发三颗，形成宽度
            for (int j = -1; j <= 1; j++)
            {
                float angle = 270f + sideOffset + j * 12f;
                Color c = Color.Lerp(Color.green, Color.yellow,
                    (Mathf.Sin(t * 3f) + 1f) / 2f);
                SpawnBullet(AngleToDir(angle), c);
            }
            t += 0.1f;
            yield return new WaitForSeconds(0.06f);
        }
    }

    // ── 模式 5：心形线 ──────────────────────────────
    // 用极坐标心形公式 r = 1 - sin(θ) 决定每颗子弹的方向偏移量
    IEnumerator Cardioid()
    {
        int count = 36;
        // 发射3波，每波旋转30度
        for (int wave = 0; wave < 3; wave++)
        {
            for (int i = 0; i < count; i++)
            {
                float theta = (360f / count) * i;
                float thetaRad = theta * Mathf.Deg2Rad;

                // 心形线：r = 1 - sin(θ)
                // r决定速度倍率，让不同方向的子弹速度不同，形成心形轮廓
                float r = 1f - Mathf.Sin(thetaRad);
                float speedMult = Mathf.Clamp(r * 0.5f + 0.3f, 0.2f, 1.2f);

                float finalAngle = theta + wave * 30f;
                SpawnBullet(AngleToDir(finalAngle),
                    new Color(1f, 0.4f, 0.6f), speedMult); // 玫红色
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    // ── 模式 6：扇形追踪扫射 ────────────────────────
    IEnumerator FanSweep()
    {
        for (int burst = 0; burst < 6; burst++)
        {
            if (player == null) yield break;
            float baseAngle = Mathf.Atan2(
                player.position.y - transform.position.y,
                player.position.x - transform.position.x) * Mathf.Rad2Deg;

            int count = 9;
            float spread = 40f;
            for (int i = 0; i < count; i++)
            {
                float offset = Mathf.Lerp(-spread, spread, (float)i / (count - 1));
                Color c = Color.Lerp(Color.magenta, Color.white,
                    (float)i / count);
                SpawnBullet(AngleToDir(baseAngle + offset), c);
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    // ── 模式 7：二次曲线扩散 ────────────────────────
    // 子弹速度按二次函数分布：中间慢、两边快，或反过来
    // 形成类似抛物线展开的视觉效果
    IEnumerator QuadraticSpray()
    {
        for (int burst = 0; burst < 5; burst++)
        {
            int count = 15;
            for (int i = 0; i < count; i++)
            {
                // 角度线性分布
                float angle = Mathf.Lerp(200f, 340f, (float)i / (count - 1));

                // 速度按二次函数：两端快，中间慢
                float t = (float)i / (count - 1); // 0到1
                float speedMult = 0.4f + 1.2f * (2f * t - 1f) * (2f * t - 1f);

                Color c = Color.Lerp(
                    new Color(0f, 1f, 0.5f),  // 青绿
                    new Color(1f, 1f, 0f),     // 黄
                    t);
                SpawnBullet(AngleToDir(angle), c, speedMult);
            }
            yield return new WaitForSeconds(0.35f);
        }
    }

    // ── 模式 8：星形爆发 ────────────────────────────
    // 5个方向同时爆发，每个方向3颗密集子弹，形成星形
    IEnumerator StarBurst()
    {
        for (int pulse = 0; pulse < 5; pulse++)
        {
            int points = 5; // 五角星
            float rotOffset = pulse * 15f; // 每次旋转一点

            for (int p = 0; p < points; p++)
            {
                float centerAngle = (360f / points) * p + rotOffset;

                // 每个方向发3颗，中间最快，两边慢
                float[] offsets = { -8f, 0f, 8f };
                float[] speeds = { 0.6f, 1.2f, 0.6f };
                for (int k = 0; k < 3; k++)
                {
                    Color c = Color.Lerp(Color.yellow, Color.white,
                        (float)p / points);
                    SpawnBullet(AngleToDir(centerAngle + offsets[k]),
                        c, speeds[k]);
                }
            }
            yield return new WaitForSeconds(0.4f);
        }
    }

    // ── 模式 9：极坐标玫瑰线 ────────────────────────
    // r = cos(k * θ)，k=3时是三叶玫瑰，k=4时是四叶
    // 用速度大小来体现r值，不同方向子弹快慢不同，形成花瓣轮廓
    IEnumerator PolarRose()
    {
        int k = Random.Range(2, 5); // 随机2~4叶
        int count = 72;             // 一圈72颗

        // 发射两波，第二波旋转45度
        for (int wave = 0; wave < 2; wave++)
        {
            for (int i = 0; i < count; i++)
            {
                float theta = (360f / count) * i;
                float thetaRad = theta * Mathf.Deg2Rad;

                // 玫瑰线公式
                float r = Mathf.Abs(Mathf.Cos(k * thetaRad));
                float speedMult = Mathf.Clamp(r * 1.2f + 0.15f, 0.15f, 1.3f);

                float finalAngle = theta + wave * 45f;

                // 颜色随角度变化，形成彩虹效果
                Color c = Color.HSVToRGB(theta / 360f, 1f, 1f);
                SpawnBullet(AngleToDir(finalAngle), c, speedMult);
            }
            yield return new WaitForSeconds(0.6f);
        }
    }

    // ── 工具方法 ────────────────────────────────────

    Vector2 AngleToDir(float angleDeg)
    {
        float rad = angleDeg * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    void SpawnBullet(Vector2 direction, Color color, float speedMultiplier = 1f)
    {
        GameObject b = Instantiate(bulletPrefab, transform.position,
                                   Quaternion.identity);
        SpriteRenderer sr = b.GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = color;

        Bullet bullet = b.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.SetDirection(direction);
            bullet.SetSpeedMultiplier(speedMultiplier);
        }
    }

    public void SetPhaseTwo()
    {
        // 第二阶段：在现有基础上额外加一条同步螺旋
        fireRate = 0.8f;
        StartCoroutine(PhaseToBonus());
    }

    // 第二阶段额外效果：持续背景螺旋压迫感
    IEnumerator PhaseToBonus()
    {
        while (true)
        {
            float rad = spiralAngle * Mathf.Deg2Rad;
            SpawnBullet(new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)),
                new Color(1f, 0.2f, 0.2f), 0.7f);
            spiralAngle += 17f;
            yield return new WaitForSeconds(0.08f);
        }
    }
}