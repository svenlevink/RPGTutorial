using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text statusText;
    [SerializeField] Text hpRemainText;
    [SerializeField] HPBar hpBar;
    [SerializeField] GameObject expBar;

    [SerializeField] Color psnColor;
    [SerializeField] Color brnColor;
    [SerializeField] Color parColor;
    [SerializeField] Color slpColor;
    [SerializeField] Color frzColor;

    Pokemon _pokemon;
    Dictionary<ConditionID, Color> statusColors;

    //zet de data van de pokemon in de Hud
    public void SetData(Pokemon pokemon)
    {
        if (_pokemon != null)
        {
            pokemon.OnHPChanged -= UpdateHP;
            _pokemon.OnStatusChanged -= SetStatusText;
        }
        _pokemon = pokemon;

        nameText.text = pokemon.Base.Name;
        SetLevel();
        hpBar.SetHP((float)pokemon.HP / pokemon.MaxHp);
        SetExp();
        SetRemainHP();

        statusColors = new Dictionary<ConditionID, Color>()
        {
            {ConditionID.psn, psnColor },
            {ConditionID.brn, brnColor },
            {ConditionID.slp, slpColor },
            {ConditionID.par, parColor },
            {ConditionID.frz, frzColor },
        };

        SetStatusText();
        _pokemon.OnStatusChanged += SetStatusText;
        _pokemon.OnHPChanged += UpdateHP;
    }

    void SetStatusText()
    {
       if (_pokemon.Status == null)
        {
            statusText.text = "";
        }
       else
        {
            statusText.text = _pokemon.Status.Id.ToString().ToUpper();
            statusText.color = statusColors[_pokemon.Status.Id];
        }
    }

    //Bepaalt level van pokemon (ook na level up)
    public void SetLevel()
    {
        levelText.text = "Lv" + _pokemon.Level;
    }

    public void SetRemainHP()
    {
        if (hpRemainText == null) return;

        //het veranderen van de health text
        hpRemainText.text = $"{_pokemon.HP} / {_pokemon.MaxHp}";
    }

    public void SetExp()
    {
        //als er geen xpbar is dan boeiuh (bij enemies)
        if (expBar == null) return;

        //het veranderen van de xpbar
        float normalizedExp = GetNormalizedExp();
        expBar.transform.localScale = new Vector3(normalizedExp, 1, 1);
    }

    public IEnumerator SetExpSmooth(bool reset=false)
    {
        //als er geen xpbar is dan boeiuh (bij enemies)
        if (expBar == null) yield break;

        //als level up, xp bar weer leeg maken
        if (reset)
            expBar.transform.localScale = new Vector3(0, 1, 1);

        //het veranderen van de xpbar (maar dan smooth)
        float normalizedExp = GetNormalizedExp();
        yield return expBar.transform.DOScaleX(normalizedExp, 1.5f).WaitForCompletion();
    }

    // achterhaal hoeveel xp nodig is voor level up
    float GetNormalizedExp()
    {
        int currentLevelExp = _pokemon.Base.GetExpForLevel(_pokemon.Level);
        int nextLevelExp = _pokemon.Base.GetExpForLevel(_pokemon.Level + 1);

        //normalize current xp...
        float normalizedExp = (float)(_pokemon.Exp - currentLevelExp) / (nextLevelExp - currentLevelExp);

        return Mathf.Clamp01(normalizedExp);
    }

    public void UpdateHP()
    {
        StartCoroutine(UpdateHPAsync());
    }

    public IEnumerator UpdateHPAsync()
    {
        yield return hpBar.SetHPSmooth((float)_pokemon.HP / _pokemon.MaxHp);
        SetRemainHP();
    }

    public IEnumerator WaitForHPUpdate()
    {
        yield return new WaitUntil(() => hpBar.IsUpdating == false);
    }

    public void ClearData()
    {
        if (_pokemon != null)
        {
            _pokemon.OnHPChanged -= UpdateHP;
            _pokemon.OnStatusChanged -= SetStatusText;
        }
    }
}
