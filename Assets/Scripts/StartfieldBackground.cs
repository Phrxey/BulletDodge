using UnityEngine;

public class StarfieldBackground : MonoBehaviour
{
    [Header("Star")]
    public int starCount = 200;
    public float scrollSpeed = 1f;
    public float minSize = 0.02f;
    public float maxSize = 0.08f;

    [Header("Meteor")]
    public Sprite[] meteorSprites; // 拖入陨石图片
    public int meteorCount = 8;
    public float meteorMinSpeed = 0.5f;
    public float meteorMaxSpeed = 1.5f;
    public float meteorMinSize = 0.3f;
    public float meteorMaxSize = 0.8f;

    private Transform[] stars;
    private Transform[] meteors;
    private float[] meteorSpeeds;

    private float topY;
    private float bottomY;
    private float leftX;
    private float rightX;

    void Start()
    {
        Camera cam = Camera.main;
        topY = cam.orthographicSize;
        bottomY = -cam.orthographicSize;
        leftX = -cam.orthographicSize * cam.aspect;
        rightX = cam.orthographicSize * cam.aspect;

        SpawnStars();

        if (meteorSprites != null && meteorSprites.Length > 0)
            SpawnMeteors();
    }

    void SpawnStars()
    {
        stars = new Transform[starCount];
        for (int i = 0; i < starCount; i++)
        {
            GameObject star = new GameObject("Star");
            star.transform.parent = transform;

            SpriteRenderer sr = star.AddComponent<SpriteRenderer>();
            sr.sprite = CreateCircleSprite();
            sr.sortingOrder = -10;

            float size = Random.Range(minSize, maxSize);
            star.transform.localScale = new Vector3(size, size, 1);

            float brightness = Random.Range(0.4f, 1f);
            sr.color = new Color(brightness, brightness, brightness, 1f);

            star.transform.position = new Vector3(
                Random.Range(leftX, rightX),
                Random.Range(bottomY, topY),
                1f
            );

            stars[i] = star.transform;
        }
    }

    void SpawnMeteors()
    {
        meteors = new Transform[meteorCount];
        meteorSpeeds = new float[meteorCount];

        for (int i = 0; i < meteorCount; i++)
        {
            GameObject meteor = new GameObject("Meteor");
            meteor.transform.parent = transform;

            SpriteRenderer sr = meteor.AddComponent<SpriteRenderer>();
            // 随机选一个陨石图片
            sr.sprite = meteorSprites[Random.Range(0, meteorSprites.Length)];
            sr.sortingOrder = -9;

            // 随机大小
            float size = Random.Range(meteorMinSize, meteorMaxSize);
            meteor.transform.localScale = new Vector3(size, size, 1);

            // 随机透明度，远处的陨石更透明
            Color c = sr.color;
            c.a = Random.Range(0.3f, 0.7f);
            sr.color = c;

            // 随机位置
            meteor.transform.position = new Vector3(
                Random.Range(leftX, rightX),
                Random.Range(bottomY, topY),
                1f
            );

            // 随机旋转
            meteor.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

            meteorSpeeds[i] = Random.Range(meteorMinSpeed, meteorMaxSpeed);
            meteors[i] = meteor.transform;
        }
    }

    void Update()
    {
        // 星星滚动
        foreach (Transform star in stars)
        {
            star.position += Vector3.down * scrollSpeed * Time.deltaTime;
            if (star.position.y < bottomY)
            {
                star.position = new Vector3(
                    Random.Range(leftX, rightX),
                    topY,
                    star.position.z
                );
            }
        }

        // 陨石滚动（速度各不同，有视差感）
        if (meteors == null) return;
        for (int i = 0; i < meteors.Length; i++)
        {
            meteors[i].position += Vector3.down * meteorSpeeds[i] * Time.deltaTime;

            if (meteors[i].position.y < bottomY)
            {
                meteors[i].position = new Vector3(
                    Random.Range(leftX, rightX),
                    topY,
                    meteors[i].position.z
                );
            }
        }
    }

    Sprite CreateCircleSprite()
    {
        int size = 16;
        Texture2D tex = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f;

        for (int i = 0; i < pixels.Length; i++)
        {
            int x = i % size;
            int y = i / size;
            float dist = Vector2.Distance(new Vector2(x, y), center);
            pixels[i] = dist <= radius ? Color.white : Color.clear;
        }

        tex.SetPixels(pixels);
        tex.Apply();

        return Sprite.Create(tex,
            new Rect(0, 0, size, size),
            new Vector2(0.5f, 0.5f));
    }
}