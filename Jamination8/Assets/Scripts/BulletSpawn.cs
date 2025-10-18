using UnityEngine;

public class BulletSpawn : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnInterval = 3.0f;
    private float timer = 0.0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = spawnInterval;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0.0f)
        {
            SpawnBullet();
            timer = spawnInterval;
        }
    }

    private void SpawnBullet()
    {
        Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);

    }
}
