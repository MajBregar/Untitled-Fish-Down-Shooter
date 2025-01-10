using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

    public float acceleration = 10f;
    public float maxSpeed = 5f;
    public float friction = 5f;

    public float meleeCooldown = 0.5f;
    public float shootCooldown = 0.2f;
    public float meleeDuration = 0.5f;
    public float shootDuration = 0.1f;
    public float reloadDuration = 0.5f;

    private float actionLockTimer = 0f;
    private float meleeCooldownTimer = 0f;
    private float shootCooldownTimer = 0f;
    private float reloadCooldownTimer = 0f;

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

    public void PickupBolt(){
        weapon.AddBolt();
    }

    void Update() {
        //shooting logic

        int shootInput = (int) Input.GetAxisRaw("Fire1");
        int meleeInput = (int) Input.GetAxisRaw("Fire2");

        if (meleeInput == 1 && meleeCooldownTimer < gameTime && actionLockTimer < gameTime) {
            //melee attack
            meleeCooldownTimer = gameTime + meleeCooldown;
            actionLockTimer = gameTime + meleeDuration;
            playerAnimator.SetTrigger("Melee");
            weaponAnimator.SetTrigger("Melee");
            weapon.Melee();

        } else if (shootInput == 1 && shootCooldownTimer < gameTime && actionLockTimer < gameTime && weapon.loadedBolts >= 1){
            //shoot bolt
            playerAnimator.SetTrigger("Shoot");
            weaponAnimator.SetTrigger("Shoot");
            weapon.Shoot();
            shootCooldownTimer = gameTime + shootCooldown;
            actionLockTimer = gameTime + shootDuration;

            UI.ChangeWeaponUI(weapon.loadedBolts);
        } else if (meleeInput == 0 && actionLockTimer < gameTime && weapon.loadedBolts < weapon.currentMax && reloadCooldownTimer < gameTime){
            AnimatorStateInfo stateInfo = weaponAnimator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Weapon Idle")){
                reloadCooldownTimer = gameTime + reloadDuration;
                weaponAnimator.SetTrigger("Reload");
                weapon.LoadBolt();

                UI.ChangeWeaponUI(weapon.loadedBolts);
            }
        }



        //DEBUG GAME RESET - will be used later for death
        if (Input.GetKeyDown(KeyCode.Space)) {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }

        gameTime += Time.deltaTime;
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
