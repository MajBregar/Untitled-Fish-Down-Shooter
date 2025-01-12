using UnityEngine;
using UnityEngine.SceneManagement;


public enum PlayerActions {
    PlayerIdleOrRunning,
    PlayerMelee,
    PlayerShoot,
    PlayerReloading
}

public class PlayerController : MonoBehaviour {
    //movement
    public float acceleration = 10f;
    public float maxSpeed = 5f;
    public float friction = 5f;

    //death
    public bool dead = false;
    public float deathScreenDuration = 2f;
    public float deathTimer = 0f;


    private PlayerActions curState = PlayerActions.PlayerIdleOrRunning;

    //melee
    public float meleeCooldown = 0.5f;
    public float meleeDuration = 0.5f;
    private float meleeCooldownTimer = 0f;
    private float meleeDurationTimer = 0f;

    //shooting
    public float shootDuration = 0.1f;
    private float shootDurationTimer = 0f;

    //reloading
    public float reloadDuration = 0.5f;
    private float reloadDurationTimer = 0f;

    //system
    private float gameTime = 0f;
    public PlayerWeapon weapon;
    private Rigidbody rb;
    public CameraTracker camTracker;
    public UIController UI;
    private Animator playerAnimator;
    private Animator weaponAnimator;

    void Start() {   
        rb = GetComponent<Rigidbody>();

        GameObject weaponObject = GameObject.FindWithTag("Weapon");
        GameObject modelObject = GameObject.FindWithTag("PlayerModel");
        
        weaponAnimator = weaponObject.GetComponent<Animator>();
        playerAnimator = modelObject.GetComponent<Animator>();
    }

    public void TakeDamage(){
        if (dead == true) return;

        GameObject modelObject = GameObject.FindWithTag("PlayerModel");
        GameObject weaponObject = weapon.currentModel;
        
        //disable player collider
        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider collider in colliders) {
            collider.enabled = false;
        }

        //freeze position
        rb.constraints = RigidbodyConstraints.FreezePosition;

        //disable player model visibility
        Renderer modelRenderer = modelObject.GetComponent<Renderer>();
        if (modelRenderer != null) {
            modelRenderer.enabled = false;
        }

        //disable weapon model visibility
        Renderer weaponRenderer = weaponObject.GetComponentInChildren<Renderer>();
        if (weaponRenderer != null) {
            weaponRenderer.enabled = false;
        }

        deathTimer = Time.time + deathScreenDuration;
        dead = true;
    }

    public void PickupBolt(){
        weapon.AddBolt();
    }

    void Update() {
        if (dead == true && Time.time < deathTimer){
            return;
        } else if (dead == true && Time.time >= deathTimer){
            //reset world
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
            return;
        }

        int shootInput = (int) Input.GetAxisRaw("Fire1");
        int meleeInput = (int) Input.GetAxisRaw("Fire2");

        switch(curState){
            case PlayerActions.PlayerIdleOrRunning:
                if (meleeInput == 1 && gameTime >= meleeCooldownTimer){
                    SetStateMelee();
                } else if (shootInput == 1 && weapon.loadedBolts > 0){
                    SetStateShoot();
                } else if (weapon.loadedBolts < weapon.currentMax) {
                    reloadDurationTimer = gameTime + reloadDuration;
                    weaponAnimator.SetTrigger("Reload");
                    curState = PlayerActions.PlayerReloading;
                }
                break;
            case PlayerActions.PlayerMelee:
                if (gameTime >= meleeDurationTimer){
                    curState = PlayerActions.PlayerIdleOrRunning;
                }
                break;
            case PlayerActions.PlayerReloading:
                if (gameTime >= reloadDurationTimer){
                    //finished animation without transition
                    weapon.LoadBolt();
                    UI.ChangeWeaponUI(weapon.loadedBolts);
                    curState = PlayerActions.PlayerIdleOrRunning;
                } else if (meleeInput == 1 && gameTime >= meleeCooldownTimer) {
                    SetStateMelee();
                } else if (shootInput == 1 && weapon.loadedBolts > 0){
                    SetStateShoot();
                }
                break;
            case PlayerActions.PlayerShoot:
                if (gameTime >= shootDurationTimer){
                    curState = PlayerActions.PlayerIdleOrRunning;
                }
                break;
            default:
                break;
        }

        gameTime += Time.deltaTime;
    }

    private void SetStateMelee(){
        meleeCooldownTimer = gameTime + meleeCooldown;
        meleeDurationTimer = gameTime + meleeDuration;
        curState = PlayerActions.PlayerMelee;
        playerAnimator.SetTrigger("Melee");
        weaponAnimator.SetTrigger("Melee");
        weapon.Melee();
    }

    private void SetStateShoot(){
        shootDurationTimer = gameTime + shootDuration;
        curState = PlayerActions.PlayerShoot;
        playerAnimator.SetTrigger("Shoot");
        weaponAnimator.SetTrigger("Shoot");
        weapon.Shoot();
    }



    void FixedUpdate() {

        float moveForward = Input.GetAxisRaw("Horizontal");
        float moveRight = Input.GetAxisRaw("Vertical");
        Vector3 inputDirection = camTracker.RemapInputs(moveForward, moveRight);


        //REWRITE
        if (inputDirection.magnitude > 0) {
            //detecting player input
            rb.AddForce(inputDirection * acceleration, ForceMode.Acceleration);

            if (rb.linearVelocity.magnitude > maxSpeed) {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        } else {
            //no player input - slow down
            if (rb.linearVelocity.magnitude > 0.1f) {

                Vector3 frictionForce = -rb.linearVelocity.normalized * friction;
                frictionForce.y = 0; //ignore vertical friction

                if (frictionForce.magnitude >= rb.linearVelocity.magnitude){
                    rb.linearVelocity = Vector3.zero;
                } else {
                    rb.AddForce(frictionForce, ForceMode.Acceleration);
                }
            }
            else {
                rb.linearVelocity = Vector3.zero;
            }
        }

        //FOR ANIMATIONS
        playerAnimator.SetFloat("PlayerSpeed", rb.linearVelocity.magnitude);
    }

    
}
