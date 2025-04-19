using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{

    [Header("Gun Stats")]

    public int ammoInClip = 30;
    public float fireRate = 0.1f;
    public int ammoInInventory = 270;
    public float reloadCooldowm = 1.3f;
    public int attackDamageInBody = 25;
    public int attackDamageInHead = 100;

    
    // Variaveis temporarias

    int _ammoInClip;
    int _ammoInInventory;
    bool _canShoot;
    float _reloadCooldown;
    bool isReloading = false;

    // Muzzle Flash

    public Image flashMuzzleImage;
    public Sprite[] flashes;

    // Aim

    public Vector3 normallocalPosition;
    public Vector3 aimlocalPosition;

    public float aimSmooth;

    // Recoil

    [Header("Recoil")]
    public float recoilAmount = 0.1f;
    public float recoilReturnSpeed = 5f;
    public float recoilSnappiness = 10f;

    [HideInInspector] public Vector3 currentRecoilOffset;
    [HideInInspector] public Vector3 targetRecoilOffset;

    [Header("Camera Shake")]
    public Cinemachine.CinemachineVirtualCamera virtualCam;
    public float shakeDuration = 0.1f;
    public float shakeAmplitude = 1.2f;
    public float shakeFrequency = 2f;

    float shakeTimer;
    Cinemachine.CinemachineBasicMultiChannelPerlin camNoise;


    // Animação

    [Header("Animação")]
    public Animator animator;


    private void Start()
    {
        _ammoInClip = ammoInClip;
        _ammoInInventory = ammoInInventory;
        _canShoot = true;
        _reloadCooldown = reloadCooldowm;

        if (virtualCam != null)
            camNoise = virtualCam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();

    }

    private void Update()
    {
        DetermineAim();

        UpdateCameraShake();

        if (Input.GetMouseButton(0) && _canShoot && _ammoInClip > 0)
        {
            _canShoot = false;
            _ammoInClip--;
            StartCoroutine(Shoot());
        }
        else if (Input.GetKeyDown(KeyCode.R) && !isReloading && _ammoInClip < ammoInClip && _ammoInInventory > 0)
        {
            StartCoroutine(Reload());
        }



    }


    void StartCameraShake()
    {
        if (camNoise == null) return;

        camNoise.m_AmplitudeGain = shakeAmplitude;
        camNoise.m_FrequencyGain = shakeFrequency;
        shakeTimer = shakeDuration;
    }

    void UpdateCameraShake()
    {
        if (camNoise == null || shakeTimer <= 0f) return;

        shakeTimer -= Time.deltaTime;
        if (shakeTimer <= 0f)
        {
            camNoise.m_AmplitudeGain = 0f;
            camNoise.m_FrequencyGain = 0f;
        }
    }


    void DetermineAim()
    {
        Vector3 target = normallocalPosition;
        if (Input.GetMouseButton(1)) target = aimlocalPosition;

        // Suaviza o recoil
        targetRecoilOffset = Vector3.Lerp(targetRecoilOffset, Vector3.zero, recoilReturnSpeed * Time.deltaTime);
        currentRecoilOffset = Vector3.Lerp(currentRecoilOffset, targetRecoilOffset, recoilSnappiness * Time.deltaTime);

        // Combina mira + recoil
        Vector3 desiredPosition = Vector3.Lerp(transform.localPosition, target, Time.deltaTime * aimSmooth);
        transform.localPosition = desiredPosition + currentRecoilOffset;
    }



    void DetermineRecoil()
    {
        float randomX = Random.Range(-0.05f, 0.05f);
        targetRecoilOffset += new Vector3(randomX, recoilAmount * 0.5f, -recoilAmount);

        StartCameraShake();
    }




    IEnumerator Shoot()
    {
        DetermineRecoil();
        
        StartCoroutine(MuzzleFlash());

        RayCastForEnemy();
        yield return new WaitForSeconds(fireRate);
        _canShoot = true;
    }

    void RayCastForEnemy()
    {
        
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 100000,LayerMask.GetMask("Shooting")))
        {
            Debug.Log("Acertou" +  hit.transform.name);

            Debug.Log("Tag do objeto: " + hit.transform.tag);

            if (hit.transform.tag == "EnemyBody")
            {
                hit.transform.root.GetComponent<ZombieController>().heathEnemy -= attackDamageInBody;
                Debug.Log(hit.transform.GetComponent<ZombieController>().heathEnemy);
            }
            else if (hit.transform.tag == "EnemyHead")
            {
                hit.transform.root.GetComponent<ZombieController>().heathEnemy -= attackDamageInHead;
                Debug.Log(hit.transform.GetComponent<ZombieController>().heathEnemy);
            }


        } 
    }

    IEnumerator MuzzleFlash()
    {
        flashMuzzleImage.sprite = flashes[Random.Range(0, flashes.Length)];
        flashMuzzleImage.color = Color.white;
        yield return new WaitForSeconds(0.05f);
        flashMuzzleImage.sprite = null;
        flashMuzzleImage.color = new Color(0, 0, 0, 0);

    }

    IEnumerator Reload()
    {
        isReloading = true;
        _canShoot = false;

        // tocar a animação de reload aqui NÃO ESQUECER
        // exemplo: animator.SetTrigger("Reload");
        animator.SetTrigger("Reload");

        yield return new WaitForSeconds(reloadCooldowm);

        int _needReload = ammoInClip - _ammoInClip;

        if (_needReload >= _ammoInInventory)
        {
            _ammoInClip += _ammoInInventory;
            _ammoInInventory = 0;
        }
        else
        {
            _ammoInClip = ammoInClip;
            _ammoInInventory -= _needReload;
        }

        _canShoot = true;
        isReloading = false;
    }

}
