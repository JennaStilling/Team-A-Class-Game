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
    weaponTypes[] types;
    private class weaponTypes
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
    public int blasterShots;

    private void changeMode(WeaponModes mode) 
    {

        switch (mode)
        {
            case WeaponModes.Sword:
                playerRaycast.damage = types[0].damage;
                playerRaycast.fireRate = types[0].damage;
                playerRaycast.weaponRange = types[0].damage;
                break;
            case WeaponModes.Blaster:
                playerRaycast.damage = types[1].damage;
                playerRaycast.fireRate = types[1].damage;
                playerRaycast.weaponRange = types[1].damage;
                //figure out what to do with ammo, maybe shoot through things
                break;
        }
    }

    void Start()
    {
        playerRaycast = FindObjectOfType<RayCast>();
    }
}
