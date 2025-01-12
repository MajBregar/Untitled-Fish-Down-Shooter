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
    
    void OnTriggerEnter(Collider other)
    {
        GameObject hit = other.gameObject;

        if (hit.tag == "StandardEnemy"){
            StandardEnemyController enemy = hit.GetComponent<StandardEnemyController>();
            enemy.TakeDamage(1);
        } else if (hit.tag == "FastEnemy"){
            FastEnemyController enemy = hit.GetComponent<FastEnemyController>();
            enemy.TakeDamage(1);
        } else if (hit.tag == "TankEnemy"){
            TankEnemyController enemy = hit.GetComponent<TankEnemyController>();
            enemy.TakeDamage(1);
        }

        Destroy(transform.parent.gameObject); //delete bolt
    }
}