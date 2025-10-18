using UnityEngine;

public class GuidedBomb : MonoBehaviour
{
    private GameObject[] targets;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float arcHeight = 8f;

    private GameObject target;
    private Transform targetTransform;
    private Vector3 startPos;
    private float t;
    private int targetIndex;
    private Vector3 lastPosition;
    private bool returning = false;
    private bool reversingStarted = false;

    void Start()
    {
        targets = GameObject.FindGameObjectsWithTag("Island");
        do
        {
            targetIndex = Random.Range(0, targets.Length);
            target = targets[targetIndex];
        } while (!target.activeSelf);
        targetTransform = target.GetComponent<Transform>();
        RandomIslands.Instance.DisableRandomIsland(targetIndex);
        target.GetComponent<Renderer>().material.color = Color.yellow;
        startPos = transform.position;
        t = 0;
    }

    public void SetReturning()
    {
        returning = true;
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
            target.GetComponent<Renderer>().material.color = Color.green;

            reversingStarted = true;
        }

        t += Time.deltaTime * (speed * 1.5f / Vector3.Distance(lastPosition, startPos));
        t = Mathf.Clamp01(t);

        // hızlı başlayıp yavaşlayan hareket (ease-out)
        float smoothT = 1 - (1 - t) * (1 - t);

        Vector3 directPos = Vector3.Lerp(lastPosition, startPos, smoothT);
        float height = Mathf.Sin(smoothT * Mathf.PI) * arcHeight / 2;

        transform.position = new Vector3(directPos.x, directPos.y + height, directPos.z);
        // Bu fonksiyon Update içinde hareketi kontrol etmek için kullanılabilir
        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Island"))
        {
            // Player ile çarpışma durumunda yapılacaklar
            Destroy(gameObject);
            Destroy(other.gameObject);
        }
    }


}
