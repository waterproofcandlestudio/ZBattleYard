using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaProgressBar : MonoBehaviour
{
    float baseValue;
    float maxValue;

    [SerializeField] Image fillImage;

    #region - Awake -

    void Awake() => GetReferences();

    #endregion

    public void SetValues(float _baseValue, float _maxValue)    // Establishes max & min values to fill the progress bar
    {
        baseValue = _baseValue;
        maxValue = _maxValue;
        CalculateFillAmount();
    }

    void CalculateFillAmount()
    {
        float fillAmount = baseValue / maxValue;
        fillImage.fillAmount = fillAmount;
    }

    #region - Reference -

    void GetReferences()
    {
        fillImage = GetComponentInChildren<Image>();
    }

    #endregion
}
