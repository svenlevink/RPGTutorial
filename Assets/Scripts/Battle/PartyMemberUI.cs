using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;
    [SerializeField] Image partySprite;
    [SerializeField] Text hpRemainText;
    [SerializeField] Text messageText;

    Pokemon _pokemon;

    public void Init(Pokemon pokemon)
    {
        _pokemon = pokemon;
        UpdateData();
        SetMessage("");

        _pokemon.OnHPChanged += () => UpdateData();
    }

    void UpdateData()
    {
        partySprite.sprite = _pokemon.Base.PartySprite;
        nameText.text = _pokemon.Base.Name;
        levelText.text = "Lv" + _pokemon.Level;
        if(_pokemon.MaxHp > 0)
        {
            hpBar.SetHP((float)_pokemon.HP / _pokemon.MaxHp);
        }
        hpRemainText.text = $"{(float)_pokemon.HP} / {(float)_pokemon.MaxHp}";
    }

    public void SetSelected(bool selected)
    {
        if (selected)
            nameText.color = GlobalSettings.i.HighlightedColor;
        else
            nameText.color = Color.black;
    }   
    
    public void SetMessage(string message)
    {
        messageText.text = message;
    }
}
