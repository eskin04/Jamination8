using System.Collections;
using UnityEngine;
using DG.Tweening;

public class RandomIslands : MonoBehaviour
{
    public static RandomIslands Instance { get; private set; }
    [SerializeField] private ParticleSystem waterEffect;
    [SerializeField] private ParticleSystem explosionEffect;
    [SerializeField] private GameObject[] islands;
    private Material currentIslandMaterial;
    private Color redMaterial;
    private Color greenMaterial;
    private int currentIslandIndex = -1;
    private float changeInterval = 4.0f; // Time in seconds between island changes
    private float timer = 2.0f;
    private float bombSpeed = 10f;
    private float bombArcHeight = 8f;

    void Awake()
    {
        // Eğer başka bir instance varsa, bunu yok et (tek kalacak)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    void Start()
    {

        redMaterial = Color.red;
        greenMaterial = Color.green;
        StartCoroutine(IncreasSpeedOverTime());
        StartCoroutine(IncreasBombSpeedOverTime());
        StartCoroutine(IncreasBombArcHeightOverTime());
    }

    private IEnumerator IncreasBombSpeedOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(15f);
            if (bombSpeed < 25f) // 15 saniyede bir hız artışı
                bombSpeed += 2f; // Hızı artır
            else break;

        }
        yield return null;
    }

    private IEnumerator IncreasBombArcHeightOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(15f);
            if (bombArcHeight < 10f) // 20 saniyede bir yay yüksekliği artışı
                bombArcHeight += 0.5f; // Yay yüksekliğini artır
            else break;
        }
        yield return null;
    }

    public float GetBombSpeed()
    {
        return bombSpeed;
    }
    public float GetBombArcHeight()
    {
        return bombArcHeight;
    }



    private IEnumerator IncreasSpeedOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(15f); // 30 saniyede bir hız artışı
            if (changeInterval > 2.0f) // Minimum interval sınırı
            {
                changeInterval -= 0.5f; // Hızı artır (intervali azalt)
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
        timer += Time.deltaTime;
        if (timer >= changeInterval)
        {
            ChangeIsland();
            timer = 0.0f;
        }
    }

    private void ChangeIsland()
    {


        // Select a new random island index
        int newIslandIndex;
        int step = 0;
        do
        {
            step++;
            newIslandIndex = Random.Range(0, islands.Length);
            if (step > 20)
            {
                // 100 denemeden sonra farklı ada bulunamazsa değişiklik yapma
                return;
            }
        } while (islands[newIslandIndex] == null || !islands[newIslandIndex].GetComponent<IslandController>().IsActive()); // Ensure it's different from the current index
        if (islands[newIslandIndex] == null)
            return;
        islands[newIslandIndex].SetActive(true);
        currentIslandIndex = newIslandIndex;
        islands[currentIslandIndex].GetComponent<IslandController>().SetIsActive(false);

        Transform childTransform = islands[currentIslandIndex].transform.GetChild(0).transform;

        Sequence seq = DOTween.Sequence();

        seq.Append(childTransform.DOShakePosition(changeInterval / 4, 1, 6, 30, false, true));
        seq.Append(childTransform.DOMoveY(childTransform.position.y - 2, changeInterval / 6).SetEase(Ease.InElastic));
        seq.AppendCallback(() =>
        {
            waterEffect.transform.position = islands[currentIslandIndex].transform.position - Vector3.up * 1;
            waterEffect.Play();
            islands[currentIslandIndex].SetActive(false);

            // Callback işlemleri burada yapılabilir
        });
        seq.AppendInterval(changeInterval / 4); // yarım saniye bekle


        seq.OnComplete(() =>
        {
            islands[newIslandIndex].SetActive(true);
            islands[currentIslandIndex].GetComponent<IslandController>().SetIsActive(true);

            childTransform.DOMoveY(childTransform.position.y + 2, changeInterval / 6).SetEase(Ease.OutElastic);

        });
    }

    public void ExplodeIsland(Vector3 position)
    {
        explosionEffect.transform.position = position - Vector3.up;
        explosionEffect.Play();
    }

}



