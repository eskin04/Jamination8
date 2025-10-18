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
    private int disableIslandIndex = -1;

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


    }

    private IEnumerator IncreasSpeedOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f); // 30 saniyede bir hız artışı
            if (changeInterval > 1.0f) // Minimum interval sınırı
            {
                changeInterval -= 0.5f; // Hızı artır (intervali azalt)
            }
        }
    }

    public void DisableRandomIsland(int index)
    {
        disableIslandIndex = index;
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
        } while (newIslandIndex == currentIslandIndex || newIslandIndex == disableIslandIndex); // Ensure it's different from the current index

        islands[newIslandIndex].SetActive(true);
        currentIslandIndex = newIslandIndex;

        // Change the material color randomly between red and green
        currentIslandMaterial = islands[currentIslandIndex].GetComponent<Renderer>().material;
        currentIslandMaterial.color = redMaterial;

        // Wait for a short duration before changing to green
        yield return new WaitForSeconds(1f);
        islands[currentIslandIndex].SetActive(false);
        yield return new WaitForSeconds(1f);
        currentIslandMaterial.color = greenMaterial;

        islands[currentIslandIndex].SetActive(true);
    }


}
