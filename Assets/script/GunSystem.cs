using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunSystem : MonoBehaviour
{
    [Header("References")]
    public Camera playerCam;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    [Header("Recoil Settings")]
    public float RecoilX = 2;
    public float RecoilY = 1;
    public float RecoilZ = 0.2f;
    public float recoilReturnSpeed = 5;
    public float floatrecoilSnapSpeed=10;

    [Header("Stats")]
    public float fireRange = 100f;
    public int damage = 25;
    public int maxAmmo = 10;
    public float reloadTime = 2;
    private int currentAmmo;
    private bool isReloading = false;

     [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip gunShotSound;

    private PlayerInputActions inputActions;
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        currentAmmo = maxAmmo;
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.player.shoot.performed += _ => TryFire();
        inputActions.player.reload.performed += _ => StartReload();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    void Update()
    {
        HandleRecoil();
    }

    void HandleRecoil()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, recoilReturnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, recoilReturnSpeed * Time.deltaTime);
        playerCam.transform.localEulerAngles += currentRotation * Time.deltaTime;
    }

    void TryFire()
    {
        if (isReloading || currentAmmo <= 0)
        {
            Debug.Log("cant shoot " + (isReloading ? "reloading" : "out of ammo"));
            return;
        }
        Fire();
    }

    private void Fire()
    {

        ApplyRecoil();
        currentAmmo--;
        Debug.Log("shots left= " + currentAmmo);


        // Play muzzle flash
        if (muzzleFlash != null)
            muzzleFlash.Play();
        audioSource.PlayOneShot(gunShotSound);

        Vector3 shootDirection = playerCam.transform.forward;

        shootDirection += new Vector3(Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f), 0);

        // Perform raycast
        Ray ray = new Ray(playerCam.transform.position, shootDirection.normalized);
        if (Physics.Raycast(ray, out RaycastHit hit, fireRange))
        {
            Debug.Log("Hit: " + hit.collider.name);

            // Apply damage
            if (hit.collider.TryGetComponent(out EnemyHealth enemy))
            {
                enemy.TakeDamage(damage);
            }

            // Show impact effect
            if (impactEffect != null)
            {
                GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 1f);
            }
        }
    }

    void ApplyRecoil()
    {
        float RecoilYValue = Random.Range(-RecoilY, RecoilY);
        targetRotation += new Vector3(RecoilX, RecoilYValue, RecoilZ);
    }

    public void ResetRecoil()
{
    currentRotation = Vector3.zero;
    targetRotation = Vector3.zero;
}


    void StartReload()
    {
        if (currentAmmo < maxAmmo && !isReloading)
        {
            StartCoroutine(Reload());
            ResetRecoil();
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("reloading");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log("reloaded");
    }

    public int GetCurrentAmmo() => currentAmmo;
    public int GetMaxAmmo() => maxAmmo;
    public bool IsReloading() => isReloading;
}
