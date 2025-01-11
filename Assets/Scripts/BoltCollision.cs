using UnityEngine;

public class Projectile : MonoBehaviour
{

     void Update()
    {
        if (Vector3.Distance(transform.position, Vector3.zero) > 200)
        {
            Destroy(transform.parent.gameObject);
        }
    }
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