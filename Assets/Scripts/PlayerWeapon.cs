using UnityEngine;

public class PlayerWeapon : MonoBehaviour {
    public float boltSpeed = 30f;   
    public int loadedBolts = 0;

    public int currentMax = 1;
    public int maxLoadedBolts = 5;
    public float projectileSpread = 10f;

    public GameLoop game;

    public Vector3 boxSize = new Vector3(1f, 1f, 1f);
    public Vector3 meleeAttackOriginPosition;
    public Vector3 meleeAttackOriginRotation;

    public LayerMask meleeTargetLayers;

    public GameObject[] weaponPrefabs;
    public GameObject currentModel;

    public GameObject boltProjectilePrefab = null;

    public Vector3 projectileSpawnLocation = new Vector3(0,0,0);

    private CameraTracker ct;

    void Start()
    {
        if (weaponPrefabs.Length > 0) {
            currentModel = Instantiate(weaponPrefabs[0], transform.position, transform.rotation, transform);
        }

        if (boltProjectilePrefab == null){
            Debug.Log("bolt prefab not found");

        }

        GameObject camTrackerObject = GameObject.FindWithTag("CameraTracker");
        ct = camTrackerObject.GetComponent<CameraTracker>();
    }

    void Update(){

        Vector3 mousePos = ct.GetMouseWorldPosition();
        mousePos.y = currentModel.transform.position.y;
        Vector3 dir = (currentModel.transform.position - mousePos);

        Quaternion targetRot = Quaternion.LookRotation(dir) * Quaternion.Euler(0, -90, 0);
        currentModel.transform.rotation = Quaternion.Slerp(currentModel.transform.rotation, targetRot, Time.deltaTime * 10f);
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

        Vector3 mousePos = ct.GetMouseWorldPosition();
        mousePos.y = projectileSpawnLocation.y;

        Vector3 location = transform.position + transform.rotation * projectileSpawnLocation;

        Vector3 direction = (mousePos - location).normalized;
        Quaternion rot = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -90, 0);

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
                ServiceMeleeHit(parentObject, parentTag);
            } else {
                ServiceMeleeHit(hit.gameObject, hit.gameObject.tag);
            }
        }
    }

    private void ServiceMeleeHit(GameObject hitObject, string tag){
        if (tag == "StandardEnemy"){
            StandardEnemyController enemy = hitObject.GetComponent<StandardEnemyController>();
            enemy.TakeDamage(10);
        } else if (tag == "FastEnemy"){
            FastEnemyController enemy = hitObject.GetComponent<FastEnemyController>();
            enemy.TakeDamage(10);
        } if (tag == "TankEnemy"){
            TankEnemyController enemy = hitObject.GetComponent<TankEnemyController>();
            enemy.TakeDamage(10);
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