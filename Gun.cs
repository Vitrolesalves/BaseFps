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

    
    // Variaveis temporarias

    int _ammoInClip;
    int _ammoInInventory;
    bool _canShoot;

    // Muzzle Flash

    public Image flashMuzzleImage;
    public Sprite[] flashes;

    // Aim

    public Vector3 normallocalPosition;
    public Vector3 aimlocalPosition;

    public float aimSmooth;

    // Recoil

    public float test;

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


    private void Start()
    {
        _ammoInClip = ammoInClip;
        _ammoInInventory = ammoInInventory;
        _canShoot = true;

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
        else if (Input.GetKey(KeyCode.R) && _ammoInClip < ammoInClip & _ammoInInventory > 0)
        {
            
            int _needReload = ammoInClip - _ammoInClip;

            if (_needReload >= _ammoInInventory)
            {
                _ammoInClip += _ammoInInventory;
                _ammoInInventory -= _needReload;
            }
            else
            {
                _ammoInClip = ammoInClip;
                _ammoInInventory -= _needReload;
            }
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

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
        {
            Debug.Log("Acertou" +  hit.transform.name);

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

}
