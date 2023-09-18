using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.SceneManagement;

public class PowerUpLogic : MonoBehaviour
{
    [SerializeField] public PowerUp powerUp;
    MeshRenderer mesh;

    [SerializeField] public static int powerUpExpireTimer = 30;

    float durationCountdown = 0f;

    [SerializeField] float rotationLimitVelocity = 90f;
    float rotationX;
    float rotationY;
    float rotationZ;

    bool meshBlinkActive = false;
    [SerializeField] float slowBlinkTimer = 1;
    [SerializeField] float fastBlinkTimer = 0.5f;
    [SerializeField] float superFastBlinkTimer = 0.2f;
    [SerializeField] float ultraFastBlinkTimer = 0.1f;

    void Awake()
    {
        GetReferences();
    }
    void Start()
    {
        InitializeVariables();
    }

    void LateUpdate()   // Movement
    {
        float X = rotationX * Time.deltaTime;
        float Y = rotationY * Time.deltaTime;
        float Z = rotationZ * Time.deltaTime;

        gameObject.transform.Rotate(X, Y, Z);
    }

    void Update()
    {
        durationCountdown -= Time.deltaTime;

        BlinkingLogic();    // Logic of blinking object mesh depending on time left to pick it up

        if (durationCountdown <= 0)
            Destroy(gameObject); // Destroy if player doesn't pickup
    }

    void BlinkingLogic()    // Blinks less or more depending on "durationCountdown", which determines when object disappear
    {
        if (durationCountdown <= (powerUpExpireTimer / 2) && durationCountdown > (powerUpExpireTimer / 3) && !meshBlinkActive)
            StartCoroutine(MeshBlink(slowBlinkTimer));
        if (durationCountdown <= (powerUpExpireTimer / 3) && durationCountdown > (powerUpExpireTimer / 6) && !meshBlinkActive)
        {
            StopCoroutine(MeshBlink(slowBlinkTimer));
            StartCoroutine(MeshBlink(fastBlinkTimer));
        }
        if (durationCountdown <= (powerUpExpireTimer / 6) && durationCountdown > (powerUpExpireTimer / 9) && !meshBlinkActive)
        {
            StopCoroutine(MeshBlink(fastBlinkTimer));
            StartCoroutine(MeshBlink(superFastBlinkTimer));
        }
        if (durationCountdown <= (powerUpExpireTimer / 9) && !meshBlinkActive)
        {
            StopCoroutine(MeshBlink(superFastBlinkTimer));
            StartCoroutine(MeshBlink(ultraFastBlinkTimer));
        }
    }

    IEnumerator MeshBlink(float timer)
    {
        meshBlinkActive = true;

        mesh.enabled = false;
        yield return new WaitForSeconds(timer);
        mesh.enabled = true;
        yield return new WaitForSeconds(timer);

        meshBlinkActive = false;
    }

    void GetReferences()
    {
        mesh = GetComponent<MeshRenderer>();
    }
    void InitializeVariables()
    {
        durationCountdown = powerUpExpireTimer;

        float negativeRotation = Random.Range(-rotationLimitVelocity, -0.1f);
        float positiveRotation = Random.Range(rotationLimitVelocity, 0.1f);

        rotationX = Random.Range(negativeRotation, positiveRotation);
        rotationY = Random.Range(negativeRotation, positiveRotation);
        rotationZ = Random.Range(negativeRotation, positiveRotation);
    }
}
