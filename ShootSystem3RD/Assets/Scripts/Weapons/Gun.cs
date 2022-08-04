using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Gun : MonoBehaviour
{
    [SerializeField]
    protected string gunName;

    [SerializeField]
    protected Sprite icon;

    public Transform shootOut;
    public Transform LHandTarget;

    [SerializeField]
    protected int maxAmo;
    [SerializeField]
    protected int amo;
    [SerializeField]
    protected int allAmo;
    [SerializeField]
    protected float fireRate = 0.1f;

    [Range(0f, 10f)]
    [SerializeField]
    protected float normalRecoil;
    [Range(0f, 10f)]
    [SerializeField]
    protected float aimRecoil;
    protected float recoil;
    protected GunInfoUI gunInfo;
    protected bool isAmo = true;
    public Crosshair crosshair;

    protected AudioSource audioSource;
    [SerializeField]
    protected AudioClip reloadClip;
    [SerializeField]
    protected RectTransform hitCrosshair;
    private RectTransform canvasRect;
    [SerializeField]
    protected LayerMask hitCrosshairLayer;
    public bool isActive = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = reloadClip;
        canvasRect = hitCrosshair.parent.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (!isActive) return;

        if (Input.GetMouseButton(0))
        {
            recoil = Mathf.Lerp(recoil, Input.GetMouseButton(1) ? aimRecoil : normalRecoil, Time.deltaTime);
        }
        else {
            recoil = Mathf.Lerp(recoil, 0f, Time.deltaTime * 5f);
        }
        if (!isAmo) {
            recoil = Mathf.Lerp(recoil, 0f, Time.deltaTime * 5f);
        }
        crosshair.recoil = recoil;

        RaycastHit hit;
        if (Physics.Raycast(shootOut.position, shootOut.forward, out hit, 3f, hitCrosshairLayer)) {
            hitCrosshair.gameObject.SetActive(true);

            Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(hit.point);
            Vector2 screenPos = new Vector2(
            ((ViewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
            ((ViewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));
            hitCrosshair.anchoredPosition = screenPos;

        }
        else
        {
            hitCrosshair.gameObject.SetActive(false);
        }


        }
    public virtual void Shoot() {
    
    }


    public void StartReload() {
        audioSource.Play();
    }

    public void EndReload() {
        int firedAmo = maxAmo - amo;
        firedAmo = (firedAmo > allAmo) ? allAmo : firedAmo;
        allAmo-= firedAmo;
        amo += firedAmo;
        if (gunInfo){
            gunInfo.UpdateInfo();
        }
        isAmo = true;
    }

    public int GetAmo(){
        return amo;
    }
    public int GetMaxAmo()
    {
        return maxAmo;
    }
    public int GetAllAmo()
    {
        return allAmo;
    }

    public bool CheckAmo() {
        return isAmo;
    }

    public string GetGunName() {
        return gunName;
    }

    public Sprite getIcon() {
        return icon;
    }

    public void SetGunInfo(GunInfoUI gunInfo) {
        this.gunInfo = gunInfo;
    }
}
