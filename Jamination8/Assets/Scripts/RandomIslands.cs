using System.Collections;
using UnityEngine;

public class RandomIslands : MonoBehaviour
{
    public static RandomIslands Instance { get; private set; }
    [SerializeField] private GameObject[] islands;
    private Material currentIslandMaterial;
    private Color redMaterial;
    private Color greenMaterial;
    private int currentIslandIndex = -1;
    private float changeInterval = 4.0f; // Time in seconds between island changes
    private float timer = 4.0f;
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
        DontDestroyOnLoad(gameObject); // Sahne değişince yok olmasın
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
        }
    }

    private IEnumerator IncreasBombArcHeightOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(15f);
            if (bombArcHeight < 10f) // 20 saniyede bir yay yüksekliği artışı
                bombArcHeight += 0.5f; // Yay yüksekliğini artır
        }
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
        }
    }


    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= changeInterval)
        {
            StartCoroutine(ChangeIsland());
            timer = 0.0f;
        }
    }

    private IEnumerator ChangeIsland()
    {


        // Select a new random island index
        int newIslandIndex;
        do
        {
            newIslandIndex = Random.Range(0, islands.Length);
        } while (!islands[newIslandIndex].GetComponent<IslandController>().IsActive()); // Ensure it's different from the current index

        islands[newIslandIndex].SetActive(true);
        currentIslandIndex = newIslandIndex;
        islands[currentIslandIndex].GetComponent<IslandController>().SetIsActive(false);

        // Change the material color randomly between red and green
        currentIslandMaterial = islands[currentIslandIndex].GetComponent<Renderer>().material;
        currentIslandMaterial.color = redMaterial;

        // Wait for a short duration before changing to green
        yield return new WaitForSeconds(changeInterval / 4);
        islands[currentIslandIndex].SetActive(false);
        yield return new WaitForSeconds(changeInterval / 4);
        currentIslandMaterial.color = greenMaterial;
        islands[currentIslandIndex].SetActive(true);
        islands[currentIslandIndex].GetComponent<IslandController>().SetIsActive(true);
    }


}
