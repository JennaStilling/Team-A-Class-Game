using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public enum WeaponModes
    {
        Sword,
        Blaster
    }

    [SerializeField]
    public weaponTypes[] types;
    [System.Serializable]
    public class weaponTypes
    {
        public float damage;
        public float fireRate;
        public float weaponRange;
    }

    private WeaponModes _currentweaponMode = WeaponModes.Sword;
    public WeaponModes CurrentWeaponModeProp
    {
        get { return _currentweaponMode; }
        set { _currentweaponMode = value; changeMode(_currentweaponMode); }
    }

    private RayCast playerRaycast;
    public int blasterShots = 3;

    //public bool[] weaponUnlocks;
    public bool gunUnlocked = true;

    private void changeMode(WeaponModes mode) 
    {

        switch (mode)
        {
            case WeaponModes.Sword:
                playerRaycast.damage = types[0].damage;
                playerRaycast.fireRate = types[0].fireRate;
                playerRaycast.weaponRange = types[0].weaponRange;
                playerRaycast.isRederingLine = false;
                break;
            case WeaponModes.Blaster:
                playerRaycast.damage = types[1].damage;
                playerRaycast.fireRate = types[1].fireRate;
                playerRaycast.weaponRange = types[1].weaponRange;
                playerRaycast.isRederingLine = true;
                //figure out what to do with ammo, maybe shoot through things
                break;
        }
    }

    void Start()
    {
        playerRaycast = FindObjectOfType<RayCast>();
    }

    private void Update()
    {
        if (gunUnlocked && Input.GetKeyDown(KeyCode.Alpha2))
        {

            CurrentWeaponModeProp = WeaponModes.Blaster;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            CurrentWeaponModeProp = WeaponModes.Sword;
        }
    }
}
