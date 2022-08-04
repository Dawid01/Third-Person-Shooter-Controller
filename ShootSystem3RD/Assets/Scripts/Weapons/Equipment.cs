using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Equipment : MonoBehaviour
{
    public enum WeaponType
    {
        Rifle = 0,
        Shotgun = 1
    }

    public WeaponType weaponType = WeaponType.Rifle;
    public Gun rifle;
    //public MultiAimConstraint rifleRig;
    public Vector3 rifleHandOffset;
    public Vector3 rifleHandRotOffset;

    public Gun shotgun;
    public Vector3 shotgunHandOffset;
    public Vector3 shotgunHandRotOffset;

    private Gun usingGun;


    public Transform rHand;
    public Transform socket;
    private PlayerMovement playerMovement;
    public Transform lHandTarget;
    public GunInfoUI gunInfo;
    public Rig rifleRig;
    public Rig shotGunRig;

    void Start()
    {
        playerMovement = transform.parent.GetComponent<PlayerMovement>();
    }


    void Update()
    {
        
    }

    public void ChangeWeapon() {

        weaponType = (weaponType == WeaponType.Rifle) ? WeaponType.Shotgun : WeaponType.Rifle;
        if (weaponType == WeaponType.Rifle) {
            usingGun = rifle;
            rifle.isActive = true;
            shotgun.isActive = false;
            rifle.transform.parent = rHand;
            rifle.transform.localPosition = rifleHandOffset;
            rifle.transform.localEulerAngles = rifleHandRotOffset;
            rifleRig.weight = 1f;
            shotGunRig.weight = 0f;
            shotgun.transform.parent = socket;
            shotgun.transform.localPosition = Vector3.zero;
            shotgun.transform.localEulerAngles = Vector3.zero;

            playerMovement.WeaponRig = rifleRig;

        }
        else
        {
            usingGun = shotgun;
            rifle.isActive = false;
            shotgun.isActive = true;
            shotgun.transform.parent = rHand;
            shotgun.transform.localPosition = shotgunHandOffset;
            shotgun.transform.localEulerAngles = shotgunHandRotOffset;
            shotGunRig.weight = 1f;
            rifleRig.weight = 0f;
            rifle.transform.parent = socket;
            rifle.transform.localPosition = Vector3.zero;
            rifle.transform.localEulerAngles = Vector3.zero;



            playerMovement.WeaponRig = shotGunRig;


        }

        playerMovement.usingGun = usingGun;
        
        lHandTarget.parent = usingGun.LHandTarget;
        lHandTarget.localPosition = Vector3.zero;
        lHandTarget.localEulerAngles = Vector3.zero;
        gunInfo.Inite(usingGun);
    }
}
