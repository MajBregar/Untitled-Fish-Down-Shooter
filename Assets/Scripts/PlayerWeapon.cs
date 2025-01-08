using UnityEngine;

public class PlayerWeapon : MonoBehaviour {
    public float boltSpeed = 30f;   
    public int loadedBolts = 0;

    public int currentMax = 0;
    public int maxLoadedBolts = 5;
    public float projectileSpread = 10f;

    public GameLoop game;

    public Vector3 boxSize = new Vector3(1f, 1f, 1f);
    public Vector3 meleeAttackOriginPosition;
    public Vector3 meleeAttackOriginRotation;

    public LayerMask meleeTargetLayers;

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

    public void AddBolt(){
        if (currentMax < maxLoadedBolts){
            currentMax += 1;
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

    public void Melee(){

        Quaternion rot = transform.rotation * Quaternion.Euler(meleeAttackOriginRotation);
        Vector3 attackPosition = transform.position + transform.rotation * meleeAttackOriginPosition;

        Collider[] hitColliders = Physics.OverlapBox(attackPosition, boxSize / 2, rot, meleeTargetLayers);

        foreach (Collider hit in hitColliders) {

            Transform parentTransform = hit.gameObject.transform.parent;
            if (parentTransform != null) {
                GameObject parentObject = parentTransform.gameObject;
                string parentTag = parentObject.tag;
                ServiceHit(parentObject, parentTag);
            }
        }
    }

    private void ServiceHit(GameObject hitObject, string tag){
        if (tag == "Enemy"){



        } else if (tag == "WaveCrate"){
            Vector3 cratePosition = hitObject.transform.position;
            Destroy(hitObject);
            game.WaveCrateBreakEvent(cratePosition);
        }

    }

    private void OnDrawGizmos()
    {
        Quaternion rot = transform.rotation * Quaternion.Euler(meleeAttackOriginRotation);
        Vector3 attackPosition = transform.position + transform.rotation * meleeAttackOriginPosition;

        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(attackPosition, rot, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
        
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