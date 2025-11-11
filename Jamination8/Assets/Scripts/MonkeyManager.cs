using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using TMPro;

public class MonkeyManager : MonoBehaviour
{
    public static MonkeyManager Instance;
    [SerializeField] private GameObject monkey;
    [SerializeField] private ParticleSystem buffEffect;
    [SerializeField] private ParticleSystem debuffEffect;
    [SerializeField] private GameObject buffPanel;
    [SerializeField] private GameObject debuffPanel;
    [SerializeField] private TextMeshProUGUI buffText;
    [SerializeField] private TextMeshProUGUI deBuffText;
    private bool isSpawning = false;
    private bool isBuff = false;
    private bool isDebuff = false;
    public float playerJumpDuration = 0.3f;
    public bool isGameOver = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
    void Start()
    {
        StartCoroutine(SpawnMonkeyRoutine());
    }

    public void SetSpawning(bool status)
    {
        isSpawning = status;
    }

    public bool GetSpawning()
    {
        return isSpawning;
    }

    public bool IsBuff()
    {
        return isBuff;
    }

    public bool IsDebuff()
    {
        return isDebuff;
    }

    public void GetShot()
    {
        float randomValue = UnityEngine.Random.value;
        if (randomValue <= .5f)
        {
            // Buff
            isBuff = true;
            Debug.Log("Buff Alındı");
            buffPanel.SetActive(true);
            buffEffect.transform.position = monkey.transform.position;
            buffEffect.Play();
            StartCoroutine(PlayerBuffCoroutine());
        }
        else
        {
            // Debuff
            isDebuff = true;
            Debug.Log("Debuff Alındı");
            debuffPanel.SetActive(true);
            debuffEffect.transform.position = monkey.transform.position;
            debuffEffect.Play();
            StartCoroutine(PlayerDebuffCoroutine());
        }
    }

    private IEnumerator PlayerBuffCoroutine()
    {
        int steps = 10;
        for (int i = 0; i < steps; i++)
        {
            buffText.text = (steps - i).ToString();
            yield return new WaitForSeconds(1f);
        }
        buffPanel.SetActive(false);
        isBuff = false;
    }

    private IEnumerator PlayerDebuffCoroutine()
    {
        int steps = 5;
        for (int i = 0; i < steps; i++)
        {
            deBuffText.text = (steps - i).ToString();
            yield return new WaitForSeconds(1f);
        }
        debuffPanel.SetActive(false);
        isDebuff = false;
    }

    private IEnumerator SpawnMonkeyRoutine()
    {
        while (true)
        {
            if (isGameOver) break;
            float spawnMonkey = UnityEngine.Random.Range(10, 20f);
            yield return new WaitForSeconds(spawnMonkey); // 20 saniyede bir maymun spawnla

            monkey.transform.DOLocalMoveY(5, 2f).SetEase(Ease.InElastic).OnComplete(() =>
            {
                isSpawning = true;

                monkey.transform.DOLocalMoveY(0, 2f).SetEase(Ease.OutElastic).SetDelay(.5f).OnComplete(() =>
                {
                    isSpawning = false;
                });
            });
        }
        yield return null;
    }
}
