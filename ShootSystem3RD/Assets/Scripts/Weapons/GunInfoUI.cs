using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GunInfoUI : MonoBehaviour
{
    public Gun gun;
    public TextMeshProUGUI gunName;
    public TextMeshProUGUI amoInfo;
    public TextMeshProUGUI allAmoInfo;
    public Image amoImage;
    public Image icon;

    void Awake()
    {
        gun.SetGunInfo(this);
    }

    void Start()
    {
        Inite(gun);
    }


    public void UpdateInfo() {
        amoInfo.text = gun.GetAmo() + "/" + gun.GetMaxAmo();
        allAmoInfo.text = gun.GetAllAmo().ToString();
        amoImage.fillAmount = ((float)gun.GetAmo() / (float)gun.GetMaxAmo());
    }

    public void Inite(Gun gun)
    {
        this.gun = gun;
        UpdateInfo();
        gunName.text = gun.GetGunName();
        icon.sprite = gun.getIcon();
    }
}
