using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    [SerializeField] Color highlightedColor;

    public Color HighlightedColor => highlightedColor;

    public static GlobalSettings i { get; private set; }

    public void Awake()
    {
        i = this;
    }
}
