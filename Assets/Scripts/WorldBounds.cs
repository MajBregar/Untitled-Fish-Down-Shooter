using UnityEngine;

public class WorldBounds : MonoBehaviour
{
    private int triggerCount = 0;
    private GameObject playerObject;
    private bool playerInBounds = true;

    void Start() {
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    void Update() {
        if (playerInBounds == true) return;

        Vector3 newPos = playerObject.transform.position - playerObject.transform.position.normalized * Time.deltaTime;
        playerObject.transform.position = newPos;
    }

    private void OnTriggerEnter(Collider other) {
        playerInBounds = true;
    }

    private void OnTriggerExit(Collider other) {
        playerInBounds = false;
    }
}
