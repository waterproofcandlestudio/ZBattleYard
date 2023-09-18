using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerkMachineLogic : MonoBehaviour
{
    [SerializeField] Perk perk;

    public Perk GetPerkInfo() => perk;
}
