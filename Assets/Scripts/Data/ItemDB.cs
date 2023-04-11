using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDB
{
    //zet alle items in een dictionary
    static Dictionary<string, ItemBase> items;

    public static void Init()
    {
        items = new Dictionary<string, ItemBase>();

        //alles laden
        var itemList = Resources.LoadAll<ItemBase>("");
        foreach (var item in itemList)
        {
            if (items.ContainsKey(item.Name))
            {
                continue;
            }
            items[item.Name] = item;
        }
    }

    //achterhalen (is sneller zo)
    public static ItemBase GetItemByName(string name)
    {
        if (!items.ContainsKey(name))
        {
            return null;
        }

        return items[name];
    }
}
