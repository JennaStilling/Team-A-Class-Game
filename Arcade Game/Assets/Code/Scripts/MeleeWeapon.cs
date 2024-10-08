using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Observations;

public class MeleeWeapon : Subject, PlayerIObserver
{
    public float swingSpeed = 5f; // Speed of the swing
    public float swingAngle = 160f;  // Maximum angle of the swing
    private bool isSwinging = false;
    private float currentSwingTime = 0f;
    private Vector3 initialRotation;
    [SerializeField] private float _damage = 10f;
    public float damageProp
    {
        get { return _damage; }
        set { _damage = value; }
    }

    private bool returning = false;

    private WeaponManager weaponManager;      // Reference to the WeaponManager to check the current weapon mode
    private PlayerMovement playerMovement;     

    void Start()
    {
        initialRotation = transform.localEulerAngles;
        weaponManager = FindObjectOfType<WeaponManager>();
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    void Update()
    {
        if (playerMovement.CanMoveProp && weaponManager.CurrentWeaponModeProp == WeaponManager.WeaponModes.Sword)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && !isSwinging)
            {
                isSwinging = true;
                returning = false;
                currentSwingTime = 0f;
            }

            if (isSwinging)
            {
                SwingWeapon();
            }
        }
        else
        {
            ResetWeaponRotation();
        }
    }

    // Switched to raycast damage
    // void OnTriggerEnter(Collider other)
    // {
    //     if(isSwinging)
    //     {
    //         if (other.CompareTag("Employee"))
    //         {
    //             other.GetComponent<EmployeeEnemyManager>().TakeDamage(_damage);
    //         }
    //     }
    // }

    void SwingWeapon()
    {
        currentSwingTime += Time.deltaTime * swingSpeed;

        if (!returning)
        {
            float angle = Mathf.Lerp(swingAngle, -swingAngle, currentSwingTime);
            transform.localEulerAngles = new Vector3(initialRotation.x, initialRotation.y + angle, initialRotation.z);

            if (currentSwingTime >= 1f)
            {
                currentSwingTime = 0f;
                returning = true;
            }
        }
        else
        {
            float angle = Mathf.Lerp(-swingAngle, swingAngle, currentSwingTime);
            transform.localEulerAngles = new Vector3(initialRotation.x, initialRotation.y + angle, initialRotation.z);

            if (currentSwingTime >= 1f)
            {
                transform.localEulerAngles = initialRotation; 
                isSwinging = false;
                returning = false;
            }
        }
    }

    void ResetWeaponRotation()
    {
        if (isSwinging)
        {
            transform.localEulerAngles = initialRotation; 
            isSwinging = false; 
            currentSwingTime = 0f; 
            returning = false; 
        }
    }

    public void OnNotify(EmployeeEnemyManager enemy)
    {
        AddObserver(enemy);
    }
}
