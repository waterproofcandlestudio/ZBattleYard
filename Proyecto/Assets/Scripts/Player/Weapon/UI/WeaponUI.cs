using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI gunName;
    [SerializeField] TextMeshProUGUI magazineSizeText;
    [SerializeField] TextMeshProUGUI storedAmmoText;

    public void UpdateInfo(string name, Sprite weaponIcon, int magazineSize, int storedAmmo)
    {
        gunName.text = name;
        icon.sprite = weaponIcon;
        magazineSizeText.text = magazineSize.ToString();
        storedAmmoText.text = storedAmmo.ToString();
    }

    public void UpdateAmmoUI(int magazineSize, int storedAmmo)
    {
        magazineSizeText.text = magazineSize.ToString();
        storedAmmoText.text = storedAmmo.ToString();
    }
}
