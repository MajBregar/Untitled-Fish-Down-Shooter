using UnityEngine;

public class Projectile : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        GameObject hitObject = collision.gameObject;

        Debug.Log("Projectile hit: " + hitObject.name);

        Destroy(transform.parent.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject hitObject = other.gameObject;

        Debug.Log("Projectile triggered by: " + hitObject.name);

        Destroy(transform.parent.gameObject);
    }
}