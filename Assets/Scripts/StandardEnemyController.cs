using UnityEngine;

public class StandardEnemyController : MonoBehaviour
{
    public int Health = 10;
    public int Difficulty = 10;
    public int CashDrop = 3;
    private int EnemyState = 0;

    public float TurnSpeed = 2f;
    public float MeleeAttackRange = 5f;
    public float MeleeAttackDuration = 1f;
    public float MeleeAttackCooldown = 1.5f;
    public float RangedAttackRangeMin = 15f;
    public float RangedAttackRangeMax = 20f;
    public float RangedAttackCooldown = 10f;

    public float DeathAnimationLength = 2f;

    private float lastMeleeAttackTime = 0f;
    private float lastRangedAttackTime = 0f;

    private Transform playerTransform;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= MeleeAttackRange && Time.time >= lastMeleeAttackTime + MeleeAttackCooldown)
        {
            PerformMeleeAttack();
        }
        else if (distanceToPlayer >= RangedAttackRangeMin && distanceToPlayer <= RangedAttackRangeMax && Time.time >= lastRangedAttackTime + RangedAttackCooldown)
        {
            PerformRangedAttack();
        }
        else
        {
            ChasePlayer();
        }
    }

    private void PerformMeleeAttack()
    {
        lastMeleeAttackTime = Time.time;
        Debug.Log("Performing melee attack");
        // TODO Implement melee attack logic
    }

    private void PerformRangedAttack()
    {
        lastRangedAttackTime = Time.time;
        Debug.Log("Performing ranged attack");
        // TODO Implement ranged attack logic
    }

    private void ChasePlayer()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, TurnSpeed * Time.deltaTime);

        transform.position += direction * TurnSpeed * Time.deltaTime;
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        Debug.Log($"Enemy took {damage} damage, remaining health: {Health}");

        if (Health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy is dead.");
        Destroy(gameObject, DeathAnimationLength);
    }
}
