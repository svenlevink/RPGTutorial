using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new Pokemon")]

public class PokemonBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;
    [SerializeField] Sprite partySprite;

    [SerializeField] PokemonType type1;
    [SerializeField] PokemonType type2;

    // Base Stats
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    [SerializeField] int expYield;
    [SerializeField] GrowthRate growthRate;

    [SerializeField] int catchRate = 255;

    [SerializeField] List<LearnableMove> learnableMoves;
    [SerializeField] List<MoveBase> learnableByItems;

    [SerializeField] List<Evolution> evolutions; 

    public int GetExpForLevel (int level)
    {
        if (growthRate == GrowthRate.Fast)
        {
            return 4 * (level * level * level) / 5;
        }
        else if (growthRate == GrowthRate.MediumFast)
        {
            return level * level * level;
        }
        else if (growthRate == GrowthRate.MediumSlow)
        {
            return ((6 / 5) * (level * level * level)) - (15 * (level * level)) + (100 * level) - 140;
        }
        else if (growthRate == GrowthRate.Slow)
        {
            return 5 * (level * level * level) / 4;
        }
        else if (growthRate == GrowthRate.Fluctuating)
        {
            if (level < 15)
            {
                return ((level * level * level) * (((level + 1) / 3) + 24)) / 50;
            }
            else if (level >= 15 && level < 36)
            {
                return ((level * level * level) * (level + 14)) / 50;
            }
            else if (level >= 36 && level < 100)
            {
                return ((level * level * level) * (((level + 1) / 3) + 32)) / 50;
            }
        }
        return -1;
    }

    public string Name
    {
        get { return name; }
    }

    public string Description
    {
        get { return description; }
    }

    public Sprite FrontSprite
    {
        get { return frontSprite; }
    }

    public Sprite BackSprite
    {
        get { return backSprite; }
    }

    public Sprite PartySprite
    {
        get { return partySprite; }
    }

    public PokemonType Type1
    {
        get { return type1; }
    }

    public PokemonType Type2
    {
        get { return type2; }
    }

    public int MaxHp
    {
        get { return maxHp; }
    }

    public int Defense
    {
        get { return defense; }
    }

    public int SpAttack
    {
        get { return spAttack; }
    }

    public int SpDefense
    {
        get { return spDefense; }
    }

    public int Speed
    {
        get { return speed; }
    }

    public int Attack
    {
        get { return attack; }
    }

    public int CatchRate
    {
        get { return catchRate; }
    }

    public int ExpYield
    {
        get { return expYield; }
    }

    public GrowthRate GrowthRate
    {
        get { return growthRate; }
    }

    public List<LearnableMove> LearnableMoves
    {
        get { return learnableMoves; }
    }

    public List<MoveBase> LearnableByItems
    {
        get { return learnableByItems; }
    }

    public List<Evolution> Evolutions
    {
        get { return evolutions; }
    }
}

[System.Serializable]

public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase Base
    {
        get { return moveBase; }
    }

    public int Level
    {
        get { return level; }
    }
}

[System.Serializable]

public class Evolution
{
    [SerializeField] PokemonBase evolvesInto;
    [SerializeField] int requiredLevel;

    public PokemonBase EvolvesInto => evolvesInto;
    public int RequiredLevel => requiredLevel;
}

public enum PokemonType
{
    None,
    Normal, 
    Fire, 
    Water, 
    Grass, 
    Flying, 
    Fighting, 
    Poison, 
    Electric, 
    Ground, 
    Rock, 
    Psychic, 
    Ice, 
    Bug, 
    Ghost, 
    Steel, 
    Dragon, 
    Dark, 
    Fairy
}

// Bij Growthrate nog Erratic toevoegen!!

public enum GrowthRate
{
    Fast, MediumFast, MediumSlow, Slow, Fluctuating
}

public enum Stat
{
    Attack,
    Defense,
    SpAttack,
    SpDefense,
    Speed,

    // not actual stats
    Accuracy,
    Evasion
}

