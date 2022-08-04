using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : Gun
{

    public ParticleSystem shootParticle;
    public ParticleSystem shellParticle;
    public TrailRenderer shootTrial;
    public GameObject[] shootHoles;
    bool canShoot = true;



    public override void Shoot()
    {
        base.Shoot();
        if (canShoot && isAmo)
        {
            StartCoroutine(ShootCoroutine(fireRate));
        }

    }

    private IEnumerator ShootCoroutine(float shootTime)
    {
        canShoot = false;
        yield return new WaitForSeconds(shootTime);
        canShoot = true;
        shootParticle.Play();
        shellParticle.Play();
        TrailRenderer trail = Instantiate(shootTrial, shootOut.position, Quaternion.identity);
        amo--;
        if (amo <= 0)
        {
            isAmo = false;
        }
        gunInfo.UpdateInfo();
        RaycastHit hit;
        Quaternion recoilRotation = Quaternion.AngleAxis(Random.RandomRange(-recoil, recoil), transform.up) * Quaternion.AngleAxis(Random.RandomRange(-recoil, recoil), transform.right);
        bool isHit = Physics.Raycast(shootOut.position, recoilRotation * shootOut.forward * 1000f, out hit);
        if (isHit)
        {

            StartCoroutine(SpawnTrail(trail, hit, isHit));
        }
        else {
            hit.point = shootOut.forward * 100f;
            StartCoroutine(SpawnTrail(trail, hit, isHit));
        }
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit, bool isHit) {

        float time = 0;
        Vector3 startPosition = trail.transform.position;

        while (time < 1) {

            trail.transform.position = Vector3.MoveTowards(startPosition, hit.point, time * 20f);
            time += Time.deltaTime / trail.time;
            yield return null;
        }
        if (isHit)
        {
            GameObject holePrefab = null;
            trail.transform.position = hit.point;
            Destroy(trail.gameObject, trail.time);
            string tag = hit.transform.tag;
            switch (tag)
            {
                case "Enemy":
                    holePrefab = shootHoles[1];
                    crosshair.HitEnemy();
                    break;
                case "InvisibleWall":
                    break;
                default:
                    holePrefab = shootHoles[0];
                    break;
            }
            if (holePrefab)
            {
                GameObject hole = Instantiate(holePrefab, hit.point, Quaternion.LookRotation(hit.normal));
                hole.transform.parent = hit.transform;
            }
            try
            {
                Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
                rb.AddForce(-hit.normal * 10f, ForceMode.Impulse);
            }
            catch { }
        }
    }

}
