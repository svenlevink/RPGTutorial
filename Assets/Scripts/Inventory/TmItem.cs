using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new TM or HM")]

public class TmItem : ItemBase
{
    [SerializeField] MoveBase move;
    [SerializeField] bool isHM;

    public override string Name => base.Name + $" - {move.Name}";

    public override bool Use(Pokemon pokemon)
    {
        //learning move is handled from Inventory Ui, if it was learned then return true
        return pokemon.HasMove(move);
    }

    public bool CanBeTaught(Pokemon pokemon)
    {
        return pokemon.Base.LearnableByItems.Contains(move);
    }

    public override bool CanUseInBattle => false;

    public override bool IsReusable => isHM;

    public MoveBase Move => move;
    public bool IsHM => isHM;

}
