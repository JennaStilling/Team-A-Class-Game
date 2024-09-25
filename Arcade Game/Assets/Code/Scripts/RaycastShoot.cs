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


    void Start () 
    {
        laserLine = GetComponent<LineRenderer>();


        fpsCam = GetComponentInParent<Camera>();
    }


    void Update () 
    {
        if (Input.GetButtonDown("Fire1") && Time.time > nextFire) 
        {
            nextFire = Time.time + fireRate;

            StartCoroutine (ShotEffect());

            Vector3 rayOrigin = fpsCam.ViewportToWorldPoint (new Vector3(0.5f, 0.5f, 0.0f));

            RaycastHit hit;

            laserLine.SetPosition (0, gunEnd.position);

            if (Physics.Raycast (rayOrigin, fpsCam.transform.forward, out hit, weaponRange))
            {
                laserLine.SetPosition (1, hit.point);

                EmployeeEnemyManager health = hit.collider.GetComponent<EmployeeEnemyManager>();

                if (health != null)
                {
                    health.TakeDamage(damage);
                }

               
            }
            else
            {
                laserLine.SetPosition (1, rayOrigin + (fpsCam.transform.forward * weaponRange));
            }
        }
    }


    private IEnumerator ShotEffect()
    {
       
       // laserLine.enabled = true;

        yield return shotDuration;

        laserLine.enabled = false;
    }

}
