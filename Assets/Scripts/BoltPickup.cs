using UnityEngine;

public class BoltPickup : MonoBehaviour
{   
    public float height = 1f;
    public float rotationSpeed = 30f;
    public float movementAmplitude = 2f;
    public float movementFrequency = 1f;

    void Start()
    {
    }

    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        float verticalMovement = Mathf.Sin(Time.time * movementFrequency) * movementAmplitude + height;
        transform.position = new Vector3(transform.position.x, verticalMovement, transform.position.z);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            PlayerController c = other.GetComponent<PlayerController>();
            c.PickupBolt();
            Destroy(gameObject);
        }
    }

}
