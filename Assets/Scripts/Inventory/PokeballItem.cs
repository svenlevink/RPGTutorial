using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.InteropServices;

[CreateAssetMenu(menuName = "Items/Create new Pokeball")]

public class PokeballItem : ItemBase
{
    [SerializeField] float catchrateModifier = 1;

    public override bool Use(Pokemon pokemon)
    {
        return true;
    }

    public override bool CanUseOutsideBattle => false;

    public float CatchRateModifier => catchrateModifier;
}
