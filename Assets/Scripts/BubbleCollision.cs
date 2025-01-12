using UnityEngine;

public class BubbleCollision : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, Vector3.zero) > 200)
        {
            Destroy(gameObject);
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        GameObject hit = other.gameObject;

        if (hit.tag == "Player"){
            PlayerController player = hit.GetComponent<PlayerController>();
            player.TakeDamage();
        } 
        Destroy(gameObject); //delete bubble
    }
}
