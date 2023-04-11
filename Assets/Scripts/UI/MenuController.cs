using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject menu;

    List<Text> menuItems;

    int selectedItem = 0;
    public event Action<int> onMenuSelected;
    public event Action onBack;

    private void Awake()
    {
        menuItems = menu.GetComponentsInChildren<Text>().ToList();
    }

    public void OpenMenu()
    {
        menu.SetActive(true);
        UpdateItemSelection();
    }

    public void CloseMenu()
    {
        menu.SetActive(false);
    }

    public void HandleUpdate()
    {
        int prevSelection = selectedItem;

        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++selectedItem;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --selectedItem;

        selectedItem = Mathf.Clamp(selectedItem, 0, menuItems.Count - 1);

        if (prevSelection != selectedItem)
            UpdateItemSelection();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            onMenuSelected?.Invoke(selectedItem);
            CloseMenu();
            selectedItem = 0;
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            onBack?.Invoke();
            CloseMenu();
            selectedItem = 0;
        }
    }

    void UpdateItemSelection()
    {
        for (int i = 0; i < menuItems.Count; ++i)
        {
            if (i == selectedItem)
                menuItems[i].color = GlobalSettings.i.HighlightedColor;
            else
                menuItems[i].color = Color.black;
        }
    }
}
