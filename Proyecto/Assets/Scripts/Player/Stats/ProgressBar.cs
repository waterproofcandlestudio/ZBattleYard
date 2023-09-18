using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    int baseValue;
    int maxValue;

    Slider fill;
    Image fillImage;
    //[SerializeField] Image fill;
    //[SerializeField] Text amount;

    #region - Awake -

    void Awake()
    {
        GetReferences();
    }

    #endregion

    public void SetValues(int _baseValue, int _maxValue)
    {
        baseValue = _baseValue;
        maxValue = _maxValue;

        //amount.text = baseValue.ToString();

        CalculateFillAmount();
    }

    void CalculateFillAmount()
    {
        float fillAmount = (float)baseValue / (float)maxValue;
        //fill.fillAmount = fillAmount;
        fill.value = fillAmount;
    }

    #region - Reference && Initialize -

    void GetReferences()
    {
        fill = GetComponentInChildren<Slider>();
        fillImage = GetComponentInChildren<Image>();
    }
    void InitVariables()
    {
        //staminaBar.maxValue = maxStamina; // Stamina
    }

    #endregion
}
