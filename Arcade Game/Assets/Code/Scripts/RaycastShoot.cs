using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCast : MonoBehaviour
{
    public float damage = 10f;                                           
    public float fireRate = 0.25f;                                       
    public float weaponRange = .5f;                                        
    public Transform gunEnd;                                            
    private Camera fpsCam;                                                
    private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);    
    private AudioSource gunAudio;                                        
    private LineRenderer laserLine;                                        
    private float nextFire;

    public bool isRederingLine = false;
    private WeaponManager weaponManager;

    private PlayerMovement playerMovement;

    void Start () 
    {
        laserLine = GetComponent<LineRenderer>();
        fpsCam = GetComponentInParent<Camera>();

        weaponManager = FindObjectOfType<WeaponManager>();
        playerMovement = FindObjectOfType<PlayerMovement>();
    }


    void Update () 
    {
        if (playerMovement.CanMoveProp)
        {
            if (Input.GetButtonDown("Fire1") && Time.time > nextFire)
            {
                if (weaponManager.CurrentWeaponModeProp == WeaponManager.WeaponModes.Blaster)
                {
                    if (GameManager.Instance.BlasterShotsProp <= 0)
                    {
                        return;
                    }
                    GameManager.Instance.BlasterShotsProp--;
                }

                nextFire = Time.time + fireRate;

                StartCoroutine(ShotEffect());

                Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

                RaycastHit hit;

                laserLine.SetPosition(0, gunEnd.position);

                if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, weaponRange))
                {
                    laserLine.SetPosition(1, hit.point);
                    EmployeeEnemyManager health = hit.collider.GetComponent<EmployeeEnemyManager>();

                    if (health != null)
                    {
                        health.TakeDamage(damage);
                    }


                }
                else
                {
                    laserLine.SetPosition(1, rayOrigin + (fpsCam.transform.forward * weaponRange));
                }
            }
        }
    }


    private IEnumerator ShotEffect()
    {
       if (isRederingLine)
            laserLine.enabled = true;

        yield return shotDuration;

        laserLine.enabled = false;
    }

}
