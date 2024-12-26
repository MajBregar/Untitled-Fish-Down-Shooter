using UnityEngine;

public class PlayerWeapon : MonoBehaviour {
    public float boltSpeed = 30f;   
    public int loadedBolts = 0;

    public int currentMax = 5;
    public int maxLoadedBolts = 5;
    public float projectileSpread = 10f;

    public GameObject[] weaponPrefabs;
    private GameObject currentModel;

    public GameObject boltProjectilePrefab = null;

    public Vector3 projectileSpawnLocation = new Vector3(0,0,0);

    void Start()
    {
        if (weaponPrefabs.Length > 0) {
            currentModel = Instantiate(weaponPrefabs[0], transform.position, transform.rotation, transform);
        }

        if (boltProjectilePrefab == null){
            Debug.Log("bolt prefab not found");

        }
    }

    public void LoadBolt(){
        loadedBolts += 1;
        SwapPrefab(loadedBolts);
    }

    public void Shoot(){
        SwapPrefab(0);

        Quaternion rot = transform.rotation * Quaternion.Euler(0, 180, 0);
        Vector3 location = transform.position + transform.rotation * projectileSpawnLocation;

        int halfCount = loadedBolts / 2;
        for (int i = 0; i < loadedBolts; i++) {
            float angle = loadedBolts % 2 == 0 ? (i - halfCount) * projectileSpread + (projectileSpread / 2) : (i - halfCount) * projectileSpread;

            Quaternion boltRot = Quaternion.Euler(0, angle, 0) * rot;
            GameObject boltInstance = Instantiate(boltProjectilePrefab, location, boltRot);
            Rigidbody boltRb = boltInstance.GetComponentInChildren<Rigidbody>();
            boltRb.linearVelocity = boltInstance.transform.right * boltSpeed;
        }
        loadedBolts = 0;
    }   

    public void SwapPrefab(int index) {
        if (index >= 0 && index < weaponPrefabs.Length) {
            if (currentModel != null) {
                Destroy(currentModel);
            }
            currentModel = Instantiate(weaponPrefabs[index], transform.position, transform.rotation, transform);
        } else {
            Debug.LogError("Invalid index: " + index);
        }
    }
}