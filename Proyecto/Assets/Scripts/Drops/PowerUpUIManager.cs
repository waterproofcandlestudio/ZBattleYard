using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpUIManager : MonoBehaviour
{
    [SerializeField] GameObject instaKillUI;
    [SerializeField] GameObject doublePointsUI;
    [SerializeField] GameObject magicBoxUI;
    [SerializeField] GameObject slowTimeUI;

    public void AddPowerUpImageFeedback(PowerUp powerUpMan)
    {
        PowerType type = powerUpMan.powerType;
        float duration = powerUpMan.duration;

        if (type == PowerType.instaKill)
        {
            if (instaKillUI.activeSelf)
                instaKillUI.GetComponentInChildren<PowerUpUIIconLogic>().ResetBlinkTimer();
            else
                ActivatePowerUpUI(instaKillUI);
        }

        if (type == PowerType.doublePoints)
        {
            if (doublePointsUI.activeSelf)
                doublePointsUI.GetComponentInChildren<PowerUpUIIconLogic>().ResetBlinkTimer();
            else
                ActivatePowerUpUI(doublePointsUI);
        }

        if (type == PowerType.magicBoxDiscount)
        {
            if (magicBoxUI.activeSelf)
                magicBoxUI.GetComponentInChildren<PowerUpUIIconLogic>().ResetBlinkTimer();
            else
                ActivatePowerUpUI(magicBoxUI);
        }

        if (type == PowerType.slowTime)
        {
            if (slowTimeUI.activeSelf)
                slowTimeUI.GetComponentInChildren<PowerUpUIIconLogic>().ResetBlinkTimer();
            else
                ActivatePowerUpUI(slowTimeUI);
            //IntantiateInUI(slowTimeUI, powerUpMan, duration);
            //Destroy(slowTimeUI, duration);
        }

        else
            return;
    }

    void ActivatePowerUpUI(GameObject gameObjectUI)
    {
        gameObjectUI.GetComponentInChildren<PowerUpUIIconLogic>().ActivateBlinkingCode();
    }
}
