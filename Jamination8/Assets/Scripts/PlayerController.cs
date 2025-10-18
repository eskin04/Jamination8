using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveStep = 3f;    // 1 adım uzunluğu
    [SerializeField] private float jumpHeight = 5f;  // zıplama yüksekliği
    [SerializeField] private float jumpDuration = 0.3f; // 3 adımı tamamlaması gereken süre
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

    [Header("Animation & Effects")]
    [SerializeField] private Animator animator;
    private float lastAttackTime = 0.0f;
    private bool isGrounded;
    private bool isRotating = false;
    private Rigidbody rb;
    private bool isGameOver = false;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();


    }

    // Update is called once per frame
    void Update()
    {



        PlayerMovement();
        if (!isGrounded && !isRotating) StartCoroutine(ResetRotation());
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerAttack();
        }
    }

    private void FixedUpdate()
    {
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

        if (Input.GetKeyDown(KeyCode.W))
        {
            direction = Vector3.forward;
            animator.SetTrigger("isJump");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            direction = Vector3.back;
            animator.SetTrigger("isJump");
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            direction = Vector3.left;
            animator.SetTrigger("isJump");
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            direction = Vector3.right;
            animator.SetTrigger("isJump");
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

                hitCollider.gameObject.GetComponent<GuidedBomb>().SetReturning();
            }
        }

        // Placeholder for player attack logic
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("water"))
        {
            Debug.Log("Game Over!");
            isGameOver = true;
            Time.timeScale = 0f;
            // Additional game over logic can be added here
        }
    }
}
