using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GuidedBomb : MonoBehaviour
{
    private GameObject[] targets;
    private float speed = 10f;
    private float arcHeight = 8f;

    private GameObject target;
    private Transform targetTransform;
    private Vector3 startPos;
    private float t;
    private int targetIndex;
    private Vector3 lastPosition;
    private bool returning = false;
    private bool reversingStarted = false;
    [SerializeField] private Vector3 explosionCenter = Vector3.zero;
    [SerializeField] private float explosionRadius = 5f;
    private Vector3 randomExplosionTarget;
    private bool goingToRandomExplosion = false;

    void Start()
    {
        speed = RandomIslands.Instance.GetBombSpeed();
        arcHeight = RandomIslands.Instance.GetBombArcHeight();
        targets = GameObject.FindGameObjectsWithTag("Island");
        int step = 0;
        do
        {
            step++;
            targetIndex = Random.Range(0, targets.Length);
            target = targets[targetIndex];
            if (step > 20)
            {
                // 100 denemeden sonra aktif ada bulunamazsa bombayı yok et
                Destroy(gameObject);
                break;
            }
        } while (target == null || !target.GetComponent<IslandController>().IsActive());
        if (target == null) return;
        target.GetComponent<IslandController>().SetIsActive(false);
        target.transform.GetChild(1).gameObject.SetActive(true);
        targetTransform = target.GetComponent<Transform>();
        target.GetComponent<Renderer>().material.color = Color.yellow;
        startPos = transform.position;
        t = 0;

    }



    public void SetReturning()
    {
        returning = true;

        // Rastgele bir patlama hedefi belirle
        Vector2 randomCircle = Random.insideUnitCircle * explosionRadius;
        randomExplosionTarget = new Vector3(
            explosionCenter.x + randomCircle.x,
            explosionCenter.y,
            explosionCenter.z + randomCircle.y
        );

        goingToRandomExplosion = true;
    }

    void Update()
    {
        MoveBomb();
        if (returning)
        {
            ReverseBomb();
        }
    }

    private void MoveBomb()
    {
        if (target == null) return;

        t += Time.deltaTime * (speed / Vector3.Distance(startPos, targetTransform.position));
        t = Mathf.Clamp01(t);

        // Yavaş başlayıp hızlanan hareket (ease-in)
        float smoothT = t * t;

        Vector3 directPos = Vector3.Lerp(startPos, targetTransform.position, smoothT);
        float height = Mathf.Sin(smoothT * Mathf.PI) * arcHeight;

        transform.position = new Vector3(directPos.x, directPos.y + height, directPos.z);
        // Bu fonksiyon Update içinde hareketi kontrol etmek için kullanılabilir
    }

    private void ReverseBomb()
    {
        Debug.Log("Reversing Bomb");
        if (!reversingStarted)
        {
            // Sadece dönüş başlarken ayarlıyoruz
            t = 0;
            lastPosition = transform.position;
            target.GetComponent<IslandController>().SetIsActive(true);
            target.transform.GetChild(1).gameObject.SetActive(false);
            reversingStarted = true;
        }

        t += Time.deltaTime * (speed * 1.5f / Vector3.Distance(lastPosition, goingToRandomExplosion ? randomExplosionTarget : startPos));
        t = Mathf.Clamp01(t);

        // hızlı başlayıp yavaşlayan hareket (ease-out)
        float smoothT = 1 - (1 - t) * (1 - t);

        Vector3 directPos = Vector3.Lerp(lastPosition, goingToRandomExplosion ? randomExplosionTarget : startPos, smoothT);
        float height = Mathf.Sin(smoothT * Mathf.PI) * arcHeight / 2;

        transform.position = new Vector3(directPos.x, directPos.y + height, directPos.z);


    }

    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Island"))
        {
            RandomIslands.Instance.ExplodeIsland(other.transform.position);
            // Player ile çarpışma durumunda yapılacaklar
            Destroy(gameObject);
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Taverna"))
        {
            // Player ile çarpışma durumunda yapılacaklar
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
            StartCoroutine(Explode());

        }
    }


}
