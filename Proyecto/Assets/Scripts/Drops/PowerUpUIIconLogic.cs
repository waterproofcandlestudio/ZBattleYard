using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpUIIconLogic : MonoBehaviour
{
    Image image;

    [SerializeField] PowerUp powerUp;

    float durationCountdown = 0f;
    bool meshBlinkActive = false;

    [SerializeField] float slowBlinkTimer = 1;
    [SerializeField] float fastBlinkTimer = 0.5f;
    [SerializeField] float superFastBlinkTimer = 0.2f;
    [SerializeField] float ultraFastBlinkTimer = 0.1f;

    bool activateBlinkingCode = false;

    Color tempColor;

    void Awake()
    {
        GetReferences();
    }
    void Start()
    {
        InitializeVariables();
    }

    void Update()
    {
        if (activateBlinkingCode)
        {
            durationCountdown -= Time.deltaTime;

            BlinkingLogic();

            if (durationCountdown <= 0) // Desactivates powerUp UI & functionality when timer == 0
            {
                DesactivatePowerUp();
                gameObject.SetActive(false);
            }
        }
        else
            return;
    }

    public void ActivateBlinkingCode()
    {
        gameObject.SetActive(true);
        activateBlinkingCode = true;
        meshBlinkActive = false;
        durationCountdown = powerUp.duration;
        ChangeImageAlpha(1f);
    }
    public void ResetBlinkTimer()   // Reset powerUp timer if i pick up another from the same kind
    {
        durationCountdown = powerUp.duration;
        ChangeImageAlpha(1f);
    }

    void BlinkingLogic()    // Blinks less or more depending on "durationCountdown", which determines when object disappear
    {
        if (durationCountdown <= (powerUp.duration / 2) && durationCountdown > (powerUp.duration / 3) && !meshBlinkActive)
            StartCoroutine(MeshBlink(slowBlinkTimer));
        if (durationCountdown <= (powerUp.duration / 3) && durationCountdown > (powerUp.duration / 6) && !meshBlinkActive)
        {
            StopCoroutine(MeshBlink(slowBlinkTimer));
            StartCoroutine(MeshBlink(fastBlinkTimer));
        }
        if (durationCountdown <= (powerUp.duration / 6) && durationCountdown > (powerUp.duration / 9) && !meshBlinkActive)
        {
            StopCoroutine(MeshBlink(fastBlinkTimer));
            StartCoroutine(MeshBlink(superFastBlinkTimer));
        }
        if (durationCountdown <= (powerUp.duration / 9) && !meshBlinkActive)
        {
            StopCoroutine(MeshBlink(superFastBlinkTimer));
            StartCoroutine(MeshBlink(ultraFastBlinkTimer));
        }
    }

    void DesactivatePowerUp()
    {
        // Reset blinking
        meshBlinkActive = true;
        activateBlinkingCode = false;

        PowerType type = powerUp.powerType;

        if (type == PowerType.instaKill)
            PowerUpManager.instaKillActive = false;

        if (type == PowerType.doublePoints)
            PowerUpManager.doublePointsActive = false;

        if (type == PowerType.magicBoxDiscount)
            PowerUpManager.magicBoxSaleActive = false;

        if (type == PowerType.slowTime)
            PowerUpManager.slowTimeActive = false;

        else
            return;
    }
    
    IEnumerator MeshBlink(float timer)
    {
        meshBlinkActive = true;

        ChangeImageAlpha(0f);
        yield return new WaitForSeconds(timer);
        ChangeImageAlpha(1f);
        yield return new WaitForSeconds(timer);

        meshBlinkActive = false;
    }

    void ChangeImageAlpha(int alpha)
    {
        tempColor = image.color;
        tempColor.a = alpha;
        image.color = tempColor;
    }
    void ChangeImageAlpha(float alpha)
    {
        tempColor = image.color;
        tempColor.a = alpha;
        image.color = tempColor;
    }

    void GetReferences()
    {
        image = GetComponent<Image>();
    }

    void InitializeVariables()
    {
        image.sprite = powerUp.icon;
    }
}
