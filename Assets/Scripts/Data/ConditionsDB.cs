using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
    public static void Init()
    {
        foreach (var kvp in Conditions)
        {
            var conditionId = kvp.Key;
            var condition = kvp.Value;

            condition.Id = conditionId;
        }
    }

    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {ConditionID.psn, new Condition()
            {
                Name = "Poison",
                StartMessage = "has been poisoned!",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.DecreaseHP(pokemon.MaxHp / 8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is slowly dying of the poison!");
                }
            }
        },
        {ConditionID.brn, new Condition()
            {
                Name = "Burn",
                StartMessage = "has been burned!",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.DecreaseHP(pokemon.MaxHp / 16);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is slowly burning to death!");
                }
            }
        },
        {ConditionID.slp, new Condition()
            {
                Name = "Sleep",
                StartMessage = "has fallen asleep",
                OnStart = (Pokemon pokemon) =>
                {
                    //sleep for 1-3 turns
                    pokemon.StatusTime = UnityEngine.Random.Range(1, 4);
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (pokemon.StatusTime <= 0)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} woke up!");
                        return true;
                    }

                    pokemon.StatusTime--;
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is fast asleep.");
                    return false;
                }

            }
        },
        {ConditionID.par, new Condition()
            {
                Name = "Paralyzed",
                StartMessage = "has been paralyzed!",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (UnityEngine.Random.Range(1, 5) == 1)
                    {
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is paralyzed and can't move!");
                        return false;
                    }

                    return true;
                }
            }
        },
        {ConditionID.frz, new Condition()
            {
                Name = "Freeze",
                StartMessage = "has been frozen solid!",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (UnityEngine.Random.Range(1, 5) == 1)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} thawed out!");
                        return true;
                    }
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is frozen solid.");
                    return false;
                }
            }
        },

        // Volatile status conditions

        {ConditionID.confusion, new Condition()
            {
                Name = "Confusion",
                StartMessage = "is now confused!",
                OnStart = (Pokemon pokemon) =>
                {
                    //Confused for 1-4 turns
                    pokemon.VolatileStatusTime = UnityEngine.Random.Range(1, 5);
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (pokemon.VolatileStatusTime <= 0)
                    {
                        pokemon.CureVolatileStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} snapped out of its confusion!");
                        return true;
                    }
                    pokemon.VolatileStatusTime--;
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is confused...");

                    // 50% chance to do something
                    if (UnityEngine.Random.Range(1, 3) == 1)
                        return true;

                    pokemon.DecreaseHP(pokemon.MaxHp / 8);
                    pokemon.StatusChanges.Enqueue($"It hurt itself in its confusion!");
                    return false;
                }

            }
        }
    };

    public static float GetStatusBonus(Condition condition)
    {
        if (condition == null)
            return 1f;
        else if (condition.Id == ConditionID.slp || condition.Id == ConditionID.frz)
            return 2f;
        else if (condition.Id == ConditionID.par || condition.Id == ConditionID.psn || condition.Id == ConditionID.brn)
            return 1.5f;

        return 1f;
    }

}

public enum ConditionID
{
    none, psn, brn, slp, par, frz,
    confusion
}
