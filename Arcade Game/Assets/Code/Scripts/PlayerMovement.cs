using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using UnityEngine.SceneManagement;



[RequireComponent(typeof(CharacterController))]

public class PlayerMovement : MonoBehaviour

{

    public Camera playerCamera;

    public float walkSpeed = 4f;

    public float runSpeed = 6f;

    public float jumpPower = 5f;

    public float gravity = 8f;

    public float lookSpeed = 2f;

    public float lookXLimit = 45f;

    public float defaultHeight = 2f;

    public float crouchHeight = 1f;

    public float crouchSpeed = 3f;



    private Vector3 moveDirection = Vector3.zero;

    private float rotationX = 0;

    private CharacterController characterController;
    private int currentHealth = 100;  
    
    public int CurrentHealthProp 
    {
        get { return currentHealth; }
        set {
            currentHealth = value;
        }
    }

    public Vector3 respawnPosition;
    public Quaternion respawnRotation;

    public float respawnDelay = 2f;      
    public float damageCooldown = 1f;      

    private bool _isDead = false;            
    private bool canTakeDamage = true;      


    private bool _canMove = true;
    public bool CanMoveProp 
    {
        get { return _canMove; }
        set {
            _canMove = value;
        }
    }


    void Start()

    {
        Debug.Log("Player movement script has loaded");
        characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;

        Cursor.visible = false;

        currentHealth = 100;

        respawnPosition = transform.position;
        respawnRotation = transform.rotation;
    }


    public void TakeDamage(int damage)
    {
        if (_isDead || !canTakeDamage)
            return;

        currentHealth -= damage;
        Debug.Log("Player took damage, current health: " + currentHealth);

        canTakeDamage = false;
        Invoke("ResetDamageCooldown", damageCooldown);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

     private void ResetDamageCooldown()
    {
        canTakeDamage = true;
    }

    private void Die()
    {
        Debug.Log("Player died!");
        _isDead = true;
        _canMove = false;
        Invoke("Respawn", respawnDelay);
    }

    void Respawn()
    {
        currentHealth = 100;
        _isDead = false;

        characterController.enabled = false;
        transform.position = respawnPosition;
        transform.rotation = respawnRotation;
        characterController.enabled = true;

        _canMove = true;
    }



    void Update()

    {

        Vector3 forward = transform.TransformDirection(Vector3.forward);

        Vector3 right = transform.TransformDirection(Vector3.right);



        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        float curSpeedX = _canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;

        float curSpeedY = _canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;

        float movementDirectionY = moveDirection.y;

        moveDirection = (forward * curSpeedX) + (right * curSpeedY);



        if (Input.GetButton("Jump") && _canMove && characterController.isGrounded)

        {

            moveDirection.y = jumpPower;

        }

        else

        {

            moveDirection.y = movementDirectionY;

        }



        if (!characterController.isGrounded)

        {

            moveDirection.y -= gravity * Time.deltaTime;

        }



        if (Input.GetKey(KeyCode.LeftControl) && _canMove)

        {

            characterController.height = crouchHeight;

            walkSpeed = crouchSpeed;

            runSpeed = crouchSpeed;



        }

        else

        {

            characterController.height = defaultHeight;

            walkSpeed = 4f;

            runSpeed = 6f;

        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (_canMove)

        {    


            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;

            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
            //Debug.Log("moving");

        }
    }
}