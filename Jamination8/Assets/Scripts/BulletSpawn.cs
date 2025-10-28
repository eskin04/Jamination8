using System.Collections;
using UnityEngine;

public class BulletSpawn : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnInterval = 3.0f;
    [SerializeField] private ParticleSystem smokeEffect;
    private float timer = 1.0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = spawnInterval;
        StartCoroutine(IncreasSpawnRateOverTime());
    }

    private IEnumerator IncreasSpawnRateOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(15f); // 15 saniyede bir spawn hızı artışı
            if (spawnInterval > 1.0f) // Minimum interval sınırı
            {
                spawnInterval -= 0.5f; // Spawn hızını artır (intervali azalt)
            }
            else
            {
                break; // Minimum intervale ulaşıldıysa döngüyü kır
            }
        }
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        if (MonkeyManager.Instance.isGameOver) return;
        timer -= Time.deltaTime;
        if (timer <= 0.0f)
        {
            SpawnBullet();
            timer = spawnInterval;
        }
    }

    private void SpawnBullet()
    {
        smokeEffect.Play();
        Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);

    }
}
