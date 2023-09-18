using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerkUIIconLogic : MonoBehaviour
{
    Image image;

    [SerializeField] Perk perk;

    Color tempColor;

    void Awake() => GetReferences();
    void Start() => InitializeVariables();

    public void ActivatePerkUI()    => gameObject.SetActive(true);
    public void DesactivatePerkUI() => gameObject.SetActive(false);
    void GetReferences()            => image = GetComponent<Image>();
    void InitializeVariables()      => image.sprite = perk.icon;
}
