using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveStep = 3f;    // 1 adım uzunluğu
    [SerializeField] private float jumpHeight = 5f;  // zıplama yüksekliği
    [SerializeField] private float jumpDuration = 0.3f;
    [SerializeField] private GameObject endGameUI;
    private float firstJumpDuration = 0.3f; // 3 adımı tamamlaması gereken süre
    private bool isJumping = false;
    private Vector3 jumpTarget;
    private Vector3 jumpStart;
    private float jumpTime;
    [SerializeField] private LayerMask islandLayer;
    [SerializeField] private GameObject groundCheck;
    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 10.0f;
    [SerializeField] private float attackCooldown = 1.0f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask targetLayer;
    [Header("Shooting Settings")]
    [SerializeField] private Transform shootPoint;
    [SerializeField] private GameObject bulletPrefab;

    [Header("Animation & Effects")]
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem dieEffect;
    [SerializeField] private ParticleSystem jumpEffect;
    [SerializeField] private ParticleSystem parryEffect;
    private float lastAttackTime = 0.0f;
    private bool isGrounded;
    private bool isRotating = false;
    private Rigidbody rb;
    private bool isGameOver = false;
    private int score = 0;
    private int highScore = 0;
    private float highTime = 0f;
    private float timeElapsed = 0f;
    [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI gameOverTimeText;
    [SerializeField] private TextMeshProUGUI highTimeText;
    [SerializeField] private TextMeshProUGUI parryText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        firstJumpDuration = jumpDuration;
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        highTime = PlayerPrefs.GetFloat("HighTime", 0f);
        StartCoroutine(UpdateTime());

    }

    private IEnumerator UpdateTime()
    {
        while (!isGameOver)
        {
            // timetext formatı 00:00
            timeText.text = string.Format("{0:D2}:{1:D2}", Mathf.FloorToInt(timeElapsed / 60), Mathf.FloorToInt(timeElapsed % 60));
            timeElapsed += 1f;
            yield return new WaitForSeconds(1f);
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (isGameOver) return;


        PlayerMovement();
        if (!isGrounded && !isRotating) StartCoroutine(ResetRotation());
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerAttack();
        }
        if (Input.GetMouseButtonDown(0))
        {
            PlayerShoot();
        }

    }

    private void FixedUpdate()
    {
        if (isGameOver) return;
        if (isJumping)
        {
            jumpTime += Time.fixedDeltaTime;
            float t = jumpTime / jumpDuration;
            if (t >= 1f)
            {
                t = 1f;
                isJumping = false;
            }

            // Parabolik hareket
            Vector3 nextPos = Vector3.Lerp(jumpStart, jumpTarget, t);
            // Daha düzgün yay için y eksenini ayrı Sin eğrisiyle ayarla
            float height = Mathf.Sin(t * Mathf.PI) * jumpHeight;
            nextPos.y = jumpStart.y + height;

            rb.MovePosition(nextPos);
        }
    }


    private void PlayerMovement()
    {
        isGrounded = Physics.CheckSphere(groundCheck.transform.position, 0.1f, islandLayer);

        if (isJumping) return; // zıplama devam ediyorsa yeni giriş engellenir

        Vector3 direction = Vector3.zero;
        if (MonkeyManager.Instance.IsBuff())
        {
            jumpDuration = 0.1f;
        }
        else
        {
            jumpDuration = firstJumpDuration;
        }
        if (MonkeyManager.Instance.IsDebuff())
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                direction = Vector3.forward;
                animator.SetTrigger("isJump");
                jumpEffect.transform.position = transform.position - Vector3.up;
                jumpEffect.Play();
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                direction = Vector3.back;
                animator.SetTrigger("isJump");
                jumpEffect.transform.position = transform.position - Vector3.up;
                jumpEffect.Play();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                direction = Vector3.left;
                animator.SetTrigger("isJump");
                jumpEffect.transform.position = transform.position - Vector3.up;
                jumpEffect.Play();
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                direction = Vector3.right;
                animator.SetTrigger("isJump");
                jumpEffect.transform.position = transform.position - Vector3.up;
                jumpEffect.Play();
            }

        }
        else
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                direction = Vector3.forward;
                animator.SetTrigger("isJump");
                jumpEffect.transform.position = transform.position - Vector3.up;
                jumpEffect.Play();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                direction = Vector3.back;
                animator.SetTrigger("isJump");
                jumpEffect.transform.position = transform.position - Vector3.up;

                jumpEffect.Play();
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                direction = Vector3.left;
                animator.SetTrigger("isJump");
                jumpEffect.transform.position = transform.position - Vector3.up;
                jumpEffect.Play();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                direction = Vector3.right;
                animator.SetTrigger("isJump");
                jumpEffect.transform.position = transform.position - Vector3.up;
                jumpEffect.Play();
            }
        }


        if (direction != Vector3.zero && isGrounded)
        {
            isJumping = true;
            jumpStart = rb.position;
            jumpTarget = rb.position + direction * moveStep + Vector3.up * jumpHeight;
            jumpTime = 0f;

            // Karakterin baktığı yönü ayarla
            Vector3 lookTarget = new Vector3(jumpTarget.x, transform.position.y, jumpTarget.z);
            transform.LookAt(lookTarget);
            isRotating = false;
        }
    }


    private IEnumerator ResetRotation()
    {
        isRotating = true;
        yield return new WaitUntil(() => isGrounded);
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void PlayerAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown && !isGrounded) return;

        lastAttackTime = Time.time;
        animator.SetTrigger("isKick");

        Collider[] hitColliders = Physics.OverlapSphere(attackPoint.position, attackRange, targetLayer);
        foreach (var hitCollider in hitColliders)
        {
            // Apply damage or effects to the hit target
            if (hitCollider.CompareTag("Bullet"))
            {
                Debug.Log("Hit: " + hitCollider.name);
                parryEffect.transform.position = hitCollider.transform.position;
                parryEffect.Play();
                parryText.gameObject.SetActive(true);
                parryText.DOFade(0, 2f).OnComplete(() =>
                {
                    parryText.color = new Color(parryText.color.r, parryText.color.g, parryText.color.b, 1);
                    parryText.gameObject.SetActive(false);
                });
                hitCollider.gameObject.GetComponent<GuidedBomb>().SetReturning();
                score += 10;
                // scoretext
                DOTween.To(() => int.Parse(scoreText.text), x => scoreText.text = x.ToString(), score, 0.5f);

            }
        }

        // Placeholder for player attack logic
    }

    private void PlayerShoot()
    {
        if (!MonkeyManager.Instance.GetSpawning())
            return;
        MonkeyManager.Instance.SetSpawning(false);
        animator.SetTrigger("throw");
        Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("effect"))
        {
            dieEffect.transform.position = transform.position;
            dieEffect.Play();
            // Additional game over logic can be added here
        }
        if (other.CompareTag("water"))
        {
            Debug.Log("Game Over!");
            endGameUI.SetActive(true);
            MonkeyManager.Instance.isGameOver = true;
            isGameOver = true;
            if (score > highScore)
            {
                highScore = score;
                PlayerPrefs.SetInt("HighScore", highScore);
            }

            if (timeElapsed > highTime)
            {
                highTime = timeElapsed;
                PlayerPrefs.SetFloat("HighTime", highTime);
            }
            StopAllCoroutines();
            gameOverScoreText.text = score.ToString();
            gameOverTimeText.text = string.Format("{0:D2}:{1:D2}", Mathf.FloorToInt(timeElapsed / 60), Mathf.FloorToInt(timeElapsed % 60));
            highScoreText.text = highScore.ToString();
            highTimeText.text = string.Format("{0:D2}:{1:D2}", Mathf.FloorToInt(highTime / 60), Mathf.FloorToInt(highTime % 60));

            // Additional game over logic can be added here
        }
    }
}
