using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerkUIManager : MonoBehaviour
{
    [SerializeField] GameObject speedMasterUI;
    [SerializeField] GameObject holsterMasterUI;
    [SerializeField] GameObject armoredFortUI;
    [SerializeField] GameObject bulletLegionUI;
    [SerializeField] GameObject flyingHandUI;
    [SerializeField] GameObject headBlowerUI;
    [SerializeField] GameObject wildcardUI;
    [SerializeField] GameObject devilPactUI;
    [SerializeField] GameObject strikeUI;

    public void AddPowerUpImageFeedback(Perk perkMan)
    {
        PerkType type = perkMan.perkType;

        if (type == PerkType.speedMaster)
            speedMasterUI.GetComponentInChildren<PerkUIIconLogic>().ActivatePerkUI();
        if (type == PerkType.holsterMaster)
            holsterMasterUI.GetComponentInChildren<PerkUIIconLogic>().ActivatePerkUI();
        if (type == PerkType.armoredFort)
            armoredFortUI.GetComponentInChildren<PerkUIIconLogic>().ActivatePerkUI();
        if (type == PerkType.bulletLegion)
            bulletLegionUI.GetComponentInChildren<PerkUIIconLogic>().ActivatePerkUI();
        if (type == PerkType.flyingHand)
            flyingHandUI.GetComponentInChildren<PerkUIIconLogic>().ActivatePerkUI();
        if (type == PerkType.headBlower)
            headBlowerUI.GetComponentInChildren<PerkUIIconLogic>().ActivatePerkUI();
        if (type == PerkType.wildcard)
            wildcardUI.GetComponentInChildren<PerkUIIconLogic>().ActivatePerkUI();
        if (type == PerkType.devilPact)
            devilPactUI.GetComponentInChildren<PerkUIIconLogic>().ActivatePerkUI();
        if (type == PerkType.goldenStrike)
            strikeUI.GetComponentInChildren<PerkUIIconLogic>().ActivatePerkUI();
    }
}
