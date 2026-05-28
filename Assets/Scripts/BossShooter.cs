using System.Collections;
using UnityEngine;

public class BossShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform player;

    private bool isPhaseTwo = false;

    // =====================================================================
    //  射速参数（秒）——统一在这里调，不用翻每个协程
    // =====================================================================
    // ① 向日葵
    const float SUN_INTERVAL_P1 = 0.02f;   // P1 每颗间隔
    const float SUN_INTERVAL_P2 = 0.0133f;  // P2 每颗间隔
    const float SUN_SPEED_P1 = 3f;
    const float SUN_SPEED_P2 = 4f;

    // ② 脉冲环
    const float RING_INTERVAL_P1 = 0.92f;   // P1 每波间隔
    const float RING_INTERVAL_P2 = 0.67f;   // P2 每波间隔
    const float RING_SPEED_P1 = 2f;    // P1 最内层速度（每层 +0.4）
    const float RING_SPEED_P2 = 2f;    // P2 最内层速度（每层 +0.4）

    // ③ 钟摆双臂
    const float PEND_INTERVAL_P1 = 0.0665f; // P1 每颗间隔
    const float PEND_INTERVAL_P2 = 0.05f;  // P2 每颗间隔
    const float PEND_SWING_SPD_P1 = 1.68f;  // P1 摆速（弧度/秒）
    const float PEND_SWING_SPD_P2 = 2.16f;  // P2 摆速（弧度/秒）
    const float PEND_AMPLITUDE = 135f;    // 摆幅（度）
    const float PEND_SPEED_P1 = 3.5f;
    const float PEND_SPEED_P2 = 3.5f;

    // ④ 扇形追踪
    const float FAN_INTERVAL_P1 = 1.08f;   // P1 每波间隔
    const float FAN_INTERVAL_P2 = 0.75f;   // P2 每波间隔
    const float FAN_SPEED_P1 = 2.5f;
    const float FAN_SPEED_P2 = 3f;

    // ⑤ 圆锯弧
    const float ARC_INTERVAL = 0.117f;  // 两阶段相同（弹数不同）
    const float ARC_ROT_SPEED = 75.6f;   // 弧中心旋转速度（度/秒）
    const float ARC_SPEED_P1 = 2.0f;
    const float ARC_SPEED_P2 = 2.5f;

    // ⑥ 双螺旋
    const float SPIRAL_INTERVAL_P1 = 0.117f;
    const float SPIRAL_INTERVAL_P2 = 0.083f;
    const float SPIRAL_SPEED_0 = 1.8f;  // 第1条臂
    const float SPIRAL_SPEED_1 = 2.05f; // 第2条臂
    const float SPIRAL_SPEED_2 = 2.3f;  // 第3条臂（P2专用）

    // =====================================================================

    void Start()
    {
        StartCoroutine(ShootingLoop());
    }

    IEnumerator ShootingLoop()
    {
        // 每轮把6种技能随机打乱后依次播放，同一轮内不重复
        var patterns = new System.Func<IEnumerator>[]
        {
            Sunflower,
            PulseRing,
            PendulumArms,
            FanTrack,
            RotatingArc,
            DualSpiral,
        };

        while (true)
        {
            Shuffle(patterns);
            foreach (var pattern in patterns)
            {
                yield return StartCoroutine(pattern());
                yield return new WaitForSeconds(0.8f);
            }
        }
    }

    // Fisher-Yates 洗牌
    void Shuffle<T>(T[] arr)
    {
        for (int i = arr.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
    }

    // =====================================================================
    //  工具函数
    // =====================================================================
    void Fire(float angleDeg, float speed, Color? color = null)
    {
        float rad = angleDeg * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        GameObject go = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        if (color.HasValue)
            go.GetComponent<SpriteRenderer>().color = color.Value;
        Bullet b = go.GetComponent<Bullet>();
        b.SetDirection(dir);
        b.speed = speed;
    }

    // =====================================================================
    //  ① 向日葵螺旋
    //  每颗子弹方向按黄金角（137.5°）累积，密度均匀无规律
    // =====================================================================
    IEnumerator Sunflower()
    {
        float duration = isPhaseTwo ? 5f : 6f;
        float interval = isPhaseTwo ? SUN_INTERVAL_P2 : SUN_INTERVAL_P1;
        float bulletSpeed = isPhaseTwo ? SUN_SPEED_P2 : SUN_SPEED_P1;
        Color color = Color.yellow;

        const float goldenAngle = 137.50776f;
        float baseAngle = 0f;
        float elapsed = 0f;
        float timer = 0f;

        while (elapsed < duration)
        {
            float dt = Time.deltaTime;
            elapsed += dt;
            timer += dt;
            while (timer >= interval && elapsed <= duration)
            {
                timer -= interval;
                Fire(baseAngle, bulletSpeed, color);
                baseAngle += goldenAngle;
            }
            yield return null;
        }
    }

    // =====================================================================
    //  ② 脉冲环
    //  每波同时爆发多层整圆，整体旋转 15°，无持久安全位
    // =====================================================================
    IEnumerator PulseRing()
    {
        float duration = isPhaseTwo ? 8f : 10f;
        float interval = isPhaseTwo ? RING_INTERVAL_P2 : RING_INTERVAL_P1;
        int layers = isPhaseTwo ? 3 : 2;
        int n = isPhaseTwo ? 26 : 20;
        float rotOffset = 15f; // 每波旋转角度

        Color[] colors = { Color.cyan, new Color(0.75f, 0.38f, 1f), new Color(1f, 0.56f, 0.19f) };

        float elapsed = 0f;
        float timer = interval; // 第一波立即触发
        int waveIndex = 0;

        while (elapsed < duration)
        {
            float dt = Time.deltaTime;
            elapsed += dt;
            timer += dt;
            if (timer >= interval && elapsed <= duration)
            {
                timer -= interval;
                float baseOff = waveIndex * rotOffset;
                for (int r = 0; r < layers; r++)
                {
                    float layerOff = baseOff + r * (180f / n);
                    float baseSpd = isPhaseTwo ? RING_SPEED_P2 : RING_SPEED_P1;
                    float speed = baseSpd + r * 0.4f;
                    for (int i = 0; i < n; i++)
                    {
                        float angle = layerOff + i * (360f / n);
                        Fire(angle, speed, colors[r]);
                    }
                }
                waveIndex++;
            }
            yield return null;
        }
    }

    // =====================================================================
    //  ③ 钟摆双臂
    //  方向做匀速正弦摆动，P2 增至四臂
    // =====================================================================
    IEnumerator PendulumArms()
    {
        float duration = isPhaseTwo ? 6f : 7f;
        float interval = isPhaseTwo ? PEND_INTERVAL_P2 : PEND_INTERVAL_P1;
        float swingSpeed = isPhaseTwo ? PEND_SWING_SPD_P2 : PEND_SWING_SPD_P1;
        int arms = isPhaseTwo ? 4 : 2;
        float bulletSpd = isPhaseTwo ? PEND_SPEED_P2 : PEND_SPEED_P1;
        Color color = new Color(0.25f, 0.88f, 0.63f);

        float elapsed = 0f;
        float timer = 0f;

        while (elapsed < duration)
        {
            float dt = Time.deltaTime;
            elapsed += dt;
            timer += dt;
            while (timer >= interval && elapsed <= duration)
            {
                timer -= interval;
                float swing = Mathf.Sin(elapsed * swingSpeed) * PEND_AMPLITUDE;
                for (int k = 0; k < arms; k++)
                {
                    float angle = swing + k * (360f / arms);
                    Fire(angle, bulletSpd, color);
                }
            }
            yield return null;
        }
    }

    // =====================================================================
    //  ④ 扇形追踪
    //  每波对准玩家当前位置发扇形，P2 子弹更多、间隔更短
    // =====================================================================
    IEnumerator FanTrack()
    {
        float duration = isPhaseTwo ? 7f : 8f;
        float interval = isPhaseTwo ? FAN_INTERVAL_P2 : FAN_INTERVAL_P1;
        int n = isPhaseTwo ? 11 : 7;
        float spreadDeg = isPhaseTwo ? 48f : 36f;
        float bulletSpd = isPhaseTwo ? FAN_SPEED_P2 : FAN_SPEED_P1;
        Color color = new Color(1f, 0.5f, 0.38f);

        float elapsed = 0f;
        float timer = interval; // 立即触发第一波

        while (elapsed < duration)
        {
            float dt = Time.deltaTime;
            elapsed += dt;
            timer += dt;
            if (timer >= interval && elapsed <= duration)
            {
                timer -= interval;
                if (player == null) yield break;
                Vector2 toPlayer = (player.position - transform.position).normalized;
                float baseDeg = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;
                float step = n > 1 ? spreadDeg / (n - 1) : 0f;
                float startAngle = baseDeg - spreadDeg / 2f;
                for (int i = 0; i < n; i++)
                    Fire(startAngle + i * step, bulletSpd, color);
            }
            yield return null;
        }
    }

    // =====================================================================
    //  ⑤ 圆锯弧
    //  只发射 120° 弧面，弧中心持续旋转；P2 增加反向弧
    // =====================================================================
    IEnumerator RotatingArc()
    {
        float duration = isPhaseTwo ? 6f : 7f;
        float interval = ARC_INTERVAL;
        int n = isPhaseTwo ? 8 : 6;
        float arcHalf = 60f; // ±60° = 120° 弧
        Color colorCW = new Color(1f, 0.69f, 0.25f);
        Color colorCCW = new Color(1f, 0.25f, 0.56f);
        float bulletSpd = isPhaseTwo ? ARC_SPEED_P2 : ARC_SPEED_P1;

        float arcAngle = 0f;
        float elapsed = 0f;
        float timer = 0f;

        while (elapsed < duration)
        {
            float dt = Time.deltaTime;
            elapsed += dt;
            timer += dt;
            arcAngle += ARC_ROT_SPEED * dt; // 顺时针旋转

            while (timer >= interval && elapsed <= duration)
            {
                timer -= interval;
                // 顺时针弧
                for (int i = 0; i < n; i++)
                {
                    float angle = arcAngle - arcHalf + i * (2f * arcHalf / (n - 1));
                    Fire(angle, bulletSpd, colorCW);
                }
                // P2：反向弧（中心角 +180°，逆时针旋转）
                if (isPhaseTwo)
                {
                    float ccwAngle = -arcAngle + 180f;
                    for (int i = 0; i < n; i++)
                    {
                        float angle = ccwAngle - arcHalf + i * (2f * arcHalf / (n - 1));
                        Fire(angle, bulletSpd, colorCCW);
                    }
                }
            }
            yield return null;
        }
    }

    // =====================================================================
    //  ⑥ 双螺旋
    //  两条（P2 三条）臂以不同角速度旋转，轨迹交叉编织
    // =====================================================================
    IEnumerator DualSpiral()
    {
        float duration = isPhaseTwo ? 7f : 8f;
        float interval = isPhaseTwo ? SPIRAL_INTERVAL_P2 : SPIRAL_INTERVAL_P1;
        int arms = isPhaseTwo ? 3 : 2;

        // 各臂角速度（度/秒），比值约 1 : 1.61 : 2.44
        float[] rotSpeeds = { 123.7f, 199.1f, 302.0f };
        float[] speeds = { SPIRAL_SPEED_0, SPIRAL_SPEED_1, SPIRAL_SPEED_2 };
        Color[] colors =
        {
            new Color(0.38f, 0.69f, 1f),
            new Color(1f,    0.38f, 0.56f),
            new Color(0.63f, 1f,    0.38f)
        };

        float[] armAngles = new float[3];
        float elapsed = 0f;
        float timer = 0f;

        while (elapsed < duration)
        {
            float dt = Time.deltaTime;
            elapsed += dt;
            timer += dt;

            for (int s = 0; s < arms; s++)
                armAngles[s] += rotSpeeds[s] * dt;

            while (timer >= interval && elapsed <= duration)
            {
                timer -= interval;
                for (int s = 0; s < arms; s++)
                {
                    // 每条臂两颗对称
                    Fire(armAngles[s], speeds[s], colors[s]);
                    Fire(armAngles[s] + 180f, speeds[s], colors[s]);
                }
            }
            yield return null;
        }
    }

    // =====================================================================
    //  阶段切换（由外部 HP 逻辑调用）
    // =====================================================================
    public void SetPhaseTwo()
    {
        isPhaseTwo = true;
    }
}