public class TypeChart
{
    static float[][] chart =
    {
        //                         NOR  FIR WAT GRA FLY FIG PO ELE GRO ROC PSY ICE BUG GHO STE DRAG DAR FAI
        /*Normal*/    new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 1f, 1f, 1f, 0f, 0.5f, 1f, 1f, 1f},
        /*Fire*/      new float[] { 1f, 0.5f, 0.5f, 2f, 1f, 1f, 1f, 1f, 1f, 0.5f, 1f, 2f, 2f, 1f, 2f, 0.5f, 1f, 1f},
        /*Water*/     new float[] { 1f, 2f, 0.5f, 0.5f, 1f, 1f, 1f, 1f, 2f, 2f, 1f, 1f, 1f, 1f, 1f, 0.5f, 1f, 1f},
        /*Grass*/     new float[] { 1f, 0.5f, 2f, 0.5f, 0.5f, 1f, 0.5f, 1f, 2f, 2f, 1f, 1f, 0.5f, 1f, 0.5f, 0.5f, 1f, 1f},
        /*Flying*/    new float[] { 1f, 1f, 1f, 2f, 1f, 2f, 1f, 0.5f, 1f, 0.5f, 1f, 1f, 2f, 1f, 0.5f, 1f, 1f, 1f},
        /*Fighting*/  new float[] { 2f, 1f, 1f, 1f, 0.5f, 1f, 0.5f, 1f, 1f, 2f, 0.5f, 2f, 0.5f, 0f, 2f, 1f, 2f, 0.5f},
        /*Poison*/    new float[] { 1f, 1f, 1f, 2f, 1f, 1f, 0.5f, 1f, 0.5f, 0.5f, 1f, 1f, 1f, 0.5f, 0f, 1f, 1f, 2f},
        /*Electric*/  new float[] { 1f, 1f, 2f, 0.5f, 2f, 1f, 1f, 0.5f, 0f, 1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 1f, 1f},
        /*Ground*/    new float[] { 1f, 2f, 1f, 0.5f, 0f, 1f, 2f, 2f, 1f, 2f, 1f, 1f, 0.5f, 1f, 2f, 1f, 1f, 1f},
        /*Rock*/      new float[] { 1f, 2f, 1f, 1f, 2f, 0.5f, 1f, 1f, 0.5f, 1f, 1f, 2f, 2f, 1f, 0.5f, 1f, 1f, 1f},
        /*Psychic*/   new float[] { 1f, 1f, 1f, 1f, 1f, 2f, 2f, 1f, 1f, 1f, 0.5f, 1f, 1f, 1f, 0.5f, 1f, 0f, 1f},
        /*Ice*/       new float[] { 1f, 0.5f, 0.5f, 2f, 2f, 1f, 1f, 1f, 2f, 1f, 1f, 0.5f, 1f, 1f, 0.5f, 2f, 1f, 1f},
        /*Bug*/       new float[] { 1f, 0.5f, 1f, 2f, 0.5f, 0.5f, 0.5f, 1f, 1f, 1f, 2f, 1f, 1f, 0.5f, 0.5f, 1f, 0.5f, 1f},
        /*Ghost*/     new float[] { 0f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 2f, 1f, 1f, 0.5f, 1f},
        /*Steel*/     new float[] { 1f, 0.5f, 0.5f, 1f, 1f, 1f, 1f, 0.5f, 1f, 2f, 1f, 2f, 1f, 1f, 0.5f, 1f, 1f, 2f},
        /*Dragon*/    new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 2f, 1f, 0f},
        /*Dark*/      new float[] { 1f, 1f, 1f, 1f, 1f, 0.5f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 2f, 1f, 1f, 0.5f, 0.5f},
        /*Fairy*/     new float[] { 1f, 0.5f, 1f, 1f, 1f, 2f, 0.5f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 2f, 2f, 1f},
    };

    public static float GetEffectiveness(PokemonType attackType, PokemonType defenseType)
    {
        if (attackType == PokemonType.None || defenseType == PokemonType.None)
            return 1;

        int row = (int)attackType - 1;
        int col = (int)defenseType - 1;

        return chart[row][col];
    }
}