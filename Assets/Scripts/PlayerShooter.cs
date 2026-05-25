using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public GameObject playerBulletPrefab;
    public float fireRate = 0.001f;
    private float nextFireTime;

    void Update()
    {
        if (Input.GetKey(KeyCode.Z) && Time.unscaledTime >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.unscaledTime + fireRate;
        }
    }

    void Shoot()
    {
        GameObject b = Instantiate(playerBulletPrefab,
                                   transform.position,
                                   Quaternion.identity);
        b.GetComponent<PlayerBullet>().SetDirection(Vector2.up);

        AudioManager.Instance.PlaySFX(AudioManager.Instance.playerShoot);
    }
}