using UnityEngine;

public class FastEnemyController: MonoBehaviour {
    // Enemy attributes
    public int Health = 10;
    public int Difficulty = 10;
    public int CashDrop = 3;

    // Movement and attack parameters
    public float MaxSpeed = 3f;
    public float TurnSpeed = 2f;
    public float MeleeAttackRange = 5f;

    public Vector3 boxSize = new Vector3(1f, 1f, 1f);
    public Vector3 meleeAttackOriginPosition;
    public Vector3 meleeAttackOriginRotation;
    public LayerMask meleeTargetLayers;

    public float MeleeAttackDuration = 1f;
    public float MeleeAttackCooldown = 1.5f;
    public float DeathAnimationLength = 3f;

    public EnemyState curState;
    private float lastMeleeAttackTime = 0f;
    private float meleeDurationTimer = 0f;


    public bool dead = false;
    private Transform playerTransform;

    private PlayerController player;

    private Rigidbody rb;
    private Animator enemyAnimator;
    private GameLoop game;


    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.GetComponent<PlayerController>();
        playerTransform = playerObject.transform;

        GameObject gameLoopObject = GameObject.FindGameObjectWithTag("GameLoop");
        game = gameLoopObject.GetComponent<GameLoop>();

        rb = GetComponent<Rigidbody>();

        enemyAnimator = GetComponentInChildren<Animator>();

    }

    void Update()
    {
        if (playerTransform == null) return;
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        TrackPlayer();

        switch (curState){
            
            case EnemyState.ChasingPlayer:
                if (distanceToPlayer <= MeleeAttackRange && Time.time >= lastMeleeAttackTime + MeleeAttackCooldown) {
                    curState = EnemyState.MeleeAttacking;
                    meleeDurationTimer = Time.time + MeleeAttackDuration;
                    StopMovement();
                    enemyAnimator.SetTrigger("Melee");
                } else {
                    MoveForward();
                }
                break;

            case EnemyState.MeleeAttacking:
                if (Time.time >= meleeDurationTimer) {
                    bool hit = ScanForMeleeAttack(1f);

                    if (hit == true){
                        player.TakeDamage();
                    }
                    curState = EnemyState.ChasingPlayer;
                }

                break;
            default:
                Debug.Log("invalid state");
                break;
        }

    }

    private bool ScanForMeleeAttack(float scanSize) {
        if (dead == true) return false;
   
        Quaternion rot = transform.rotation * Quaternion.Euler(meleeAttackOriginRotation);
        Vector3 attackPosition = transform.position + transform.rotation * meleeAttackOriginPosition;

        Collider[] hitColliders = Physics.OverlapBox(attackPosition, boxSize / 2, rot, meleeTargetLayers);
        foreach (Collider hit in hitColliders) {
            return true;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Quaternion rot = transform.rotation * Quaternion.Euler(meleeAttackOriginRotation);
        Vector3 attackPosition = transform.position + transform.rotation * meleeAttackOriginPosition;

        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(attackPosition, rot, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
        

        Gizmos.matrix = Matrix4x4.identity;

        Gizmos.color = Color.blue;
        float radius = MeleeAttackRange; // Replace with desired radius
        int segments = 32; // More segments for a smoother circle
        float angleStep = 360f / segments;

        Vector3 previousPoint = transform.position + Vector3.right * radius;
        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector3 newPoint = transform.position + new Vector3(Mathf.Cos(angle), 0.02f, Mathf.Sin(angle)) * radius;
            Gizmos.DrawLine(previousPoint, newPoint);
            previousPoint = newPoint;
        }
    }

    private void TrackPlayer(){
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up); // Create a rotation pointing towards the player
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, TurnSpeed * Time.deltaTime);
    }
    private void MoveForward()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Vector3 movement = transform.forward * MaxSpeed * Time.deltaTime; // Move in the forward direction
        rb.MovePosition(rb.position + movement); // Use Rigidbody to move
    }

    private void StopMovement()
    {
        rb.linearVelocity = Vector3.zero;
    }


    public void TakeDamage(int damage)
    {   
        if (dead == true) return;

        Health -= damage;
        if (Health <= 0)
        {   
            Die();
        }
    }

    private void Die()
    {
        dead = true;
        enemyAnimator.SetTrigger("Die");
        Destroy(gameObject, DeathAnimationLength);
        game.EnemyDeathEvent();
    }
}
