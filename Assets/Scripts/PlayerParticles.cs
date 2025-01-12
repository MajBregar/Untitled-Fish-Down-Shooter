using UnityEngine;

public class PlayerParticles : MonoBehaviour {
    public float v = 4f;
    public float speedMultiplier = 0.1f;
    private ParticleSystem ps;
    private Rigidbody rb;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ps = GetComponent<ParticleSystem>();

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = rb.linearVelocity;
        var velocityOverLifetimeModule = ps.velocityOverLifetime;
        velocityOverLifetimeModule.x = -velocity.x * speedMultiplier;
        velocityOverLifetimeModule.y = v;
        velocityOverLifetimeModule.z = -velocity.z * speedMultiplier;
        
    }
}
