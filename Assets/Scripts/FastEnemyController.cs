using UnityEngine;

public class FastEnemyController : MonoBehaviour
{
    // Enemy attributes
    public int Health = 100;
    public int Difficulty = 10;
    public int CashDrop = 3;

    // State management
    private int EnemyState = 0;

    // Movement and attack parameters
    public float TurnSpeed = 2f;
    public float MeleeAttackRange = 5f;
    public float MeleeAttackDuration = 1f;
    public float MeleeAttackCooldown = 1.5f;

    // Animation
    public float DeathAnimationLength = 2f;
    public float MeleeAttackForwardMovement = 2f; // Forward movement during melee attack
    public float MeleeAttackPitchAngle = 15f; // Pitch angle during melee attack

    // Internal state
    private float lastMeleeAttackTime = 0f;
    private float meleeDurationTimer = 0f;

    private Transform playerTransform;
    private bool avoidDisplacement = false;
    private Quaternion originalRotation;

    void Start()
    {
        // Initialization logic
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Melee attack
        if (distanceToPlayer <= MeleeAttackRange && Time.time >= lastMeleeAttackTime + MeleeAttackCooldown)
        {
            AttemptMeleeAttack(Time.time);
        }

        // Chase player if not attacking
        if (Time.time >= meleeDurationTimer)
        {
            avoidDisplacement = false;
            ChasePlayer();
        }
    }

    private void AttemptMeleeAttack(float currentTime)
    {
        Vector3 playerDirection = (playerTransform.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance < MeleeAttackRange)
        {
            meleeDurationTimer = currentTime + MeleeAttackDuration;
            avoidDisplacement = true;
            originalRotation = transform.rotation;
            Debug.Log("Performing melee attack");
            StartCoroutine(MeleeAttackAnimation(playerDirection));
        }
    }

    private System.Collections.IEnumerator MeleeAttackAnimation(Vector3 playerDirection)
    {
        float elapsedTime = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(playerDirection) * Quaternion.Euler(-MeleeAttackPitchAngle, 0, 0);

        while (elapsedTime < MeleeAttackDuration)
        {
            float progress = elapsedTime / MeleeAttackDuration;

            // Move forward
            transform.position += playerDirection * (MeleeAttackForwardMovement * Time.deltaTime / MeleeAttackDuration);

            // Pitch forward
            transform.rotation = Quaternion.Slerp(originalRotation, targetRotation, progress);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset rotation after attack
        transform.rotation = originalRotation;

        ScanForMeleeAttack(1f); // Check for player hit during melee
    }

    private void ScanForMeleeAttack(float scanSize)
    {
        Vector3 playerDirection = (playerTransform.position - transform.position).normalized * MeleeAttackRange;
        Vector3 attackLocation = transform.position + playerDirection;

        Bounds attackBounds = new Bounds(attackLocation, new Vector3(scanSize * 2, 12, scanSize * 2));
        Collider playerCollider = playerTransform.GetComponent<Collider>();

        if (playerCollider != null && attackBounds.Intersects(playerCollider.bounds))
        {
            PlayerController player = playerTransform.GetComponent<PlayerController>();
            if (player != null)
            {
                player.Die(); // Kill Player
                Debug.Log("Player hit by melee attack");
            }
        }
    }

    private void ChasePlayer()    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, TurnSpeed * Time.deltaTime);

        if (!avoidDisplacement)
        {
            transform.position += direction * TurnSpeed * Time.deltaTime;
        }
    }

    public void TakeDamage(int damage)    {
        Health -= damage;
        Debug.Log($"Enemy took {damage} damage, remaining health: {Health}");

        if (Health <= 0)
        {
            Die();
        }
    }

    private void Die()    {
        Debug.Log("Enemy is dead.");
        // Implement death animation and logic
        Destroy(gameObject, DeathAnimationLength);
    }
}